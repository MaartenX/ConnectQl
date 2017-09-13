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

namespace ConnectQl.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Ast.Statements;
    using ConnectQl.Internal.Ast.Visitors;

    /// <summary>
    /// The plugin loader.
    /// </summary>
    internal class PluginLoader : NodeVisitor
    {
        private readonly IConnectQlPlugin[] plugins;
        private readonly IPluginContext context;
        private readonly HashSet<IConnectQlPlugin> registered = new HashSet<IConnectQlPlugin>();
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginLoader"/> class.
        /// </summary>
        /// <param name="plugins">The plugins.</param>
        /// <param name="loggers">The loggers.</param>
        private PluginLoader(IConnectQlPlugin[] plugins, LoggerCollection loggers)
        {
            this.plugins = plugins;
            this.context = new PluginContext(loggers);
            this.logger = loggers;
        }

        /// <summary>
        /// Loads log plugins from the script, so errors are logged.
        /// </summary>
        /// <param name="script">
        /// The script to load the imports from.
        /// </param>
        /// <param name="loggers">
        /// The loggers collection to add the loggers to.
        /// </param>
        /// <param name="pluginResolver">
        /// The plugin resolver to use.
        /// </param>
        public static void LoadLogPlugins(Block script, LoggerCollection loggers, IPluginResolver pluginResolver)
        {
            new PluginLoader(pluginResolver.EnumerateAvailablePlugins().ToArray(), loggers).Visit(script);
        }

        /// <summary>
        /// Visits a <see cref="ImportPluginStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitImportPluginStatement(ImportPluginStatement node)
        {
            var plugin = this.plugins.FirstOrDefault(p => string.Equals(p.Name,node.Plugin, StringComparison.OrdinalIgnoreCase));

            try
            {
                if (this.registered.Add(plugin))
                {
                    plugin?.RegisterPlugin(this.context);
                }
            }
            catch (Exception e)
            {
                this.logger.Error(e);
            }

            return node;
        }

        /// <summary>
        /// The plugin context.
        /// </summary>
        private class PluginContext : IPluginContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PluginContext"/> class.
            /// </summary>
            /// <param name="loggers">
            /// The loggers.
            /// </param>
            public PluginContext(LoggerCollection loggers)
            {
                this.Loggers = loggers;
                this.Functions = new ConnectQlFunctions(new Dictionary<string, IFunctionDescriptor>(), () => loggers);
            }

            /// <summary>
            /// Gets the functions.
            /// </summary>
            public IConnectQlFunctions Functions { get; }

            /// <summary>
            /// Gets the file formats.
            /// </summary>
            public IFileFormats FileFormats { get; } = new FileFormatsImplementation();

            /// <summary>
            /// Gets the loggers.
            /// </summary>
            public ICollection<ILogger> Loggers { get; }
        }
    }
}