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

namespace ConnectQl.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal;

    /// <summary>
    /// The descriptor.
    /// </summary>
    public static class Descriptor
    {
        /// <summary>
        /// Creates a dynamic data source; no columns, but allows any column name.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="IDataSourceDescriptor"/>.
        /// </returns>
        public static IDataSourceDescriptor DynamicDataSource(string alias) => new DataSourceDescriptor(alias, Enumerable.Empty<IColumnDescriptor>(), true);

        /// <summary>
        /// Creates a descriptor for a column.
        /// </summary>
        /// <param name="name">
        /// The name of the column.
        /// </param>
        /// <param name="type">
        /// The type of the column.
        /// </param>
        /// <param name="description">
        /// Optional. The description of the column.
        /// </param>
        /// <returns>
        /// The <see cref="IColumnDescriptor"/>.
        /// </returns>
        public static IColumnDescriptor ForColumn(string name, Type type, string description = null)
        {
            return new ColumnDescriptor(type, name, description);
        }

        /// <summary>
        /// The for data source.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="columns">
        /// The columns.
        /// </param>
        /// <param name="allowsAnyColumnName">
        /// The allows any column name.
        /// </param>
        /// <returns>
        /// The <see cref="IDataSourceDescriptor"/>.
        /// </returns>
        public static IDataSourceDescriptor ForDataSource(string alias, IEnumerable<IColumnDescriptor> columns, bool allowsAnyColumnName = false)
        {
            return new DataSourceDescriptor(alias, columns, allowsAnyColumnName);
        }
    }
}