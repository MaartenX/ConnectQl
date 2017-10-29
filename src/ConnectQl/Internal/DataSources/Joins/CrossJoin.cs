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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Internal.Validation.Operators;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The CROSS JOIN.
    /// </summary>
    internal class CrossJoin : DataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrossJoin"/> class.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        protected CrossJoin([NotNull] DataSource left, [NotNull] DataSource right)
            : base(new HashSet<string>(left.Aliases.Concat(right.Aliases)))
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        protected DataSource Left { get; }

        /// <summary>
        /// Gets the right.
        /// </summary>
        protected DataSource Right { get; }

        /// <summary>
        /// Gets the rows for the join.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="multiPartQuery">
        /// The multi part query.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{Row}"/>.
        /// </returns>
        internal override IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, [NotNull] IMultiPartQuery multiPartQuery)
        {
            var rowBuilder = new RowBuilder();

            //// Build the left part by filtering by parts that contain the fields of the left side.
            var leftQuery = new MultiPartQuery
                                {
                                    Fields = multiPartQuery.Fields.Where(f => this.Left.Aliases.Contains(f.SourceAlias)),
                                    FilterExpression = multiPartQuery.FilterExpression.FilterByAliases(this.Left.Aliases),
                                    WildcardAliases = multiPartQuery.WildcardAliases.Intersect(this.Left.Aliases).ToArray(),
                                };

            //// Create the enumerable.
            return context.CreateAsyncEnumerable(
                    async () =>
                        {
                            //// Retrieve the records from the left side.
                            var leftData = await this.Left.GetRows(context, leftQuery).MaterializeAsync().ConfigureAwait(false);

                            var rightQuery = new MultiPartQuery
                                                 {
                                                     Fields = multiPartQuery.Fields.Where(f => this.Right.Aliases.Contains(f.SourceAlias)),
                                                     FilterExpression = CrossJoin.RangesToJoinFilter(await this.FindRangesAsync(context, multiPartQuery.FilterExpression, leftData)),
                                                     WildcardAliases = multiPartQuery.WildcardAliases.Intersect(this.Right.Aliases),
                                                 };

                            var rightData = await this.Right.GetRows(context, rightQuery).MaterializeAsync().ConfigureAwait(false);

                            return leftData.CrossJoin(rightData, rowBuilder.CombineRows);
                        })
                .Where(multiPartQuery.FilterExpression.GetRowFilter())
                .OrderBy(multiPartQuery.OrderByExpressions)
                .AfterLastElement(count => context.Logger.Verbose($"{this.GetType().Name} returned {count} records."));
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
        [ItemNotNull]
        protected internal override async Task<IEnumerable<IDataSourceDescriptor>> GetDataSourceDescriptorsAsync(IExecutionContext context)
        {
            return (await this.Left.GetDataSourceDescriptorsAsync(context).ConfigureAwait(false))
                .Concat(await this.Right.GetDataSourceDescriptorsAsync(context).ConfigureAwait(false));
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
                        if (!node.IsComparison())
                        {
                            return null;
                        }

                        var rightRange = node.Right as RangeExpression;

                        if (rightRange == null)
                        {
                            var leftRange = node.Left as RangeExpression;

                            if (leftRange == null)
                            {
                                return null;
                            }

                            switch (node.NodeType)
                            {
                                case ExpressionType.Equal:

                                    return Expression.AndAlso(
                                        BinaryOperator.GenerateExpression(ExpressionType.GreaterThanOrEqual, leftRange.MinExpression, node.Right),
                                        BinaryOperator.GenerateExpression(ExpressionType.LessThanOrEqual, leftRange.MaxExpression, node.Right));

                                case ExpressionType.GreaterThan:
                                case ExpressionType.GreaterThanOrEqual:

                                    return BinaryOperator.GenerateExpression(node.NodeType, leftRange.MinExpression, node.Right);

                                case ExpressionType.LessThan:
                                case ExpressionType.LessThanOrEqual:

                                    return BinaryOperator.GenerateExpression(node.NodeType, leftRange.MaxExpression, node.Right);

                                case ExpressionType.NotEqual:

                                    return Expression.Constant(true);

                                default:
                                    return null;
                            }
                        }

                        switch (node.NodeType)
                        {
                            case ExpressionType.Equal:

                                return Expression.AndAlso(
                                    BinaryOperator.GenerateExpression(ExpressionType.GreaterThanOrEqual, node.Left, rightRange.MinExpression),
                                    BinaryOperator.GenerateExpression(ExpressionType.LessThanOrEqual, node.Left, rightRange.MaxExpression));

                            case ExpressionType.GreaterThan:
                            case ExpressionType.GreaterThanOrEqual:

                                return BinaryOperator.GenerateExpression(node.NodeType, node.Left, rightRange.MinExpression);

                            case ExpressionType.LessThan:
                            case ExpressionType.LessThanOrEqual:

                                return BinaryOperator.GenerateExpression(node.NodeType, node.Left, rightRange.MaxExpression);

                            case ExpressionType.NotEqual:

                                return Expression.Constant(true);

                            default:
                                return null;
                        }
                    },
                filter);
        }

        /// <summary>
        /// Finds the ranges in an expression using the already retrieved rows.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> returning the <see cref="Expression"/> containing ranges.
        /// </returns>
        private async Task<Expression> FindRangesAsync(IExecutionContext context, Expression expression, IAsyncReadOnlyCollection<Row> rows)
        {
            var ranges = await expression.SplitByOrExpressions().ToRangedExpressionAsync(rows, this.Right.Aliases).ConfigureAwait(false);

            return ranges
                .Select(r => r.SimplifyExpression(context))
                .Where(r => !object.Equals((r as ConstantExpression)?.Value, false))
                .DefaultIfEmpty().Aggregate(Expression.OrElse).SimplifyRanges();
        }
    }
}