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
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    /// <summary>
    /// The result message.
    /// </summary>
    internal sealed class Message : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="text">
        /// The message text.
        /// </param>
        /// <param name="file">
        /// The file.
        /// </param>
        public Message(Position start, Position end, ResultMessageType type, string text, string file)
        {
            this.Start = start;
            this.End = end;
            this.Type = type;
            this.Text = text;
            this.File = file;
        }

        /// <summary>
        /// Gets the end position.
        /// </summary>
        public Position End { get; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Gets the start position.
        /// </summary>
        public Position Start { get; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public ResultMessageType Type { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Start.Column == this.End.Column && this.Start.Line == this.End.Line
                       ? $"{this.File}({this.Start.Line}, {this.Start.Column}): {this.Type}: {this.Text}"
                       : $"{this.File}({this.Start.Line}, {this.Start.Column}, {this.End.Line}, {this.End.Column}): {this.Type}: {this.Text}";
        }
    }
}