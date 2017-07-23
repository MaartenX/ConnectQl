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

namespace ConnectQl.Azure
{
    using ConnectQl.Azure.Sources;
    using ConnectQl.Interfaces;

    /// <summary>
    /// The plugin.
    /// </summary>
    public class Plugin : IConnectQlPlugin
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => "Azure";

        /// <summary>
        /// Registers the plugin on the context.
        /// </summary>
        /// <param name="context">
        /// The context to register the plugin on.
        /// </param>
        public void RegisterPlugin(IPluginContext context)
        {
            context.Functions
                .AddWithoutSideEffects("TABLE", (string name) => new TableDataSource(name))
                .SetDescription("Creates a connection to an Azure table using the default connection string.", "The name of the table.")
                .AddWithoutSideEffects("TABLE", (string name, string connectionString) => new TableDataSource(name, connectionString))
                .SetDescription("Creates a connection to an Azure table using the specified connection string.", "The name of the table.", "The connection string.")
                .AddWithoutSideEffects("BLOB", (string name) => new BlobDataSource(name))
                .SetDescription("Creates a connection to an Azure blob using the default connection string.", "The name of the blob.")
                .AddWithoutSideEffects("BLOB", (string name, string connectionString) => new BlobDataSource(name, connectionString))
                .SetDescription("Creates a connection to an Azure blob using the specified connection string.", "The name of the blob.", "The connection string.");
        }
    }
}