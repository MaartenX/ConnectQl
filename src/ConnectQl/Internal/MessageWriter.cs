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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    /// <summary>
    /// The message writer.
    /// </summary>
    internal class MessageWriter : IMessageWriter, IEnumerable<Message>
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
        /// Initializes a new instance of the <see cref="MessageWriter"/> class.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        public MessageWriter(string file)
        {
            this.file = file;
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public IEnumerable<Message> Errors => this.messages.Where(msg => msg.Type == ResultMessageType.Error).OrderBy(msg => msg.Start.Line).ThenBy(msg => msg.Start.Column);

        /// <summary>
        /// Gets a value indicating whether the message writer has any errors.
        /// </summary>
        public bool HasErrors => this.messages.Any(msg => msg.Type == ResultMessageType.Error);

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection. </returns>
        public int Count => this.messages.Count;

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<Message> GetEnumerator()
        {
            return this.messages.OrderBy(msg => msg.Start.Line).ThenBy(msg => msg.Start.Column).GetEnumerator();
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
        void IMessageWriter.AddError(Position start, Position end, string message)
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
        void IMessageWriter.AddInformation(Position start, Position end, string message)
        {
            this.messages.Add(new Message(start, end, ResultMessageType.Information, message, this.file));
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
        void IMessageWriter.AddWarning(Position start, Position end, string message)
        {
            this.messages.Add(new Message(start, end, ResultMessageType.Warning, message, this.file));
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}