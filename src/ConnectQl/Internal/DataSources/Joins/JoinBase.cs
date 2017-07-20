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
    using ConnectQl.Internal.Comparers;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    /// <summary>
    /// The join base.
    /// </summary>
    internal abstract class JoinBase : DataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinBase"/> class.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        protected JoinBase(DataSource left, DataSource right, Expression filter)
            : base(new HashSet<string>(left.Aliases.Union(right.Aliases)))
        {
            this.Left = left;
            this.Right = right;
            this.Filter = filter;
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        protected Expression Filter { get; }

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
        internal override IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, IMultiPartQuery multiPartQuery)
        {
            var rowBuilder = new RowBuilder();

            //// For a join, we can append the filter expression (WHERE part) with the filter for the join.
            var filterExpression = multiPartQuery.FilterExpression == null ? this.Filter : Expression.AndAlso(multiPartQuery.FilterExpression, this.Filter);

            //// Build the left part by filtering by parts that contain the fields of the left side.
            var leftQuery = new MultiPartQuery
                                {
                                    Fields = multiPartQuery.Fields.Where(f => this.Left.Aliases.Contains(f.SourceAlias)).Union(this.Filter.GetFields().Where(f => this.Left.Aliases.Contains(f.SourceAlias))),
                                    FilterExpression = filterExpression.FilterByAliases(this.Left.Aliases).Simplify(context),
                                    WildcardAliases = multiPartQuery.WildcardAliases.Intersect(this.Left.Aliases).ToArray(),
                                };

            //// Create the enumerable.
            return context.CreateAsyncEnumerable(
                    async () =>
                        {
                            //// Split by OR-expressions,
                            var joinsParts = this.Filter.SplitByOrExpressions()

                                //// Then, re-order AND expressions, so the most specific comparer is in front.
                                .Select(part => part.Simplify(context).SplitByAndExpressions().Cast<CompareExpression>()
                                    .Select(c => c.MoveFieldsToTheLeft(this.Left))
                                    .OrderBy(p => p, MostSpecificComparer.Default)
                                    .ToArray())
                                .ToArray();

                            //// Retrieve the records from the left side.
                            var leftData = await this.Left.GetRows(context, leftQuery).MaterializeAsync().ConfigureAwait(false);

                            if (leftData.Count == 0)
                            {
                                return context.CreateEmptyAsyncEnumerable<Row>();
                            }

                            //// Build the right query, by using the
                            var rightQuery = new MultiPartQuery
                                                 {
                                                     Fields = multiPartQuery.Fields.Where(f => this.Right.Aliases.Contains(f.SourceAlias)).Union(this.Filter.GetDataSourceFields(this.Right)),
                                                     FilterExpression = RangesToJoinFilter(await this.FindRangesAsync(context, filterExpression, leftData)),
                                                     WildcardAliases = multiPartQuery.WildcardAliases.Intersect(this.Right.Aliases),
                                                 };

                            var rightData = await this.Right.GetRows(context, rightQuery).MaterializeAsync().ConfigureAwait(false);
                            var result = this.CombineResults(joinsParts, rowBuilder, leftData, rightData);

                            result = await result.MaterializeAsync();

                            return result;
                        })
                .Where(multiPartQuery.FilterExpression.GetRowFilter())
                .OrderBy(multiPartQuery.OrderByExpressions)
                .AfterLastElement(count => context.Log.Verbose($"{this.GetType().Name} returned {count} records."));
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
        protected internal override async Task<IEnumerable<IDataSourceDescriptor>> GetDataSourceDescriptorsAsync(IExecutionContext context)
        {
            return (await this.Left.GetDataSourceDescriptorsAsync(context).ConfigureAwait(false))
                .Concat(await this.Right.GetDataSourceDescriptorsAsync(context).ConfigureAwait(false));
        }

        /// <summary>
        /// Combines the results of the left and right parts into the query result.
        /// </summary>
        /// <param name="joinsParts">
        /// An array of array of compare expressions.
        /// </param>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <param name="leftData">
        /// The left data.
        /// </param>
        /// <param name="rightData">
        /// The right data.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{Row}"/>.
        /// </returns>
        protected abstract IAsyncEnumerable<Row> CombineResults(CompareExpression[][] joinsParts, RowBuilder rowBuilder, IAsyncReadOnlyCollection<Row> leftData, IAsyncReadOnlyCollection<Row> rightData);

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
                (CompareExpression node) =>
                    {
                        var rightRange = node.Right as RangeExpression;

                        if (rightRange == null)
                        {
                            var leftRange = node.Left as RangeExpression;

                            if (leftRange == null)
                            {
                                return null;
                            }

                            switch (node.CompareType)
                            {
                                case ExpressionType.Equal:

                                    return Expression.AndAlso(
                                        CustomExpression.MakeCompare(ExpressionType.GreaterThanOrEqual, leftRange.MinExpression, node.Right),
                                        CustomExpression.MakeCompare(ExpressionType.LessThanOrEqual, leftRange.MaxExpression, node.Right));

                                case ExpressionType.GreaterThan:
                                case ExpressionType.GreaterThanOrEqual:

                                    return CustomExpression.MakeCompare(node.CompareType, leftRange.MinExpression, node.Right);

                                case ExpressionType.LessThan:
                                case ExpressionType.LessThanOrEqual:

                                    return CustomExpression.MakeCompare(node.CompareType, leftRange.MaxExpression, node.Right);

                                case ExpressionType.NotEqual:

                                    return Expression.Constant(true);

                                default:
                                    return null;
                            }
                        }

                        switch (node.CompareType)
                        {
                            case ExpressionType.Equal:

                                return Expression.AndAlso(
                                    CustomExpression.MakeCompare(ExpressionType.GreaterThanOrEqual, node.Left, rightRange.MinExpression),
                                    CustomExpression.MakeCompare(ExpressionType.LessThanOrEqual, node.Left, rightRange.MaxExpression));

                            case ExpressionType.GreaterThan:
                            case ExpressionType.GreaterThanOrEqual:

                                return CustomExpression.MakeCompare(node.CompareType, node.Left, rightRange.MinExpression);

                            case ExpressionType.LessThan:
                            case ExpressionType.LessThanOrEqual:

                                return CustomExpression.MakeCompare(node.CompareType, node.Left, rightRange.MaxExpression);

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
                .Where(r => !Equals((r as ConstantExpression)?.Value, false))
                .DefaultIfEmpty().Aggregate(Expression.OrElse).SimplifyRanges();
        }
    }
}