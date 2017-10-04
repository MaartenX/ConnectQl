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
    using System.Collections.Generic;
    using System.Linq;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Comparers;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Validation;

    using JetBrains.Annotations;

    /// <summary>
    /// The node data.
    /// </summary>
    internal class NodeDataProvider : INodeDataProvider
    {
        /// <summary>
        /// The node data.
        /// </summary>
        private readonly Dictionary<Node, Dictionary<string, object>> nodeData = new Dictionary<Node, Dictionary<string, object>>(ReferenceEqualityComparer<Node>.Default);

        /// <summary>
        /// Gets all values for the node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// An enumerable of keys and values.
        /// </returns>
        [NotNull]
        public KeyValuePair<string, object>[] GetAllValues(Node node)
        {
            return this.nodeData.TryGetValue(node, out Dictionary<string, object> values) ? values.OrderBy(kv => kv.Key).ToArray() : new KeyValuePair<string, object>[0];
        }

        /// <summary>
        /// Transfers the data from a node to another node.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="overwrite">
        /// <c>true</c> to overwrite values that already existed, <c>false</c> to keep them.
        /// </param>
        /// <typeparam name="T">
        /// The type of the node.
        /// </typeparam>
        /// <returns>
        /// The node that received the data.
        /// </returns>
        T INodeDataProvider.CopyValues<T>(T from, T to, bool overwrite)
        {
            if (this.nodeData.TryGetValue(from, out Dictionary<string, object> fromValues))
            {
                if (!this.nodeData.TryGetValue(to, out Dictionary<string, object> targetValues))
                {
                    this.nodeData[to] = targetValues = new Dictionary<string, object>();
                }

                foreach (var nodeValue in fromValues)
                {
                    if (overwrite || targetValues.ContainsKey(nodeValue.Key))
                    {
                        targetValues[nodeValue.Key] = nodeValue.Value;
                    }
                }
            }

            return to;
        }

        /// <summary>
        /// Gets the data for the node.
        /// </summary>
        /// <param name="node">
        /// The node to get the data for.
        /// </param>
        /// <param name="data">
        /// The name of the data to get (e.g. 'Type' or 'Scope').
        /// </param>
        /// <typeparam name="T">
        /// The type of the data to get.
        /// </typeparam>
        /// <returns>
        /// The data for the node.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no data is found.
        /// </exception>
        T INodeDataProvider.Get<T>(Node node, string data)
        {
            if (((INodeDataProvider)this).TryGet(node, data, out T result))
            {
                return result;
            }

            throw new NodeException(node, $"Could not find data '{data}' for node '{node}'.");
        }

        /// <summary>
        /// Sets the data for the specified node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="data">
        /// The name of the data to set (e.g. 'Type' or 'Scope').
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        /// <typeparam name="T">
        /// The type of the value to set.
        /// </typeparam>
        void INodeDataProvider.Set<T>(Node node, string data, T value)
        {
            if (!this.nodeData.TryGetValue(node, out Dictionary<string, object> nodeValues))
            {
                nodeValues = this.nodeData[node] = new Dictionary<string, object>();
            }

            nodeValues[data] = value;
        }

        /// <summary>
        /// Tries to get the data for the node.
        /// </summary>
        /// <param name="node">
        /// The node to get the data for.
        /// </param>
        /// <param name="data">
        /// The name of the data to get (e.g. 'Type' or 'Scope').
        /// </param>
        /// <param name="value">
        /// The data for the node, if data exists.
        /// </param>
        /// <typeparam name="T">
        /// The type of the data to get.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the data was found, <c>false</c> otherwise.
        /// </returns>
        bool INodeDataProvider.TryGet<T>([CanBeNull] Node node, string data, out T value)
        {
            if (node != null && this.nodeData.TryGetValue(node, out Dictionary<string, object> nodeValues) && nodeValues.TryGetValue(data, out object objectValue))
            {
                value = (T)objectValue;
                return true;
            }

            value = default(T);

            return false;
        }
    }
}