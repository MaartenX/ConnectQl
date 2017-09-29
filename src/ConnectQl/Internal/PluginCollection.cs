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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Interfaces;

    /// <summary>
    /// The plugin collection.
    /// </summary>
    internal class PluginCollection : IPluginCollection
    {
        /// <summary>
        /// The plugins.
        /// </summary>
        private IEnumerable<IConnectQlPlugin> plugins;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCollection"/> class.
        /// </summary>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        public PluginCollection(IPluginResolver resolver)
        {
            if (resolver is IDynamicPluginResolver dynamicResolver)
            {
                dynamicResolver.AvailablePluginsChanged += (o, e) =>
                    {
                        this.IsLoading = dynamicResolver.IsLoading;
                        this.plugins = dynamicResolver.EnumerateAvailablePlugins().ToArray();

                        this.AvailablePluginsChanged?.Invoke(this, e);
                    };

                this.IsLoading = dynamicResolver.IsLoading;
            }

            this.plugins = resolver?.EnumerateAvailablePlugins().ToArray() ?? new IConnectQlPlugin[0];
        }

        /// <summary>
        /// Triggered when the available plugins change.
        /// </summary>
        public event EventHandler AvailablePluginsChanged;

        /// <summary>
        /// Gets a value indicating whether the plugin collection is still loaded.
        /// </summary>
        public bool IsLoading { get; private set; }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IConnectQlPlugin> GetEnumerator() => this.plugins.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}