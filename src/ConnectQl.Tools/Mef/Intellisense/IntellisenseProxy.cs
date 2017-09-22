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
    using System.Security;
    using System.Security.Permissions;

    using AssemblyLoader;

    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;

    using EnvDTE;

    using EnvDTE80;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using VSLangProj;

    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// The intellisense proxy.
    /// </summary>
    internal class IntellisenseProxy : IIntellisenseSession
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
        private RemoteEventHandler<byte[]> handler;

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
        public IntellisenseProxy(string projectUniqueName)
        {
            this.Init(projectUniqueName);
        }

        /// <summary>
        /// Occurs when a document is updated.
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

            if (this.watchers != null)
            {
                foreach (var watcher in this.watchers)
                {
                    watcher.Changed -= this.FileChanged;
                    watcher.Renamed -= this.FileChanged;
                    watcher.Deleted -= this.FileChanged;
                    watcher.Created -= this.FileChanged;

                    watcher.Dispose();
                }

                this.watchers.Clear();
                this.watchers = null;
            }

            if (this.intellisenseSession != null)
            {
                if (this.handler != null)
                {
                    this.intellisenseSession.DocumentUpdated -= this.handler.Handler;
                    this.handler = null;
                }

                this.intellisenseSession = null;
            }

            if (this.appDomain != null)
            {
                AppDomain.Unload(this.appDomain);
            }

            this.appDomain = null;
        }

        /// <summary>
        /// Initializes the proxy.
        /// </summary>
        /// <param name="projectId">
        /// The project id.
        /// </param>
        public void Init(string projectId)
        {
            Task.Run(() =>
                {
                    try
                    {
                        var visualStudioProject = IntellisenseProxy.GetVisualStudioProjectByUniqueName(projectId);
                        var projectName = "Unknown project";
                        var configFile = (string)null;
                        var assemblies = new string[0];

                        if (visualStudioProject != null)
                        {
                            projectName = visualStudioProject.Project.Name;

                            try
                            {
                                this.referencesEvents = visualStudioProject.Events.ReferencesEvents;

                                this.referencesEvents.ReferenceAdded += this.ReferencesUpdated;
                                this.referencesEvents.ReferenceChanged += this.ReferencesUpdated;
                                this.referencesEvents.ReferenceRemoved += this.ReferencesUpdated;
                            }
                            catch (NotImplementedException)
                            {
                                //// CPS does not have reference events.
                            }

                            try
                            {
                                this.importsEvents = visualStudioProject.Events.ImportsEvents;
                                this.importsEvents.ImportAdded += this.ImportsUpdated;
                                this.importsEvents.ImportRemoved += this.ImportsUpdated;
                            }
                            catch (NotImplementedException)
                            {
                                //// CPS does not have import events.
                            }

                            configFile = visualStudioProject.Project.ProjectItems.OfType<ProjectItem>().FirstOrDefault(i => i.Name.EndsWith(".config", StringComparison.OrdinalIgnoreCase))?.FileNames[0];

                            assemblies = visualStudioProject.References.Cast<Reference>()
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
                        }

                        var setupInfomation = AppDomain.CurrentDomain.SetupInformation;

                        setupInfomation.ApplicationBase = string.Empty;
                        setupInfomation.ConfigurationFile = configFile;
                        setupInfomation.LoaderOptimization = LoaderOptimization.SingleDomain;

                        AppDomain.CurrentDomain.AssemblyResolve += IntellisenseProxy.AppDomainFix;

                        this.appDomain = AppDomain.CreateDomain($"Intellisense project {projectName} domain", null, setupInfomation);

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

                        this.handler = new RemoteEventHandler<byte[]>(this.IntellisenseSessionOnDocumentUpdated);

                        this.intellisenseSession = (AppDomainIntellisenseSession)this.appDomain.CreateInstanceFromAndUnwrap(
                            typeof(AppDomainIntellisenseSession).Assembly.Location,
                            typeof(AppDomainIntellisenseSession).FullName ?? string.Empty,
                            false,
                            BindingFlags.CreateInstance,
                            null,
                            arguments,
                            CultureInfo.CurrentCulture,
                            null);

                        this.intellisenseSession.DocumentUpdated += this.handler.Handler;

                        this.Initialized?.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        this.Dispose();

                        Task.Run(async () =>
                            {
                                await Task.Delay(TimeSpan.FromSeconds(1));

                                this.Init(projectId);
                            });
                    }
                    finally
                    {
                        AppDomain.CurrentDomain.AssemblyResolve -= IntellisenseProxy.AppDomainFix;
                    }
                });
        }

        /// <summary>
        /// Updates the document.
        /// </summary>
        /// <param name="filename">
        ///     The filename.
        /// </param>
        /// <param name="contents">
        ///     The contents.
        /// </param>
        /// <param name="documentVersion">
        /// The version of the document.
        /// </param>
        public void UpdateDocument(string filename, string contents, int documentVersion)
        {
            this.intellisenseSession.UpdateDocument(filename, contents, documentVersion);
        }

        /// <summary>
        /// Updates the document span.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="newSpan">The new span text.</param>
        /// <param name="documentVersion">The new document version number.</param>
        public void UpdateDocumentSpan(string filename, int startIndex, int endIndex, string newSpan, int documentVersion)
        {
            this.intellisenseSession.UpdateDocumentSpan(filename, startIndex, endIndex, newSpan, documentVersion);
        }

        /// <summary>
        /// Removes a document.
        /// </summary>
        /// <param name="filename">
        /// The name of the document to remove.
        /// </param>
        public void RemoveDocument(string filename)
        {
            this.intellisenseSession.RemoveDocument(filename);
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>
        /// The document desciptor or <c>null</c> if it wasn't found.
        /// </returns>
        public IDocumentDescriptor GetDocument(string filename)
        {
            return Descriptor.Document(this.intellisenseSession.GetDocumentAsByteArray(filename));
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
                    .SelectMany(IntellisenseProxy.GetProjectsRecursive);

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
                   .SelectMany(IntellisenseProxy.GetProjectsRecursive)
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
        /// Handles the <see cref="AppDomainIntellisenseSession.DocumentUpdated"/> event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="serializedDocumentDescriptor">
        /// The serialized document descriptor.
        /// </param>
        private void IntellisenseSessionOnDocumentUpdated(object sender, byte[] serializedDocumentDescriptor)
        {
            this.DocumentUpdated?.Invoke(this, new DocumentUpdatedEventArgs(serializedDocumentDescriptor));
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
            this.watchers = new List<FileSystemWatcher>();
            var paths = arguments.Select(Path.GetDirectoryName).OrderBy(argument => argument).Where(argument => argument != null).ToArray();

            if (paths.Length == 0)
            {
                return;
            }

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