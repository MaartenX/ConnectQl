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
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The base class for CROSS APPLY and OUTER APPLY.
    /// </summary>
    internal abstract class ApplyBase : DataSource
    {
        /// <summary>
        /// Gets the extra fields.
        /// </summary>
        private readonly IEnumerable<IField> extraFields;

        /// <summary>
        /// Gets the left.
        /// </summary>
        private readonly DataSource left;

        /// <summary>
        /// Gets the right.
        /// </summary>
        private readonly DataSource right;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyBase"/> class.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="rightFactory">
        /// The right Factory.
        /// </param>
        protected ApplyBase([NotNull] DataSource left, [NotNull] DataSource right, [CanBeNull] Expression rightFactory)
            : base(new HashSet<string>(left.Aliases.Concat(right.Aliases)))
        {
            this.left = left;
            this.right = right;

            this.extraFields = rightFactory?.GetDataSourceFields(this.left).ToArray() ?? new Field[0];

            var context = Expression.Parameter(typeof(IExecutionContext));
            var row = Expression.Parameter(typeof(Row));

            this.RightFactory = rightFactory == null
                                    ? null
                                    : Expression.Lambda<Func<IExecutionContext, Row, DataSource>>(
                                        GenericVisitor.Visit(
                                            (ExecutionContextExpression e) => context,
                                            (SourceFieldExpression f) => f.CreateGetter(row),
                                            (UnaryExpression e) => e.NodeType == ExpressionType.Convert ? (e.Operand as SourceFieldExpression)?.CreateGetter(row, e.Type) : null,
                                            rightFactory),
                                        context,
                                        row).Compile();
        }

        /// <summary>
        /// Gets the right factory.
        /// </summary>
        protected Func<IExecutionContext, Row, DataSource> RightFactory { get; }

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
                                    Fields = multiPartQuery.Fields.Where(f => this.left.Aliases.Contains(f.SourceAlias)).Union(this.extraFields),
                                    FilterExpression = multiPartQuery.FilterExpression.FilterByAliases(this.left.Aliases),
                                    WildcardAliases = multiPartQuery.WildcardAliases.Intersect(this.left.Aliases),
                                };

            //// Create the enumerable.
            return context.CreateAsyncEnumerable(
                    async () =>
                        {
                            //// Retrieve the records from the left side.
                            var leftData = await this.left.GetRows(context, leftQuery).MaterializeAsync().ConfigureAwait(false);

                            var rightQuery = new MultiPartQuery
                                                 {
                                                     Fields = multiPartQuery.Fields.Where(f => this.right.Aliases.Contains(f.SourceAlias)),
                                                     FilterExpression = ApplyBase.RangesToJoinFilter(await this.FindRangesAsync(context, multiPartQuery.FilterExpression, leftData)),
                                                     WildcardAliases = multiPartQuery.WildcardAliases.Intersect(this.right.Aliases),
                                                 };

                            IAsyncReadOnlyCollection<Row> rightData = null;

                            if (this.RightFactory == null)
                            {
                                rightData = await this.right.GetRows(context, rightQuery).MaterializeAsync().ConfigureAwait(false);
                            }

                            var collection = this.CombineResults(context, leftData, rightData, rightQuery, rowBuilder);

                            collection = await collection.MaterializeAsync().ConfigureAwait(false);

                            return collection;
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
            return (await this.left.GetDataSourceDescriptorsAsync(context).ConfigureAwait(false))
                .Concat(
                    this.right == null
                        ? Enumerable.Empty<IDataSourceDescriptor>()
                        : await this.right.GetDataSourceDescriptorsAsync(context).ConfigureAwait(false));
        }

        /// <summary>
        /// Combines the left and right result sets.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="leftData">
        /// The left result set.
        /// </param>
        /// <param name="rightData">
        /// The right result set.
        /// </param>
        /// <param name="rightQuery">
        /// The query for the right side.
        /// </param>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{Row}"/>.
        /// </returns>
        protected abstract IAsyncEnumerable<Row> CombineResults(IInternalExecutionContext context, IAsyncReadOnlyCollection<Row> leftData, IAsyncReadOnlyCollection<Row> rightData, MultiPartQuery rightQuery, RowBuilder rowBuilder);

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
            var ranges = await expression.SplitByOrExpressions().ToRangedExpressionAsync(rows, this.right.Aliases).ConfigureAwait(false);

            return ranges
                .Select(r => r.SimplifyExpression(context))
                .Where(r => !object.Equals((r as ConstantExpression)?.Value, false))
                .DefaultIfEmpty().Aggregate(Expression.OrElse).SimplifyRanges();
        }
    }
}