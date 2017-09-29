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

namespace ConnectQl.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ConnectQl.Interfaces;

    /// <summary>
    /// Resolves plugins using the assembly list provided.
    /// </summary>
    public class AssemblyPluginResolver : IDynamicPluginResolver
    {
        /// <summary>
        /// The assemblies.
        /// </summary>
        private readonly Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

        /// <summary>
        /// The plugins.
        /// </summary>
        private IConnectQlPlugin[] plugins;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyPluginResolver"/> class.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies.
        /// </param>
        public AssemblyPluginResolver(IList<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                this.assemblies[assembly.GetName().ToString()] = assembly;
            }

            this.IsLoading = true;
        }

        /// <summary>
        /// The available plugins changed.
        /// </summary>
        public event EventHandler AvailablePluginsChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the resolver is currently loading plugins.
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Adds assemblies to the resolver.
        /// </summary>
        /// <param name="assembliesToAdd">
        /// The assemblies to add.
        /// </param>
        public void AddAssemblies(IEnumerable<Assembly> assembliesToAdd)
        {
            foreach (var assemblyToAdd in assembliesToAdd)
            {
                this.assemblies[assemblyToAdd.GetName().ToString()] = assemblyToAdd;
            }

            this.plugins = null;

            this.AvailablePluginsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Enumerates the available plugins.
        /// </summary>
        /// <returns>
        /// The available plugins.
        /// </returns>
        public IEnumerable<IConnectQlPlugin> EnumerateAvailablePlugins()
        {
            if (this.plugins != null)
            {
                return this.plugins;
            }

            var typeName = typeof(IConnectQlPlugin).FullName;

            return this.plugins = this.assemblies.Values
                                        .SelectMany(a => a.ExportedTypes
                                            .Select(type => new
                                            {
                                                Type = type,
                                                TypeInfo = type.GetTypeInfo(),
                                            })
                                            .Where(
                                                type =>
                                                    type.TypeInfo.IsPublic && !type.TypeInfo.IsAbstract && type.TypeInfo.IsClass
                                                    && type.TypeInfo.ImplementedInterfaces.Any(i => i.FullName == typeName))
                                            .Select(type => Activator.CreateInstance(type.Type))
                                            .Cast<IConnectQlPlugin>())
                                        .ToArray();
        }
    }
}