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

namespace ConnectQl.Internal.DataSources.Joins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Comparers;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Query;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// Base class for a filtered join.
    /// </summary>
    internal abstract class FilteredJoinSourceBase : IMultiPartSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredJoinSourceBase"/> class.
        /// </summary>
        /// <param name="left">
        /// The left data source.
        /// </param>
        /// <param name="right">
        /// The right data source.
        /// </param>
        /// <param name="filterExpression">
        /// The filter.
        /// </param>
        /// <param name="isInnerJoin">
        /// True if this is an inner join, false if it's a left join.
        /// </param>
        protected FilteredJoinSourceBase(DataSource left, DataSource right, Expression filterExpression, bool isInnerJoin)
        {
            this.Left = left;
            this.Right = right;
            this.FilterExpression = filterExpression;
            this.IsInnerJoin = isInnerJoin;
        }

        /// <summary>
        /// Gets the filter expression.
        /// </summary>
        protected Expression FilterExpression { get; }

        /// <summary>
        /// Gets a value indicating whether this is an inner join.
        /// </summary>
        protected bool IsInnerJoin { get; }

        /// <summary>
        /// Gets the left side of the join.
        /// </summary>
        protected DataSource Left { get; }

        /// <summary>
        /// Gets the right side of the join.
        /// </summary>
        protected DataSource Right { get; }

        /// <summary>
        /// Retrieves the data from the source as an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="multiPartQuery">
        /// The query expression.
        /// </param>
        /// <returns>
        /// A task returning the data set.
        /// </returns>
        public IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, IMultiPartQuery multiPartQuery)
        {
            var expressions = GenericVisitor.Visit((ExecutionContextExpression e) => Expression.Constant(context), this.FilterExpression).SplitByOrExpressions().Distinct(new ExpressionComparer()).ToArray();
            var orderBy = expressions.Length > 1 ? Enumerable.Empty<OrderByExpression>() : multiPartQuery.OrderByExpressions;
            var result = expressions.Select(subFilter => this.GetRows(context, this.CreateJoinQuery(context, multiPartQuery.ReplaceOrderBy(orderBy), subFilter)))
                .Aggregate((current, next) => current.Union(next, new RowIdComparer()));

            if (expressions.Length == 1)
            {
                return result;
            }

            context.Logger.Verbose($"Expression contains or, splitting in {expressions.Length} parts.");

            return result.OrderBy(multiPartQuery.OrderByExpressions);
        }

        /// <summary>
        /// Splits the query into queries for the left and right side of the join, a join filter, a result filter and the ORDER
        ///     BY expressions.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="joinFilter">
        /// The join Filter.
        /// </param>
        /// <returns>
        /// The <see cref="JoinQuery"/>.
        /// </returns>
        [NotNull]
        protected virtual JoinQuery CreateJoinQuery(IExecutionContext context, [NotNull] IMultiPartQuery query, Expression joinFilter)
        {
            var filterParts = (query.FilterExpression == null ? joinFilter : Expression.AndAlso(query.GetFilter(context), joinFilter)).SplitByAndExpressions();

            // Get all parts of the query that contain fields from both sources.
            var joinParts = filterParts.OfType<BinaryExpression>()
                .Where(b => b.IsComparison())
                .Where(comparison =>
                    {
                        var fields = comparison.GetFields().Select(f => f.SourceAlias).ToArray();
                        return fields.Intersect(this.Left.Aliases).Any() && fields.Intersect(this.Right.Aliases).Any();
                    })
                .ToArray();

            // What's left are the filters for only one source. These we can split in a left part, a right part, and a filter part over the result.
            var filter = filterParts.Except(joinParts, new ExpressionComparer()).DefaultIfEmpty().Aggregate(Expression.AndAlso);
            var leftFilter = filter.RemoveAllPartsThatAreNotInSource(this.Left);
            var rightFilter = filter.RemoveAllPartsThatAreNotInSource(this.Right);
            var resultFilter = filter.Except(leftFilter, rightFilter);
            var joinExpression = joinParts.OrderBy(p => p, new MostSpecificComparer()).First().MoveFieldsToTheLeft(this.Left);
            var ascending = joinExpression.NodeType != ExpressionType.GreaterThan && joinExpression.NodeType != ExpressionType.GreaterThanOrEqual;

            var leftQuery = new MultiPartQuery
                                {
                                    Fields = query.Fields
                                        .Where(f => this.Left.Aliases.Contains(f.SourceAlias))
                                        .Concat(joinParts.SelectMany(l => l.GetDataSourceFields(this.Left)))
                                        .Concat(resultFilter.GetDataSourceFields(this.Left))
                                        .Distinct(),
                                    FilterExpression = leftFilter,
                                    OrderByExpressions = new[]
                                                             {
                                                                 new OrderByExpression(joinExpression.Left, ascending)
                                                             },
                                };

            var rightQuery = new MultiPartQuery
                                 {
                                     Fields = query.Fields
                                         .Where(f => this.Right.Aliases.Contains(f.SourceAlias))
                                         .Concat(joinParts.SelectMany(l => l.GetDataSourceFields(this.Right)))
                                         .Concat(resultFilter.GetDataSourceFields(this.Right))
                                         .Distinct(),
                                     FilterExpression = rightFilter,
                                     OrderByExpressions = new[]
                                                              {
                                                                  new OrderByExpression(joinExpression.Right, ascending)
                                                              },
                                 };

            var exraJoinFilter = GenericVisitor.Visit(
                (ExecutionContextExpression e) => Expression.Constant(context),
                joinParts.Skip(1).DefaultIfEmpty().Aggregate<Expression>(Expression.AndAlso));

            return new JoinQuery(
                leftQuery,
                rightQuery,
                joinExpression.Left.GetRowExpression<Row>(),
                joinExpression.NodeType,
                joinExpression.Right.GetRowExpression<Row>(),
                joinParts.DefaultIfEmpty<Expression>().Aggregate(Expression.AndAlso),
                exraJoinFilter.GetJoinFunction(this.Left),
                query.OrderByExpressions,
                resultFilter);
        }

        /// <summary>
        /// Gets the joined data.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// A data set containing the rows retrieved from the source.
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
            /// <param name="leftKey">
            /// The left Key.
            /// </param>
            /// <param name="joinType">
            /// The join Type.
            /// </param>
            /// <param name="rightKey">
            /// The right Key.
            /// </param>
            /// <param name="joinExpression">
            /// The join Expression.
            /// </param>
            /// <param name="joinFilter">
            /// The join filter.
            /// </param>
            /// <param name="orderBy">
            /// The order by.
            /// </param>
            /// <param name="resultFilter">
            /// The filter.
            /// </param>
            public JoinQuery(IMultiPartQuery leftQuery, IMultiPartQuery rightQuery, Func<Row, object> leftKey, ExpressionType joinType, Func<Row, object> rightKey, Expression joinExpression, Func<Row, Row, bool> joinFilter, [NotNull] IEnumerable<IOrderByExpression> orderBy, Expression resultFilter)
            {
                this.LeftQuery = leftQuery;
                this.RightQuery = rightQuery;
                this.LeftKey = leftKey;
                this.RightKey = rightKey;
                this.JoinType = joinType;
                this.JoinExpression = joinExpression;
                this.JoinFilter = joinFilter;
                this.OrderBy = orderBy.ToArray();
                this.ResultFilter = resultFilter.GetRowFilter();
            }

            /// <summary>
            /// Gets the join filter.
            /// </summary>
            public Expression JoinExpression { get; }

            /// <summary>
            /// Gets the join filter.
            /// </summary>
            public Func<Row, Row, bool> JoinFilter { get; }

            /// <summary>
            /// Gets the join type.
            /// </summary>
            public ExpressionType JoinType { get; }

            /// <summary>
            /// Gets the left key.
            /// </summary>
            public Func<Row, object> LeftKey { get; }

            /// <summary>
            /// Gets the left query.
            /// </summary>
            public IMultiPartQuery LeftQuery { get; }

            /// <summary>
            /// Gets the order by.
            /// </summary>
            public IOrderByExpression[] OrderBy { get; }

            /// <summary>
            /// Gets the filter for the join result.
            /// </summary>
            public Func<Row, bool> ResultFilter { get; }

            /// <summary>
            /// Gets the right key.
            /// </summary>
            public Func<Row, object> RightKey { get; }

            /// <summary>
            /// Gets the right query.
            /// </summary>
            public IMultiPartQuery RightQuery { get; }
        }
    }
}