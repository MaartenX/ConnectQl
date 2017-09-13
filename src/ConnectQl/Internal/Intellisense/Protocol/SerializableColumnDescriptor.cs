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
    /// The serializable column descriptor.
    /// </summary>
    internal class SerializableColumnDescriptor : IColumnDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableColumnDescriptor"/> class.
        /// </summary>
        public SerializableColumnDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableColumnDescriptor"/> class.
        /// </summary>
        /// <param name="columnDescriptor">
        /// The column descriptor.
        /// </param>
        public SerializableColumnDescriptor(IColumnDescriptor columnDescriptor)
        {
            this.Name = columnDescriptor.Name;
            this.Description = columnDescriptor.Description;
            this.Type = new SerializableTypeDescriptor(columnDescriptor.Type);
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public SerializableTypeDescriptor Type { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        ITypeDescriptor IColumnDescriptor.Type => this.Type;

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as SerializableArgumentDescriptor;

            return !object.ReferenceEquals(null, other) &&
                   string.Equals(this.Name, other.Name) &&
                   string.Equals(this.Description, other.Description) &&
                   object.Equals(this.Type, other.Type);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Name?.GetHashCode() ?? 0;

                hashCode = (hashCode * 397) ^ (this.Description?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Type?.GetHashCode() ?? 0);

                return hashCode;
            }
        }
    }
}