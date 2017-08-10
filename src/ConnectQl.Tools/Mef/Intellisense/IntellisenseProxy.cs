// MIT License
//
// Copyright (c) 2017 Maarten van Sambeek.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace ConnectQl.Tools.Mef.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using ConnectQl.Tools.AssemblyLoader;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using VSLangProj;

    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// The intellisense proxy.
    /// </summary>
    internal class IntellisenseProxy : IDisposable
    {
        /// <summary>
        /// The app domain.
        /// </summary>
        private AppDomain appDomain;

        /// <summary>
        /// The assembly paths.
        /// </summary>
        private string[] watchPaths;

        /// <summary>
        /// The handler.
        /// </summary>
        private RemoteEventHandler<Tuple<string, byte[]>> handler;

        /// <summary>
        /// The imports events.
        /// </summary>
        private ImportsEvents importsEvents;

        /// <summary>
        /// The session.
        /// </summary>
        private AppDomainIntellisenseSession intellisenseSession;

        /// <summary>
        /// The references events.
        /// </summary>
        private ReferencesEvents referencesEvents;

        /// <summary>
        /// The watchers.
        /// </summary>
        private List<FileSystemWatcher> watchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntellisenseProxy"/> class.
        /// </summary>
        /// <param name="projectUniqueName">
        /// The project unique name.
        /// </param>
        /// <param name="documents">
        /// The documents.
        /// </param>
        public IntellisenseProxy(string projectUniqueName, IDictionary<string, ConnectQlDocument> documents)
        {
            this.Init(projectUniqueName, documents);
        }

        /// <summary>
        /// The document updated.
        /// </summary>
        public event EventHandler<DocumentUpdatedEventArgs> DocumentUpdated;

        /// <summary>
        /// The initialized.
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// The reload requested.
        /// </summary>
        public event EventHandler ReloadRequested;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.referencesEvents != null)
            {
                this.referencesEvents.ReferenceAdded -= this.ReferencesUpdated;
                this.referencesEvents.ReferenceChanged -= this.ReferencesUpdated;
                this.referencesEvents.ReferenceRemoved -= this.ReferencesUpdated;
                this.referencesEvents = null;
            }

            if (this.importsEvents != null)
            {
                this.importsEvents.ImportAdded -= this.ImportsUpdated;
                this.importsEvents.ImportRemoved -= this.ImportsUpdated;
                this.importsEvents = null;
            }

            foreach (var watcher in this.watchers)
            {
                watcher.Changed -= this.FileChanged;
                watcher.Renamed -= this.FileChanged;
                watcher.Deleted -= this.FileChanged;
                watcher.Created -= this.FileChanged;

                watcher.Dispose();
            }

            this.watchers.Clear();

            if (this.intellisenseSession != null)
            {
                if (this.handler != null)
                {
                    this.intellisenseSession.ClassificationChanged -= this.handler.Handler;
                    this.handler = null;
                }

                this.intellisenseSession = null;
            }

            AppDomain.Unload(this.appDomain);

            this.appDomain = null;
        }

        /// <summary>
        /// Initializes the proxy.
        /// </summary>
        /// <param name="projectId">
        /// The project id.
        /// </param>
        /// <param name="documents">
        /// The documents.
        /// </param>
        public void Init(string projectId, IDictionary<string, ConnectQlDocument> documents)
        {
            Debug.WriteLine("Initializing proxy.");

            Task.Run(() =>
                {
                    try
                    {
                        var visualStudioProject = GetVisualStudioProjectByUniqueName(projectId);

                        if (visualStudioProject == null)
                        {
                            this.Dispose();

                            return;
                        }

                        try
                        {
                            this.referencesEvents = visualStudioProject.Events.ReferencesEvents;

                            this.referencesEvents.ReferenceAdded += this.ReferencesUpdated;
                            this.referencesEvents.ReferenceChanged += this.ReferencesUpdated;
                            this.referencesEvents.ReferenceRemoved += this.ReferencesUpdated;
                        }
                        catch (NotImplementedException)
                        {
                            Debug.WriteLine("ReferencesEvents not implemented for this project.");
                        }

                        try
                        {
                            this.importsEvents = visualStudioProject.Events.ImportsEvents;
                            this.importsEvents.ImportAdded += this.ImportsUpdated;
                            this.importsEvents.ImportRemoved += this.ImportsUpdated;
                        }
                        catch (NotImplementedException)
                        {
                            Debug.WriteLine("ImportsEvents not implemented for this project.");
                        }

                        var configFile = visualStudioProject.Project.ProjectItems.OfType<ProjectItem>().FirstOrDefault(i => i.Name.EndsWith(".config", StringComparison.OrdinalIgnoreCase))?.FileNames[0];

                        var assemblies = visualStudioProject.References.Cast<Reference>()
                            .Where(r =>
                                {
                                    try
                                    {
                                        return !string.IsNullOrEmpty(r.Path);
                                    }
                                    catch
                                    {
                                        return false;
                                    }
                                })
                            .Select(r => r.Path)
                            .ToArray();

                        var setupInfomation = AppDomain.CurrentDomain.SetupInformation;

                        setupInfomation.ApplicationBase = string.Empty;
                        setupInfomation.ConfigurationFile = configFile;

                        AppDomain.CurrentDomain.AssemblyResolve += AppDomainFix;

                        this.appDomain = AppDomain.CreateDomain($"Intellisense project {visualStudioProject.Project.Name} domain", null, setupInfomation);

                        object[] arguments =
                            {
                                assemblies,
                                typeof(ConnectQlContext).Assembly.Location
                            };

                        this.watchPaths = assemblies
                            .Concat(new[]
                                        {
                                            configFile
                                        }).ToArray();

                        this.WatchPaths(this.watchPaths);

                        this.handler = new RemoteEventHandler<Tuple<string, byte[]>>(this.IntellisenseSessionOnClassificationChanged);

                        this.intellisenseSession = (AppDomainIntellisenseSession)this.appDomain.CreateInstanceFromAndUnwrap(
                            typeof(AppDomainIntellisenseSession).Assembly.Location ?? string.Empty,
                            typeof(AppDomainIntellisenseSession).FullName,
                            false,
                            BindingFlags.CreateInstance,
                            null,
                            arguments,
                            CultureInfo.CurrentCulture,
                            null);

                        this.intellisenseSession.ClassificationChanged += this.handler.Handler;

                        foreach (var keyValuePair in documents)
                        {
                            this.intellisenseSession.UpdateDocument(keyValuePair.Key, keyValuePair.Value.Content);
                        }

                        this.Initialized?.Invoke(this, EventArgs.Empty);

                        Debug.WriteLine("Initialized proxy.");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Error while initializing proxy: {e.Message}.");

                        this.Dispose();

                        Task.Run(async () =>
                            {
                                await Task.Delay(TimeSpan.FromSeconds(1));

                                this.Init(projectId, documents);
                            });
                    }
                    finally
                    {
                        AppDomain.CurrentDomain.AssemblyResolve -= AppDomainFix;
                    }
                });
        }

        /// <summary>
        /// Updates the document.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="contents">
        /// The contents.
        /// </param>
        public void UpdateDocument(string filename, string contents)
        {
            this.intellisenseSession.UpdateDocument(filename, contents);
        }

        /// <summary>
        /// Loads the correct (already loaded) assembly.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        /// <returns>
        /// The <see cref="Assembly"/>.
        /// </returns>
        private static Assembly AppDomainFix(object sender, ResolveEventArgs e)
        {
            return e.Name == typeof(AppDomainIntellisenseSession).Assembly.FullName ? typeof(AppDomainIntellisenseSession).Assembly : null;
        }

        /// <summary>
        /// Gets the projects recursively.
        /// </summary>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        private static IEnumerable<Project> GetProjectsRecursive(Project project)
        {
            if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
            {
                var subProjects = project.ProjectItems
                    .Cast<ProjectItem>()
                    .Select(p => p.SubProject)
                    .Where(p => p != null)
                    .SelectMany(GetProjectsRecursive);

                foreach (var subProject in subProjects)
                {
                    yield return subProject;
                }
            }
            else
            {
                yield return project;
            }
        }

        /// <summary>
        /// Gets a visual studio project by its unique name.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <returns>
        /// The <see cref="VSProject"/>.
        /// </returns>
        private static VSProject GetVisualStudioProjectByUniqueName(string uniqueName)
        {
            return ((DTE)Package.GetGlobalService(typeof(SDTE))).Solution.Projects
                   .Cast<Project>()
                   .SelectMany(GetProjectsRecursive)
                   .FirstOrDefault(p => p.UniqueName == uniqueName)?.Object as VSProject;
        }

        /// <summary>
        /// The file changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="fileSystemEventArgs">
        /// The file system event args.
        /// </param>
        private void FileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (!this.watchPaths.Any(path => path.Equals(fileSystemEventArgs.FullPath, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            this.ReloadRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// The imports updated.
        /// </summary>
        /// <param name="import">
        /// The import.
        /// </param>
        private void ImportsUpdated(string import)
        {
            this.ReloadRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the <see cref="AppDomainIntellisenseSession.ClassificationChanged"/> event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="tuple">
        /// The tuple.
        /// </param>
        private void IntellisenseSessionOnClassificationChanged(object sender, Tuple<string, byte[]> tuple)
        {
            this.DocumentUpdated?.Invoke(this, new DocumentUpdatedEventArgs(tuple.Item1, ConnectQl.Intellisense.IntellisenseSession.Deserialize(tuple.Item2)));
        }

        /// <summary>
        /// The references updated.
        /// </summary>
        /// <param name="reference">
        /// The p reference.
        /// </param>
        private void ReferencesUpdated(Reference reference)
        {
            this.ReloadRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// The watch paths.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        private void WatchPaths(IReadOnlyCollection<string> arguments)
        {
            if (arguments.Count == 0)
            {
                return;
            }

            this.watchers = new List<FileSystemWatcher>();
            var paths = arguments.Select(Path.GetDirectoryName).OrderBy(argument => argument).Where(argument => argument != null).ToArray();
            var last = paths[0];

            this.watchers.Add(new FileSystemWatcher
                                  {
                                      Path = last,
                                      IncludeSubdirectories = true,
                                      EnableRaisingEvents = true,
                                      NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.DirectoryName,
                                      Filter = "*.*"
                                  });

            foreach (var path in paths.Skip(1))
            {
                if (path.StartsWith(last, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                this.watchers.Add(new FileSystemWatcher
                                      {
                                          Path = path,
                                          IncludeSubdirectories = true,
                                          EnableRaisingEvents = true,
                                          NotifyFilter = (NotifyFilters)383,
                                          Filter = "*.*"
                                      });

                last = path;
            }

            foreach (var fileSystemWatcher in this.watchers)
            {
                fileSystemWatcher.Changed += this.FileChanged;
                fileSystemWatcher.Renamed += this.FileChanged;
                fileSystemWatcher.Deleted += this.FileChanged;
                fileSystemWatcher.Created += this.FileChanged;
            }
        }
    }
}