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

namespace ConnectQl.DataSources.Joins
{
    using System.Linq.Expressions;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The sequential left join source.
    /// </summary>
    internal class SequentialJoinSource : JoinSourceBase
    {
        /// <summary>
        /// Indicates whether the join is an inner join or an outer join.
        /// </summary>
        private readonly bool isInnerJoin;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialJoinSource"/> class.
        /// </summary>
        /// <param name="left">
        /// The first.
        /// </param>
        /// <param name="right">
        /// The second.
        /// </param>
        /// <param name="isInnerJoin">
        /// <c>false</c> if this is a left join, <c>true</c> if this is an inner join.
        /// </param>
        public SequentialJoinSource(DataSource left, DataSource right, bool isInnerJoin)
            : base(left, right)
        {
            this.isInnerJoin = isInnerJoin;
        }

        /// <summary>
        /// Splits the query into queries for the left and right side of the join, a filter and the ORDER BY expressions.
        ///     Removes filters from the left and right side, so all records will be retrieved. Filters the data afterwards.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="query">
        /// The query to split.
        /// </param>
        /// <returns>
        /// The <see cref="JoinSourceBase.JoinQuery"/>.
        /// </returns>
        [NotNull]
        protected override JoinQuery CreateJoinQuery(IExecutionContext context, [NotNull] IMultiPartQuery query)
        {
            var resultFilter = query.GetFilter(context);

            var leftQuery = new MultiPartQuery
                                {
                                    Fields = query.GetUsedFields(this.Left, resultFilter),
                                };
            var rightQuery = new MultiPartQuery
                                 {
                                     Fields = query.GetUsedFields(this.Right, resultFilter),
                                 };

            return new JoinQuery(leftQuery, rightQuery, resultFilter, query.OrderByExpressions);
        }

        /// <summary>
        /// Retrieves the data from the source as an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="query">
        /// The query expression.
        /// </param>
        /// <returns>
        /// A task returning the rows.
        /// </returns>
        protected override IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, [NotNull] JoinQuery query)
        {
            var rowBuilder = new RowBuilder();
            var leftRows = this.Left.GetRows(context, query.LeftQuery);
            var rightRows = this.Right.GetRows(context, query.RightQuery);

            return
                this.isInnerJoin
                    ? leftRows.Zip(rightRows, rowBuilder.CombineRows)
                    : leftRows.ZipAll(rightRows, rowBuilder.CombineRows)
                        .Where(query.ResultFilter?.GetRowFilter())
                        .OrderBy(query.OrderBy);
        }
    }
}