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

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.AsyncEnumerables.Policies;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    /// <summary>
    /// The no data source.
    /// </summary>
    internal class NoDataSource : DataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoDataSource"/> class.
        /// </summary>
        internal NoDataSource()
            : base(new HashSet<string>())
        {
        }

        /// <summary>
        /// Gets the rows of this datasource that conform to the query..
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{Row}"/> containing all returned rows.
        /// </returns>
        internal override IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, IMultiPartQuery query)
        {
            var builder = new RowBuilder();

            return context.CreateAsyncEnumerable(new[] { builder.CreateRow(1, Enumerable.Empty<KeyValuePair<string, object>>()) });
        }

        /// <summary>
        /// Gets the descriptors for this data source.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>
        /// All data sources inside this data source.
        /// </returns>
        protected internal override Task<IEnumerable<IDataSourceDescriptor>> GetDataSourceDescriptorsAsync(IExecutionContext context)
        {
            return Task.FromResult<IEnumerable<IDataSourceDescriptor>>(new[] { Descriptor.ForDataSource(string.Empty, Enumerable.Empty<IColumnDescriptor>()) });
        }
    }
}