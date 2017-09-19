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

namespace ConnectQl.Crm
{
    using ConnectQl.Crm.Sources;
    using ConnectQl.Interfaces;

    /// <summary>
    /// The Microsoft Dynamics CRM plugin.
    /// </summary>
    public class Plugin : IConnectQlPlugin
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => "Crm";

        /// <summary>
        /// Registers the CRM plugin on the context.
        /// </summary>
        /// <param name="context">
        /// The context to register the plugin on.
        /// </param>
        public void RegisterPlugin(IPluginContext context)
        {
            context.Functions
                .AddWithoutSideEffects("ENTITY", (string name) => new EntityDataSource(name))
                .SetDescription("Creates a connection to a CRM entity using the default connection string.", "The name of the table.")
                .AddWithoutSideEffects("ENTITY", (string name, string connectionString) => new EntityDataSource(name, connectionString))
                .SetDescription("Creates a connection to a CRM entity using the specified connection string.", "The name of the entity.", "The connection string.");
        }
    }
}
