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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Comparers;
    using ConnectQl.Results;

    /// <summary>
    /// The temp data source.
    /// </summary>
    public class TempDataSource : IDataSource, IDataTarget, IDescriptableDataSource
    {
        /// <summary>
        /// The rows.
        /// </summary>
        private IAsyncReadOnlyCollection<Row> rows;

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
            return Task.FromResult(Descriptor.DynamicDataSource(sourceAlias));
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
            return
                this.rows?.Select(rowBuilder.Attach)
                    .Where(query.GetFilter(context).GetRowFilter())
                    .OrderBy(query.GetSortOrders(context))
                ?? context.CreateAsyncEnumerable(Enumerable.Empty<Row>());
        }

        /// <summary>
        /// Writes the rowsToWrite to the specified target.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rowsToWrite">
        /// The rowsToWrite.
        /// </param>
        /// <param name="upsert">
        /// True to also update records, false to insert.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<long> WriteRowsAsync(IExecutionContext context, IAsyncEnumerable<Row> rowsToWrite, bool upsert)
        {
            var builder = context.CreateBuilder<Row>();

            if (this.rows == null)
            {
                this.rows = await rowsToWrite.MaterializeAsync();

                return this.rows.Count;
            }

            var originalCount = this.rows.Count;

            if (upsert)
            {
                await builder.AddAsync(this.rows.Union(rowsToWrite, new RowUniqueIdComparer()));
            }
            else
            {
                await builder.AddAsync(this.rows).ConfigureAwait(false);
                await builder.AddAsync(rowsToWrite).ConfigureAwait(false);
            }

            this.rows = await builder.BuildAsync().ConfigureAwait(false);

            return this.rows.Count - originalCount;
        }
    }
}