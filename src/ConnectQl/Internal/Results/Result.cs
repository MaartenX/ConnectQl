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

namespace ConnectQl.Internal.Results
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Results;

    /// <summary>
    /// The result.
    /// </summary>
    internal class Result : IResult, IResultBuilder
    {
        /// <summary>
        /// The file.
        /// </summary>
        private readonly string file;

        /// <summary>
        /// The messages.
        /// </summary>
        private readonly IList<Message> messages = new List<Message>();

        /// <summary>
        /// The node data.
        /// </summary>
        private readonly Dictionary<Node, Dictionary<string, object>> nodeData = new Dictionary<Node, Dictionary<string, object>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        public Result(string file)
        {
            this.file = file;
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public ReadOnlyCollection<Message> Errors => new ReadOnlyCollection<Message>(this.messages.Where(m => m.Type == ResultMessageType.Error).ToList());

        /// <summary>
        /// Gets the information messages.
        /// </summary>
        public ReadOnlyCollection<Message> InformationMessages => new ReadOnlyCollection<Message>(this.messages.Where(m => m.Type == ResultMessageType.Information).ToList());

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        public ReadOnlyCollection<Message> Warnings => new ReadOnlyCollection<Message>(this.messages.Where(m => m.Type == ResultMessageType.Warning).ToList());

        /// <summary>
        /// Gets the node data for the specified node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the result.
        /// </typeparam>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="type">
        /// The type of the data to get.
        /// </param>
        /// <returns>
        /// The data, or <c>default{T}</c> if no data is available.
        /// </returns>
        public T GetNodeData<T>(Node node, string type)
        {
            return this.nodeData.TryGetValue(node, out Dictionary<string, object> existingData) && existingData.TryGetValue(type, out object data) && data is T ? (T)data : default(T);
        }

        /// <summary>
        /// The add error.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void IResultBuilder.AddError(Position start, Position end, string message)
        {
            this.messages.Add(new Message(start, end, ResultMessageType.Error, message, this.file));
        }

        /// <summary>
        /// The add information.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void IResultBuilder.AddInformation(Position start, Position end, string message)
        {
            this.messages.Add(new Message(start, end, ResultMessageType.Information, message, this.file));
        }

        /// <summary>
        /// Adds data to the specified node.
        /// </summary>
        /// <param name="node">
        /// The node to add data to.
        /// </param>
        /// <param name="type">
        /// The type of the data.
        /// </param>
        /// <param name="data">
        /// The data to add to the node.
        /// </param>
        /// <typeparam name="T">
        /// The type of the data to write.
        /// </typeparam>
        /// <returns>
        /// The data that was added.
        /// </returns>
        T IResultBuilder.AddNodeData<T>(Node node, string type, T data)
        {
            if (!this.nodeData.TryGetValue(node, out Dictionary<string, object> existingData))
            {
                this.nodeData[node] = existingData = new Dictionary<string, object>();
            }

            existingData[type] = data;

            return data;
        }

        /// <summary>
        /// The add warning.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void IResultBuilder.AddWarning(Position start, Position end, string message)
        {
            this.messages.Add(new Message(start, end, ResultMessageType.Warning, message, this.file));
        }

        /// <summary>
        /// Removes the data from the node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="type">
        /// The type of the data to remove.
        /// </param>
        /// <returns>
        /// <c>true</c> if the node was removed, <c>false</c> otherwise.
        /// </returns>
        bool IResultBuilder.RemoveNodeData(Node node, string type)
        {
            return this.nodeData.TryGetValue(node, out Dictionary<string, object> existingData) && existingData.Remove(type);
        }

        /// <summary>
        /// Transfers the data from one node to another.
        /// </summary>
        /// <param name="from">
        /// The node to transfer all data from.
        /// </param>
        /// <param name="to">
        /// The node to transfer all data to.
        /// </param>
        /// <param name="overwrite">
        /// When <c>true</c>, overwrites duplicate values in <paramref name="to"/>, when <c>false</c>, keeps the original
        ///     values.
        /// </param>
        /// <returns>
        /// <c>true</c> if data was moved, <c>false</c> otherwise.
        /// </returns>
        bool IResultBuilder.TransferData(Node from, Node to, bool overwrite)
        {
            if (!this.nodeData.TryGetValue(from, out Dictionary<string, object> dataFrom))
            {
                return false;
            }

            if (!this.nodeData.TryGetValue(to, out Dictionary<string, object> dataTo))
            {
                this.nodeData[to] = dataTo = new Dictionary<string, object>();
            }

            var result = false;

            foreach (var keyValuePair in dataFrom)
            {
                if (!overwrite && dataTo.ContainsKey(keyValuePair.Key))
                {
                    continue;
                }

                dataTo[keyValuePair.Key] = keyValuePair.Value;
                result = true;
            }

            this.nodeData.Remove(from);

            return result;
        }
    }
}