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
    using System.Linq;

    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The serializable data source descriptor.
    /// </summary>
    internal class SerializableDataSourceDescriptor : IDataSourceDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDataSourceDescriptor"/> class.
        /// </summary>
        public SerializableDataSourceDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDataSourceDescriptor"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public SerializableDataSourceDescriptor([NotNull] IDataSourceDescriptor source)
        {
            this.Alias = source.Alias;
            this.AllowsAnyColumnName = source.AllowsAnyColumnName;
            this.Columns = source.Columns.Select(c => new SerializableColumnDescriptor(c)).ToArray();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data source allows any column name, or only the columns returned by
        ///     <see cref="IDataSourceDescriptor.Columns"/>.
        /// </summary>
        public bool AllowsAnyColumnName { get; set; }

        /// <summary>
        /// Gets or sets the columns for this data source.
        /// </summary>
        public SerializableColumnDescriptor[] Columns { get; set; }

        /// <summary>
        /// Gets gets or sets the columns for this data source.
        /// </summary>
        [NotNull]
        IReadOnlyCollection<IColumnDescriptor> IDataSourceDescriptor.Columns => this.Columns ?? new IColumnDescriptor[0];

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
            var other = obj as SerializableDataSourceDescriptor;

            return other != null &&
                   this.Alias == other.Alias &&
                   this.AllowsAnyColumnName == other.AllowsAnyColumnName &&
                   EnumerableComparer.Equals(this.Columns, other.Columns);
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
                var hashCode = this.AllowsAnyColumnName.GetHashCode();
                hashCode = (hashCode * 397) ^ EnumerableComparer.GetHashCode(this.Columns);
                hashCode = (hashCode * 397) ^ (this.Alias?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}