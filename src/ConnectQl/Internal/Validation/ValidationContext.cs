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

namespace ConnectQl.Internal.Validation
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    /// <summary>
    /// The validation context.
    /// </summary>
    internal class ValidationContext
    {
        /// <summary>
        /// The file.
        /// </summary>
        private readonly string filename;

        /// <summary>
        /// The errors.
        /// </summary>
        private readonly IList<Message> messages = new List<Message>();

        /// <summary>
        /// The node data.
        /// </summary>
        private readonly Dictionary<Node, Dictionary<string, object>> nodeData = new Dictionary<Node, Dictionary<string, object>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public ValidationContext(string filename)
        {
            this.filename = filename;
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public IReadOnlyCollection<Message> Errors => new ReadOnlyCollection<Message>(this.messages.Where(m => m.Type == ResultMessageType.Error).ToArray());

        /// <summary>
        /// Gets the information messages.
        /// </summary>
        public IReadOnlyCollection<Message> InformationMessages => new ReadOnlyCollection<Message>(this.messages.Where(m => m.Type == ResultMessageType.Information).ToArray());

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        public IReadOnlyCollection<Message> Warnings => new ReadOnlyCollection<Message>(this.messages.Where(m => m.Type == ResultMessageType.Warning).ToArray());

        /// <summary>
        /// Adds an error to the node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public void AddError(Node node, string message)
        {
            this.messages.Add(new Message(null, null, ResultMessageType.Error, message, this.filename));
        }
    }
}