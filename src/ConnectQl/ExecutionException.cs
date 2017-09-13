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

namespace ConnectQl
{
    using System;
    using System.Collections.Generic;
    using ConnectQl.Interfaces;

    /// <summary>
    /// Thrown when execution fails.
    /// </summary>
    public class ExecutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionException"/> class.
        /// </summary>
        /// <param name="messages">Messages, warnings and errors that were found in the file.</param>
        public ExecutionException(IReadOnlyCollection<IMessage> messages)
            : this(null, messages)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionException"/> class.
        /// </summary>
        /// <param name="message">The text message of the exception.</param>
        /// <param name="messages">
        /// Messages, warnings and errors that were found in the file.
        /// </param>
        public ExecutionException(string message, IReadOnlyCollection<IMessage> messages)
            : base($"{message}{string.Join("\n", messages)}")
        {
            this.Messages = messages;
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public IReadOnlyCollection<IMessage> Messages { get; }
    }
}