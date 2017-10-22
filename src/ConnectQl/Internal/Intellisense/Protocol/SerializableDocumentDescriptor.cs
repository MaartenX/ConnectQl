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
    using System.Collections.Generic;

    using ConnectQl.Interfaces;

    /// <summary>
    /// The serializable document.
    /// </summary>
    internal class SerializableDocumentDescriptor : IDocumentDescriptor
    {
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the document version number.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the functions.
        /// </summary>
        public SerializableFunctionDescriptor[] Functions { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public SerializableMessage[] Messages { get; set; }

        /// <summary>
        /// Gets or sets the plugins.
        /// </summary>
        public List<string> Plugins { get; set; }

        /// <summary>
        /// Gets or sets the tokens.
        /// </summary>
        public SerializableDataSourceDescriptorRange[] Sources { get; set; }

        /// <summary>
        /// Gets or sets the tokens.
        /// </summary>
        public SerializableToken[] Tokens { get; set; }

        /// <summary>
        /// Gets or sets the tokens.
        /// </summary>
        public SerializableVariableDescriptorRange[] Variables { get; set; }

        /// <summary>
        /// Gets the functions.
        /// </summary>
        IReadOnlyList<IFunctionDescriptor> IDocumentDescriptor.Functions => this.Functions;

        /// <summary>
        /// Gets the messages.
        /// </summary>
        IReadOnlyList<IMessage> IDocumentDescriptor.Messages => this.Messages;

        /// <summary>
        /// Gets the sources.
        /// </summary>
        IReadOnlyList<IDataSourceDescriptorRange> IDocumentDescriptor.Sources => this.Sources;

        /// <summary>
        /// Gets the tokens.
        /// </summary>
        IReadOnlyList<IClassifiedToken> IDocumentDescriptor.Tokens => this.Tokens;

        /// <summary>
        /// Gets the variables.
        /// </summary>
        IReadOnlyList<IVariableDescriptorRange> IDocumentDescriptor.Variables => this.Variables;

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        IReadOnlyList<string> IDocumentDescriptor.Plugins => this.Plugins;

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
            var other = obj as SerializableDocumentDescriptor;

            return other != null &&
                EnumerableComparer.Equals(this.Functions, other.Functions) &&
                EnumerableComparer.Equals(this.Messages, other.Messages) &&
                EnumerableComparer.Equals(this.Plugins, other.Plugins) &&
                EnumerableComparer.Equals(this.Sources, other.Sources) &&
                EnumerableComparer.Equals(this.Tokens, other.Tokens) &&
                EnumerableComparer.Equals(this.Variables, other.Variables);
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
                var hashCode = EnumerableComparer.GetHashCode(this.Functions);
                hashCode = (hashCode * 397) ^ EnumerableComparer.GetHashCode(this.Messages);
                hashCode = (hashCode * 397) ^ EnumerableComparer.GetHashCode(this.Plugins);
                hashCode = (hashCode * 397) ^ EnumerableComparer.GetHashCode(this.Sources);
                hashCode = (hashCode * 397) ^ EnumerableComparer.GetHashCode(this.Tokens);
                hashCode = (hashCode * 397) ^ EnumerableComparer.GetHashCode(this.Variables);
                return hashCode;
            }
        }
    }
}