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

namespace ConnectQl.Intellisense.Protocol
{
    using ConnectQl.Interfaces;

    /// <summary>
    /// The serializable data source range.
    /// </summary>
    internal class SerializableDataSourceDescriptorRange : IDataSourceDescriptorRange
    {
        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public SerializableDataSourceDescriptor DataSource { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        IDataSourceDescriptor IDataSourceDescriptorRange.DataSource => this.DataSource;

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
            var other = obj as SerializableDataSourceDescriptorRange;

            return other != null &&
                   this.Start == other.Start &&
                   this.End == other.End &&
                   object.Equals(this.DataSource, other.DataSource);
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
                var hashCode = this.DataSource?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ this.Start;
                hashCode = (hashCode * 397) ^ this.End;
                return hashCode;
            }
        }
    }
}