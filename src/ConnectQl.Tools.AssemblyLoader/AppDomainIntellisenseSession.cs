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

namespace ConnectQl.Tools.AssemblyLoader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The intellisense session implementation.
    /// </summary>
    public class AppDomainIntellisenseSession : MarshalByRefObject
    {
        /// <summary>
        /// The public key.
        /// </summary>
        private static readonly byte[] PublicKey = typeof(AppDomainIntellisenseSession).Assembly.GetName().GetPublicKey();

        /// <summary>
        /// The get document method.
        /// </summary>
        private readonly MethodInfo getDocument;

        /// <summary>
        /// The remove document method.
        /// </summary>
        private readonly MethodInfo removeDocument;

        /// <summary>
        /// The session.
        /// </summary>
        private readonly object session;

        /// <summary>
        /// The update document method.
        /// </summary>
        private readonly MethodInfo updateDocument;

        /// <summary>
        /// The update document span method.
        /// </summary>
        private readonly MethodInfo updateDocumentSpan;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainIntellisenseSession"/> class.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies.
        /// </param>
        /// <param name="fallbackConnectQl">
        /// The fallback ConnectQl assembly when no reference is available in the project.
        /// </param>
        public AppDomainIntellisenseSession(IList<string> assemblies, string fallbackConnectQl)
        {
            AppDomain.CurrentDomain.AssemblyLoad += (o, e) =>
                {
                    Debug.WriteLine($"Assembly loaded: {e.LoadedAssembly.FullName}.");
                };

            var loadedAssemblies = new List<LoadedAssembly>
                                       {
                                           new LoadedAssembly(this.GetType().Assembly)
                                       };

            var handler = CreateAssemblyResolver(loadedAssemblies);

            AppDomain.CurrentDomain.AssemblyResolve += handler;

            var referencedAssemblies = new List<Assembly>();

            foreach (var assemblyFile in assemblies)
            {
                try
                {
                    var pdb = Regex.Replace(assemblyFile, @"\.dll$", ".pdb", RegexOptions.IgnoreCase);
                    var assembly = Assembly.Load(File.ReadAllBytes(assemblyFile), File.Exists(pdb) ? File.ReadAllBytes(pdb) : null, SecurityContextSource.CurrentAppDomain);
                    referencedAssemblies.Add(assembly);
                    loadedAssemblies.Add(new LoadedAssembly(assembly, assemblyFile));
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Error loading assembly {assemblyFile}: {e.Message.TrimEnd('.')}.");

                    // Ignore.
                }
            }

            var connectQl = GetConnectQlAssembly(loadedAssemblies, "ConnectQl");

            if (connectQl == null)
            {
                var pdb = Regex.Replace(fallbackConnectQl, @"\.dll$", ".pdb", RegexOptions.IgnoreCase);
                var assembly = Assembly.Load(File.ReadAllBytes(fallbackConnectQl), File.Exists(pdb) ? File.ReadAllBytes(pdb) : null, SecurityContextSource.CurrentAppDomain);
                referencedAssemblies.Add(assembly);
                loadedAssemblies.Add(new LoadedAssembly(assembly, fallbackConnectQl));

                connectQl = GetConnectQlAssembly(loadedAssemblies, "ConnectQl");
            }

            var assemblyLookup = loadedAssemblies.ToDictionary(a => a.Assembly.GetName().ToString());

            LoadReferencesRecursively(loadedAssemblies, assemblyLookup);

            var pluginLoader = Activator.CreateInstance(connectQl.GetType("ConnectQl.Intellisense.AssemblyPluginResolver"), referencedAssemblies);
            var contextType = connectQl.GetType("ConnectQl.ConnectQlContext");
            var createSession = connectQl.GetType("ConnectQl.Intellisense.ConnectQlExtensions").GetMethod("CreateIntellisenseSession");
            var sessionType = connectQl.GetType("ConnectQl.Intellisense.IntellisenseSession");

            var context = Activator.CreateInstance(contextType, pluginLoader);

            this.session = createSession.Invoke(null, new[] { context });
            this.getDocument = sessionType.GetMethod("GetDocumentAsByteArray");
            this.removeDocument = sessionType.GetMethod("RemoveDocument");
            this.updateDocument = sessionType.GetMethod("UpdateDocument");
            this.updateDocumentSpan = sessionType.GetMethod("UpdateDocumentSpan");

            sessionType.GetEvent("InternalDocumentUpdated", BindingFlags.Public | BindingFlags.Instance)
                .AddEventHandler(this.session, Delegate.CreateDelegate(typeof(EventHandler<byte[]>), this, nameof(this.HandleEvent)));
        }

        /// <summary>
        /// The classification has changed.
        /// </summary>
        public event EventHandler<byte[]> DocumentUpdated;

        /// <summary>
        /// Gets the document by its path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public byte[] GetDocumentAsByteArray(string path)
        {
            object[] arguments =
                {
                    path
                };

            return (byte[])this.getDocument.Invoke(this.session, arguments);
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy
        ///     for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new
        ///     lifetime service object initialized to the value of the
        ///     <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">
        /// The immediate caller does not have infrastructure permission.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
        /// </PermissionSet>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// The remove document.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        public void RemoveDocument(string document)
        {
            object[] arguments =
                {
                    document
                };

            this.removeDocument.Invoke(this.session, arguments);
        }

        /// <summary>
        /// Updates the document.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="contents">
        /// The contents.
        /// </param>
        public void UpdateDocument(string document, string contents)
        {
            object[] arguments =
                {
                    document, contents
                };

           this.updateDocument.Invoke(this.session, arguments);
        }

        /// <summary>
        /// Updates a span in the document.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="endIndex">
        /// The end index.
        /// </param>
        /// <param name="span">
        /// The span.
        /// </param>
        public void UpdateDocumentSpan(string document, int startIndex, int endIndex, string span)
        {
            object[] arguments =
                {
                    document, startIndex, endIndex, span
                };

            this.updateDocumentSpan.Invoke(this.session, arguments);
        }

        /// <summary>
        /// Creates an event wrappedHandler for the <see cref="AppDomain.AssemblyResolve"/> event that
        ///     looks up the assembly by name and public key in the list of already loaded assemblies.
        ///     This means that version mismatches will be ignored.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies.
        /// </param>
        /// <returns>
        /// The <see cref="ResolveEventHandler"/>.
        /// </returns>
        private static ResolveEventHandler CreateAssemblyResolver(IEnumerable<LoadedAssembly> assemblies)
        {
            return (o, e) =>
                {
                    var assemblyName = new AssemblyName(e.Name);
                    var token = assemblyName.GetPublicKeyToken();
                    var result = assemblies.FirstOrDefault(a => a.Assembly.GetName().Name == assemblyName.Name && (token == null || token.SequenceEqual(a.Assembly.GetName().GetPublicKeyToken() ?? new byte[0])));

                    return result?.Assembly;
                };
        }

        /// <summary>
        /// Gets a ConnectQl assembly.
        /// </summary>
        /// <param name="loadedAssemblies">
        /// The loaded assemblies.
        /// </param>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <returns>
        /// The <see cref="Assembly"/>.
        /// </returns>
        private static Assembly GetConnectQlAssembly(IEnumerable<LoadedAssembly> loadedAssemblies, string assemblyName)
        {
            return loadedAssemblies.FirstOrDefault(a =>
                {
                    var name = a.Assembly.GetName();

                    return name.Name.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase) &&
                           (PublicKey.Length == 0 || name.GetPublicKey().SequenceEqual(PublicKey));
                })?.Assembly;
        }

        /// <summary>
        /// Loads the assembly references recursively.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies.
        /// </param>
        /// <param name="assemblyLookup">
        /// The assembly lookup.
        /// </param>
        private static void LoadReferencesRecursively(List<LoadedAssembly> assemblies, IDictionary<string, LoadedAssembly> assemblyLookup)
        {
            var extraAssemblies = new List<LoadedAssembly>();

            foreach (var loadedAssembly in assemblies.ToArray())
            {
                try
                {
                    foreach (var reference in loadedAssembly.Assembly.GetReferencedAssemblies())
                    {
                        if (assemblyLookup.Any(a => a.Key.Split(',')[0].Equals(reference.ToString().Split(',')[0], StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }

                        try
                        {
                            var dlls = new[]
                                           {
                                               $"{reference.Name}.dll",
                                               Path.Combine(Path.GetDirectoryName(loadedAssembly.Path) ?? string.Empty, $"{reference.Name}.dll")
                                           };

                            var dll = dlls.FirstOrDefault(File.Exists);

                            if (dll != null)
                            {
                                var pdb = Regex.Replace(dll, @"\.dll$", ".pdb", RegexOptions.IgnoreCase);
                                var assembly = Assembly.Load(File.ReadAllBytes(dll), File.Exists(pdb) ? File.ReadAllBytes(pdb) : null, SecurityContextSource.CurrentAppDomain);
                                var loaded = new LoadedAssembly(assembly, dll);

                                extraAssemblies.Add(loaded);
                                assemblyLookup[assembly.GetName().ToString()] = loaded;
                            }
                        }
                        catch (FileNotFoundException)
                        {
                        }
                    }
                }
                catch
                {
                    //// Do nothing.
                }
            }

            if (extraAssemblies.Count > 0)
            {
                LoadReferencesRecursively(extraAssemblies, assemblyLookup);

                assemblies.AddRange(extraAssemblies);
            }
        }

        /// <summary>
        /// The handle event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void HandleEvent(object sender, byte[] eventArgs)
        {
            this.DocumentUpdated?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// The loaded assembly.
        /// </summary>
        private class LoadedAssembly
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LoadedAssembly"/> class.
            /// </summary>
            /// <param name="assembly">
            /// The assembly.
            /// </param>
            /// <param name="path">
            /// The path.
            /// </param>
            public LoadedAssembly(Assembly assembly, string path)
            {
                this.Assembly = assembly;
                this.Path = path;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="LoadedAssembly"/> class.
            /// </summary>
            /// <param name="assembly">
            /// The assembly.
            /// </param>
            public LoadedAssembly(Assembly assembly)
            {
                this.Assembly = assembly;
                this.Path = assembly.Location;
            }

            /// <summary>
            /// Gets the assembly.
            /// </summary>
            public Assembly Assembly { get; }

            /// <summary>
            /// Gets the path.
            /// </summary>
            public string Path { get; }
        }
    }
}