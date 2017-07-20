﻿// MIT License
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
    public class AssemblyPluginResolver : IPluginResolver
    {
        /// <summary>
        /// The assemblies.
        /// </summary>
        private readonly IList<Assembly> assemblies;

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
            this.assemblies = assemblies;
        }

        /// <summary>
        /// Enumerates the available plugins.
        /// </summary>
        /// <returns>
        /// The available plugins.
        /// </returns>
        public IEnumerable<IConnectQlPlugin> EnumerateAvailablePlugins()
        {
            var typeName = typeof(IConnectQlPlugin).FullName;

            return this.plugins ?? (this.plugins = this.assemblies
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
                                        .ToArray());
        }
    }
}