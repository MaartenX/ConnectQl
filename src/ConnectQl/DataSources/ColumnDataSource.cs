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

namespace ConnectQl.DataSources
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    /// <summary>
    /// The column data source.
    /// </summary>
    public static class ColumnDataSource
    {
        /// <summary>
        /// Creates a data source that returns the values as columns with the value <c>true</c>.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="IDataSource"/>.
        /// </returns>
        public static IDataSource Create(string columnName)
        {
            return new ColumnDataSourceImplementation(columnName);
        }

        /// <summary>
        /// Creates a data source that returns the values as columns with the value <c>true</c>.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <param name="columnValue">
        /// The column value.
        /// </param>
        /// <returns>
        /// The <see cref="IDataSource"/>.
        /// </returns>
        public static IDataSource Create(string columnName, object columnValue)
        {
            return new ColumnDataSourceImplementation(columnName, columnValue);
        }

        /// <summary>
        /// The column data source implementation.
        /// </summary>
        private class ColumnDataSourceImplementation : IDataSource, IDescriptableDataSource
        {
            /// <summary>
            /// The value.
            /// </summary>
            private readonly string columnName;

            /// <summary>
            /// The column value.
            /// </summary>
            private readonly object columnValue;

            /// <summary>
            /// Indicates whether the column value was set.
            /// </summary>
            private readonly bool columnValueSet;

            /// <summary>
            /// Initializes a new instance of the <see cref="ColumnDataSourceImplementation"/> class.
            /// </summary>
            /// <param name="columnName">
            /// The column name.
            /// </param>
            public ColumnDataSourceImplementation(string columnName)
            {
                this.columnName = columnName;
                this.columnValueSet = false;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ColumnDataSourceImplementation"/> class.
            /// </summary>
            /// <param name="columnName">
            /// The column name.
            /// </param>
            /// <param name="columnValue">
            /// The column value or <c>null</c> to insert <c>true</c> or <c>false</c>.
            /// </param>
            public ColumnDataSourceImplementation(string columnName, object columnValue)
            {
                this.columnName = columnName;
                this.columnValue = columnValue;
                this.columnValueSet = true;
            }

            /// <summary>
            /// Gets the descriptor for this data source.
            /// </summary>
            /// <param name="sourceAlias">
            /// The source alias.
            /// </param>
            /// <param name="context">
            /// The execution context.
            /// </param>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            public Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(string sourceAlias, IExecutionContext context)
            {
                var columns = new[]
                                  {
                                      Descriptor.ForColumn(this.columnName, typeof(bool)),
                                  };

                return Task.FromResult(Descriptor.ForDataSource(sourceAlias, columns));
            }

            /// <summary>
            /// Retrieves the data from the source as an <see cref="IAsyncEnumerable{T}"/>.
            /// </summary>
            /// <param name="context">
            /// The context.
            /// </param>
            /// <param name="rowBuilder">
            /// The row builder.
            /// </param>
            /// <param name="query">
            /// The query expression. Can be <c>null</c>.
            /// </param>
            /// <returns>
            /// A task returning the data set.
            /// </returns>
            public IAsyncEnumerable<Row> GetRows(IExecutionContext context, IRowBuilder rowBuilder, IQuery query)
            {
                var row = rowBuilder.CreateRow(0, new KeyValuePair<string, object>(this.columnName, this.columnValueSet ? this.columnValue : true));

                return context.CreateAsyncEnumerable(Enumerable.Repeat(row, 1));
            }
        }
    }
}