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

    /// <summary>
    /// The serializable variable descriptor range.
    /// </summary>
    internal class SerializableVariableDescriptorRange : IVariableDescriptorRange
    {
        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the variable.
        /// </summary>
        public SerializableVariableDescriptor Variable { get; set; }

        /// <summary>
        /// Gets the variable.
        /// </summary>
        IVariableDescriptor IVariableDescriptorRange.Variable => this.Variable;

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
            var other = obj as SerializableVariableDescriptorRange;

            return other != null &&
                   this.End == other.End &&
                   this.Start == other.Start &&
                   object.Equals(this.Variable, other.Variable);
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
                var hashCode = this.End;
                hashCode = (hashCode * 397) ^ this.Start;
                hashCode = (hashCode * 397) ^ (this.Variable?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}