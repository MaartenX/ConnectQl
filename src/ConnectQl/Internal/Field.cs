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

namespace ConnectQl.Internal
{
    using System;

    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The field.
    /// </summary>
    internal class Field : IField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="sourceAlias">
        /// The source alias.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        public Field(string sourceAlias, string fieldName)
        {
            this.SourceAlias = sourceAlias;
            this.FieldName = fieldName;
        }

        /// <summary>
        /// Gets the field name.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// Gets the source alias.
        /// </summary>
        public string SourceAlias { get; }

        /// <summary>
        /// Compares this object to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">
        /// The object to compare to.
        /// </param>
        /// <returns>
        /// <c>true</c> if the objects are equals, <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as Field;
            return other != null && string.Equals(this.SourceAlias, other.SourceAlias, StringComparison.OrdinalIgnoreCase) && string.Equals(this.FieldName, other.FieldName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the hash code for this field.
        /// </summary>
        /// <returns>
        /// The hash code.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.SourceAlias != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.SourceAlias) : 0) * 397) ^ (this.FieldName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.FieldName) : 0);
            }
        }

        /// <summary>
        /// Converts the field to a string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NotNull]
        public override string ToString() => $"{this.SourceAlias}.{this.FieldName}";
    }
}