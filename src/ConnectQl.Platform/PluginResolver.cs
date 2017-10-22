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

namespace ConnectQl.Platform
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using ConnectQl.Interfaces;

#if NETSTANDARD1_5
    using System.Runtime.Loader;
#endif

    /// <summary>
    /// The plugin resolver that looks for .DLL's in the directory where the executable was started.
    /// </summary>
    public class PluginResolver : IPluginResolver
    {
#if NETSTANDARD1_5
        /// <summary>
        /// The default assembly load context.
        /// </summary>
        private static readonly AssemblyLoadContext DefaultAssemblyLoadContext = AssemblyLoadContext.GetLoadContext(typeof(PluginResolver).GetTypeInfo().Assembly);
#endif

        /// <summary>
        /// The default plugin assemblies (already loaded).
        /// </summary>
        private static readonly Assembly[] DefaultPluginAssemblies = { typeof(ConnectQlContext).GetTypeInfo().Assembly, typeof(PluginResolver).GetTypeInfo().Assembly };

        /// <summary>
        /// The plugins.
        /// </summary>
        private IConnectQlPlugin[] plugins;

        /// <summary>
        /// Loads plugins by assembly name.
        /// </summary>
        /// <returns>
        /// The plugin.
        /// </returns>
        public IEnumerable<IConnectQlPlugin> EnumerateAvailablePlugins()
        {
            var folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (folder == null)
            {
                return Enumerable.Empty<IConnectQlPlugin>();
            }

            return this.plugins
                   ?? (this.plugins = Directory.GetFiles(folder, "*.dll")
                   .SelectMany(PluginResolver.LoadAssembly)
                   .SelectMany(PluginResolver.EnumeratePlugins).ToArray());
        }

        /// <summary>
        /// Loads the assembly from the specified path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/> of assemblies.
        /// </returns>
        private static IEnumerable<Assembly> LoadAssembly(string path)
        {
            Assembly result;

            try
            {
#if NETSTANDARD1_5
                result = PluginResolver.DefaultPluginAssemblies.FirstOrDefault(a => a.Location.Equals(path)) ?? PluginResolver.DefaultAssemblyLoadContext.LoadFromAssemblyPath(path);
#elif NET45
                result = DefaultPluginAssemblies.FirstOrDefault(a => a.Location.Equals(path)) ?? Assembly.LoadFrom(path);
#endif
            }
            catch
            {
#if NETSTANDARD1_5
                // DefaultAssemblyLoadContext.LoadFromAssemblyPath will throw when the assembly is already
                // loaded. In that case, we can load it by its AssemblyName.
                var name = AssemblyLoadContext.GetAssemblyName(path);

                try
                {
                    result = Assembly.Load(name);
                }
                catch
                {
                    yield break;
                }
#elif NET45
                yield break;
#endif
            }

            yield return result;
        }

        /// <summary>
        /// Enumerates the plugins in the specified assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to enumerate the plugins for.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/> of plugins.
        /// </returns>
        private static IEnumerable<IConnectQlPlugin> EnumeratePlugins(Assembly assembly)
        {
            try
            {
                return assembly.ExportedTypes
                        .Where(
                            type =>
                            {
                                var typeInfo = type.GetTypeInfo();

                                return typeInfo.IsPublic && !typeInfo.IsAbstract && typeInfo.IsClass && typeInfo.GetInterface(typeof(IConnectQlPlugin).ToString()) != null;
                            })
                        .Select(Activator.CreateInstance)
                        .Cast<IConnectQlPlugin>();
            }
            catch
            {
                return Enumerable.Empty<IConnectQlPlugin>();
            }
        }
    }
}