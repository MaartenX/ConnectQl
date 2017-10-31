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
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Internal.Validation.Operators;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The inner join source.
    /// </summary>
    internal class JoinSource : FilteredJoinSourceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinSource"/> class.
        /// </summary>
        /// <param name="left">
        /// The first.
        /// </param>
        /// <param name="right">
        /// The second.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="isInnerJoin">
        /// <c>false</c> if this is a left join, <c>true</c> if this is an inner join.
        /// </param>
        public JoinSource(DataSource left, DataSource right, Expression filter, bool isInnerJoin)
            : base(left, right, filter, isInnerJoin)
        {
        }

        /// <summary>
        /// Retrieves records for the join.
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
        protected override IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, [NotNull] JoinQuery query)
        {
            var rowBuilder = new RowBuilder();

            return context.CreateAsyncEnumerable(
                    async () =>
                        {
                            var leftData = await this.Left.GetRows(context, query.LeftQuery).MaterializeAsync().ConfigureAwait(false);
                            var extraFilter = (await new[]
                                                         {
                                                             query.JoinExpression,
                                                         }.ToRangedExpressionAsync(leftData, this.Right.Aliases).ConfigureAwait(false)).Select(JoinSource.RangesToJoinFilter).First();
                            var rightData = await this.Right.GetRows(context, query.RightQuery.ReplaceFilter(extraFilter)).MaterializeAsync().ConfigureAwait(false);

                            return this.IsInnerJoin
                                       ? leftData.PreSortedJoin(rightData, query.LeftKey, query.JoinType, query.RightKey, query.JoinFilter, rowBuilder.CombineRows)
                                       : leftData.PreSortedLeftJoin(rightData, query.LeftKey, query.JoinType, query.RightKey, query.JoinFilter, rowBuilder.CombineRows);
                        })
                .Where(query.ResultFilter)
                .OrderBy(query.OrderBy);
        }

        /// <summary>
        /// The ranges to join filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private static Expression RangesToJoinFilter(Expression filter)
        {
            return GenericVisitor.Visit(
                (BinaryExpression node) =>
                    {
                        if (node.IsComparison())
                        {
                            return null;
                        }

                        var field = node.Left as SourceFieldExpression;
                        var range = node.Right as RangeExpression;

                        if (field == null || range == null)
                        {
                            return null;
                        }

                        switch (node.NodeType)
                        {
                            case ExpressionType.Equal:

                                return Expression.AndAlso(
                                    Operator.GenerateExpression(ExpressionType.GreaterThanOrEqual, field, Expression.Constant(range.Min, range.Type)),
                                    Operator.GenerateExpression(ExpressionType.LessThanOrEqual, field, Expression.Constant(range.Max, range.Type)));

                            case ExpressionType.GreaterThan:
                            case ExpressionType.GreaterThanOrEqual:

                                return Operator.GenerateExpression(node.NodeType, field, Expression.Constant(range.Min, range.Type));

                            case ExpressionType.LessThan:
                            case ExpressionType.LessThanOrEqual:

                                return Operator.GenerateExpression(node.NodeType, field, Expression.Constant(range.Max, range.Type));

                            case ExpressionType.NotEqual:

                                return Expression.Constant(true);

                            default:
                                return null;
                        }
                    },
                filter);
        }
    }
}