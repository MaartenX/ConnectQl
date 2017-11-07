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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.AsyncEnumerables.Policies;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// Data source from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableDataSource
    {
        /// <summary>
        /// Creates the enumerable data source from the enumerable.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="enumerable">
        /// The <see cref="IEnumerable{T}"/> to create the data source from.
        /// </param>
        /// <returns>
        /// The <see cref="IDataSource"/>.
        /// </returns>
        [NotNull]
        public static IDataSource Create<T>(IEnumerable<T> enumerable)
        {
            return new EnumerableDataSourceImplementation<T>(enumerable, EnumerableDataSource.Item);
        }

        /// <summary>
        /// Creates the enumerable data source from the enumerable.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="enumerable">
        /// The <see cref="IEnumerable{T}"/> to create the data source from.
        /// </param>
        /// <param name="rowGenerator">
        /// The row generator.
        /// </param>
        /// <returns>
        /// The <see cref="IDataSource"/>.
        /// </returns>
        [NotNull]
        public static IDataSource Create<T>(IEnumerable<T> enumerable, Func<T, IEnumerable<KeyValuePair<string, object>>> rowGenerator)
        {
            return new EnumerableDataSourceImplementation<T>(enumerable, rowGenerator);
        }

        /// <summary>
        /// Creates an enumerable of key/value pairs containing the item.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <param name="value">
        /// The value of the item.
        /// </param>
        /// <returns>
        /// A <see cref="KeyValuePair{TKey,TValue}"/> containing 'Item' as the Key, and the <paramref name="value"/> as
        ///     the Value.
        /// </returns>
        private static IEnumerable<KeyValuePair<string, object>> Item<T>(T value)
        {
            yield return new KeyValuePair<string, object>("Item", value);
        }

        /// <summary>
        /// Implementation of the <see cref="EnumerableDataSource"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        private class EnumerableDataSourceImplementation<T> : IDataSource, IDescriptableDataSource
        {
            /// <summary>
            /// The enumerable.
            /// </summary>
            private readonly IEnumerable<T> enumerable;

            /// <summary>
            /// The row generator.
            /// </summary>
            private readonly Func<T, IEnumerable<KeyValuePair<string, object>>> rowGenerator;

            /// <summary>
            /// Initializes a new instance of the <see cref="EnumerableDataSourceImplementation{T}"/> class.
            /// </summary>
            /// <param name="enumerable">
            /// The enumerable.
            /// </param>
            /// <param name="rowGenerator">
            /// The row Generator.
            /// </param>
            public EnumerableDataSourceImplementation(IEnumerable<T> enumerable, Func<T, IEnumerable<KeyValuePair<string, object>>> rowGenerator)
            {
                this.enumerable = enumerable;
                this.rowGenerator = rowGenerator;
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
                return Task.FromResult(Descriptor.ForDataSource(sourceAlias, Enumerable.Repeat<IColumnDescriptor>(new ColumnDescriptor(typeof(T), "Item"), 1)));
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
            public IAsyncEnumerable<Row> GetRows(IExecutionContext context, IRowBuilder rowBuilder, [NotNull] IQuery query)
            {
                var idx = 0L;

                return context
                    .CreateAsyncEnumerable(this.enumerable)
                    .Select(value => rowBuilder.CreateRow(idx++, this.rowGenerator(value)))
                    .Where(query.GetFilter(context).GetRowFilter())
                    .OrderBy(query.GetSortOrders(context))
                    .Take(query.Count);
            }
        }
    }
}