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
    using System.Diagnostics;
    using System.Linq;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// A serializable version of the <see cref="IAutoCompletions"/> interface.
    /// </summary>
    /// <seealso cref="IAutoCompletions" />
    [DebuggerDisplay("{Type,nq}{LiteralsDisplay,nq}")]
    internal class SerializableAutoCompletions : IAutoCompletions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableAutoCompletions"/> class.
        /// </summary>
        /// <param name="completions">The completions.</param>
        public SerializableAutoCompletions([NotNull] IAutoCompletions completions)
        {
            this.Type = completions.Type;
            this.Literals = completions.Literals?.ToArray();
        }

        /// <summary>
        /// Gets or sets the type of auto completions that are possible.
        /// </summary>
        public AutoCompleteType Type { get; set; }

        /// <summary>
        /// Gets or sets the literal options that are available. This field is <c>null</c> if <see cref="P:ConnectQl.Interfaces.IAutoCompletions.Type" /> does not have
        /// the <see cref="F:ConnectQl.Intellisense.AutoCompleteType.Literal" /> flag.
        /// </summary>
        public string[] Literals { get; set; }

        /// <summary>
        /// Gets the literal options that are available. This field is <c>null</c> if <see cref="P:ConnectQl.Interfaces.IAutoCompletions.Type" /> does not have
        /// the <see cref="F:ConnectQl.Intellisense.AutoCompleteType.Literal" /> flag.
        /// </summary>
        IReadOnlyList<string> IAutoCompletions.Literals => this.Literals;

        /// <summary>
        /// Gets a value used to display the literals in debug mode.
        /// </summary>
        [NotNull]
        private string LiteralsDisplay => this.Literals == null ? string.Empty : $" ({string.Join(", ", this.Literals)})";

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as SerializableAutoCompletions;

            return other != null &&
                this.Type == other.Type &&
                EnumerableComparer.Equals(this.Literals, other.Literals);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Type.GetHashCode() * 397) ^ EnumerableComparer.GetHashCode(this.Literals);
            }
        }
    }
}