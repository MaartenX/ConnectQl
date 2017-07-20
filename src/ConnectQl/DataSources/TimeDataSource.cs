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

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Results;

    /// <summary>
    /// The time query source.
    /// </summary>
    public class TimeDataSource : IDataSource, IDescriptableDataSource
    {
        /// <summary>
        /// The future.
        /// </summary>
        private readonly TimeSpan future;

        /// <summary>
        /// The interval.
        /// </summary>
        private readonly TimeSpan interval;

        /// <summary>
        /// The offset.
        /// </summary>
        private readonly TimeOffset offset;

        /// <summary>
        /// The amount of time in the past.
        /// </summary>
        private readonly TimeSpan past;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeDataSource"/> class.
        /// </summary>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="past">
        /// The amount of time in the past.
        /// </param>
        /// <param name="future">
        /// The future.
        /// </param>
        /// <param name="interval">
        /// The interval.
        /// </param>
        public TimeDataSource(TimeOffset offset, TimeSpan past, TimeSpan future, TimeSpan interval)
        {
            this.offset = offset;
            this.past = past;
            this.future = future;
            this.interval = interval;
        }

        /// <summary>
        /// The time offset.
        /// </summary>
        public enum TimeOffset
        {
            /// <summary>
            /// The today.
            /// </summary>
            Midnight,

            /// <summary>
            /// The now.
            /// </summary>
            Now,

            /// <summary>
            /// The UTC midnight.
            /// </summary>
            UtcMidnight,

            /// <summary>
            /// The UTC now.
            /// </summary>
            UtcNow,
        }

        /// <summary>
        /// Gets the columns for this data source.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>
        /// The columns in this data source.
        /// </returns>
        public Task<IEnumerable<IColumnDescriptor>> GetColumnsAsync(IExecutionContext context)
        {
            return Task.FromResult(Enumerable.Repeat<IColumnDescriptor>(new ColumnDescriptor(typeof(DateTime), "Time"), 1));
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
            return Task.FromResult(Descriptor.ForDataSource(sourceAlias, Enumerable.Repeat<IColumnDescriptor>(new ColumnDescriptor(typeof(DateTime), "Time"), 1)));
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
            var filter = query.GetFilter(context).GetRowFilter();

            DateTime start;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (this.offset)
            {
                case TimeOffset.Midnight:
                    start = DateTime.Today - this.past;
                    break;
                case TimeOffset.UtcNow:
                    start = DateTime.UtcNow - this.past;
                    break;
                case TimeOffset.UtcMidnight:
                    start = DateTime.UtcNow.Date - this.past;
                    break;
                default:
                    start = DateTime.Now - this.past;
                    break;
            }

            var num = (int)(((long)this.future.TotalMilliseconds + (long)this.past.TotalMilliseconds) / (long)this.interval.TotalMilliseconds);

            return context.ToAsyncEnumerable(Enumerable
                .Range(0, num)
                .Select(i => start + TimeSpan.FromMilliseconds(i * this.interval.TotalMilliseconds))
                .Select(i => rowBuilder.CreateRow(i.Ticks, new[] { new KeyValuePair<string, object>("Time", i), }))
                .Where(filter)).OrderBy(query.OrderByExpressions);
        }
    }
}