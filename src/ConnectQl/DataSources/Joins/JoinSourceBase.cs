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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Comparers;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The join source base.
    /// </summary>
    internal abstract class JoinSourceBase : IMultiPartSource
    {
        /// <summary>
        /// A comparer that compares expressions by their string representation.
        /// </summary>
        protected static readonly IEqualityComparer<Expression> ExpressionComparer = new ExpressionComparer();

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinSourceBase"/> class.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        protected JoinSourceBase(DataSource left, DataSource right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Gets the left part of the join.
        /// </summary>
        protected DataSource Left { get; }

        /// <summary>
        /// Gets the right part of the join.
        /// </summary>
        protected DataSource Right { get; }

        /// <summary>
        /// Retrieves the data from the source as an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="multiPartQuery">
        /// The query expression. Can be <c>null</c>.
        /// </param>
        /// <returns>
        /// A task returning the data set.
        /// </returns>
        public IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, IMultiPartQuery multiPartQuery)
        {
            return this.GetRows(context, this.CreateJoinQuery(context, multiPartQuery));
        }

        /// <summary>
        /// Splits the query into queries for the left and right side of the join, a filter and the ORDER BY expressions.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="query">
        /// The query to split.
        /// </param>
        /// <returns>
        /// The <see cref="JoinQuery"/>.
        /// </returns>
        [NotNull]
        protected virtual JoinQuery CreateJoinQuery(IExecutionContext context, [NotNull] IMultiPartQuery query)
        {
            var filter = query.GetFilter(context);
            var leftFilter = filter.RemoveAllPartsThatAreNotInSource(this.Left);
            var rightFilter = filter.RemoveAllPartsThatAreNotInSource(this.Right);
            var resultFilter = filter.SplitByAndExpressions()
                .Except(leftFilter.SplitByAndExpressions(), JoinSourceBase.ExpressionComparer)
                .Except(rightFilter.SplitByAndExpressions(), JoinSourceBase.ExpressionComparer)
                .DefaultIfEmpty()
                .Aggregate(Expression.AndAlso);

            var leftQuery = new MultiPartQuery
                                {
                                    Fields = query.GetUsedFields(this.Left, resultFilter),
                                    FilterExpression = leftFilter,
                                };

            var rightQuery = new MultiPartQuery
                                 {
                                     Fields = query.GetUsedFields(this.Right, resultFilter),
                                     FilterExpression = leftFilter,
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
        protected abstract IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, JoinQuery query);

        /// <summary>
        /// The query parts.
        /// </summary>
        protected class JoinQuery
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="JoinQuery"/> class.
            /// </summary>
            /// <param name="leftQuery">
            /// The left query.
            /// </param>
            /// <param name="rightQuery">
            /// The right query.
            /// </param>
            /// <param name="resultFilter">
            /// The filter.
            /// </param>
            /// <param name="orderBy">
            /// The order by.
            /// </param>
            public JoinQuery(IMultiPartQuery leftQuery, IMultiPartQuery rightQuery, Expression resultFilter, [NotNull] IEnumerable<IOrderByExpression> orderBy)
            {
                this.LeftQuery = leftQuery;
                this.RightQuery = rightQuery;
                this.ResultFilter = resultFilter;
                this.OrderBy = orderBy.ToArray();
            }

            /// <summary>
            /// Gets the left query.
            /// </summary>
            public IMultiPartQuery LeftQuery { get; }

            /// <summary>
            /// Gets the order by.
            /// </summary>
            public IOrderByExpression[] OrderBy { get; }

            /// <summary>
            /// Gets the filter.
            /// </summary>
            public Expression ResultFilter { get; }

            /// <summary>
            /// Gets the right query.
            /// </summary>
            public IMultiPartQuery RightQuery { get; }
        }
    }
}