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
    /// The serializable argument descriptor.
    /// </summary>
    internal class SerializableArgumentDescriptor : IArgumentDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableArgumentDescriptor"/> class.
        /// </summary>
        public SerializableArgumentDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableArgumentDescriptor"/> class.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        public SerializableArgumentDescriptor(IArgumentDescriptor argument)
        {
            this.Name = argument.Name;
            this.Description = argument.Description;
            this.Type = new SerializableTypeDescriptor(argument.Type);
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
        /// Gets the argument type.
        /// </summary>
        ITypeDescriptor IArgumentDescriptor.Type => this.Type;

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            var obj = other as SerializableArgumentDescriptor;

            return !object.ReferenceEquals(null, obj) &&
                   string.Equals(this.Name, obj.Name) &&
                   string.Equals(this.Description, obj.Description) &&
                   object.Equals(this.Type, obj.Type);
        }

        /// <summary>
        /// Returns the hash code for this string.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
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