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

namespace ConnectQl.Internal.Intellisense.Protocol
{
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    /// <summary>
    /// The serializable message.
    /// </summary>
    internal class SerializableMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableMessage"/> class.
        /// </summary>
        public SerializableMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public SerializableMessage(Message message)
        {
            this.Start = message.Start;
            this.End = message.End;
            this.Type = message.Type;
            this.File = message.File;
            this.Text = message.Text;
        }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        public Position End { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public Position Start { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public ResultMessageType Type { get; set; }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as SerializableMessage;

            return other != null &&
                   this.Start == other.Start &&
                   this.End == other.End &&
                   string.Equals(this.File, other.File) &&
                   string.Equals(this.Text, other.Text) &&
                   this.Type == other.Type;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.End?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (this.File?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Start?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Text?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (int)this.Type;
                return hashCode;
            }
        }
    }
}