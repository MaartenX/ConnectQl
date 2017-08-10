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
    using System.ComponentModel;
    using System.Diagnostics;

    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;

    /// <summary>
    /// The serializable token.
    /// </summary>
    [DebuggerDisplay("{Start,nq}-{End,nq} {Classification,nq} {Value}")]
    internal class SerializableToken : IClassifiedToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableToken"/> class.
        /// </summary>
        public SerializableToken()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableToken"/> class.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        public SerializableToken(IClassifiedToken token)
        {
            this.Start = token.Start;
            this.Classification = token.Classification;
            this.Value = token.Value;
            this.Scope = token.Scope;
        }

        /// <summary>
        /// Gets or sets the classification.
        /// </summary>
        public Classification Classification { get; set; }

        /// <summary>
        /// Gets gets or sets the end.
        /// </summary>
        public int End => this.Start + this.Length;

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length => this.Value.Length;

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public ClassificationScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [DefaultValue("")]
        public string Value { get; set; }

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
            var other = obj as SerializableToken;

            return other != null &&
                   this.Scope == other.Scope &&
                   this.Classification == other.Classification &&
                   this.Start == other.Start &&
                   string.Equals(this.Value, other.Value);
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
                var hashCode = (int)this.Classification;
                hashCode = (hashCode * 397) ^ (int)this.Scope;
                hashCode = (hashCode * 397) ^ this.Start;
                hashCode = (hashCode * 397) ^ (this.Value?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}