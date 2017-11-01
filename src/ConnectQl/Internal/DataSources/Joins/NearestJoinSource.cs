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
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Query;
    using ConnectQl.Internal.Validation.Operators;
    using ConnectQl.Results;

    /// <summary>
    /// The nearest inner join source.
    /// </summary>
    internal class NearestJoinSource : FilteredJoinSourceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NearestJoinSource"/> class.
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
        public NearestJoinSource(DataSource left, DataSource right, Expression filter, bool isInnerJoin)
            : base(left, right, filter, isInnerJoin)
        {
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
        protected override IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, JoinQuery query)
        {
            throw new NotImplementedException();
        }

        /*
        /// <summary>
        /// Retrieves the data from the source.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="query">
        ///     The query expression. Can be <c>null</c>.
        /// </param>
        /// <returns>
        /// A task returning the data set.
        /// </returns>
        public async Task<DataSet> GetDataAsync(IExecutionContext context, IQuery query)
        {

            var leftRows = await context.MaterializeAsync(await this.GetDataAsync(context, this.left, query, query.GetFilter(context), Enumerable.Empty<OrderByExpression>()).ConfigureAwait(false)).ConfigureAwait(false);

            if (leftRows.Count == 0)
            {
                return DataSet.Empty();
            }

            var sortOrders = new List<OrderByExpression>();
            var filter = await this.CreateJoinFilterAsync(context, leftRows, sortOrders);
            var rightRows = await context.MaterializeAsync(await this.GetDataAsync(context, this.right, query, filter, sortOrders).ConfigureAwait(false));

            if (rightRows.Count == 0)
            {
                return this.isInnerJoin ? DataSet.Empty() : DataSet.FromEnumerable(rightRows);
            }

            var joinFilter = this.GetFilter(context);

            return null;
        }
        */

        /// <summary>
        /// Creates the join filter.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <param name="leftRows">
        /// The left rows.
        /// </param>
        /// <param name="sortOrders">
        /// Will be filled with the sort orders for this join.
        /// </param>
        /// <returns>
        /// A task containing the the filter expression.
        /// </returns>
        private Task<Expression> CreateJoinFilterAsync(IExecutionContext context, IAsyncReadOnlyCollection<Row> leftRows, ICollection<OrderByExpression> sortOrders)
        {
            var filter = (Expression)null; // await this.GetFilter(context, null).ToRangedExpressionAsync(leftRows, this.right.Aliases);

            return Task.FromResult(new GenericVisitor
                       {
                           (GenericVisitor visitor, BinaryExpression node) =>
                               {
                                   if (node.NodeType != ExpressionType.And && node.NodeType != ExpressionType.AndAlso)
                                   {
                                       return null;
                                   }

                                   Expression leftSide, rightSide;

                                   if (((leftSide = visitor.Visit(node.Left)) as ConstantExpression)?.Value?.Equals(true) ?? false)
                                   {
                                       return visitor.Visit(node.Right);
                                   }

                                   if (((rightSide = visitor.Visit(node.Right)) as ConstantExpression)?.Value?.Equals(true) ?? false)
                                   {
                                       return visitor.Visit(node.Left);
                                   }

                                   return Expression.MakeBinary(ExpressionType.AndAlso, leftSide, rightSide);
                               },
                           (RangeExpression node) =>
                               node.Type == typeof(bool) && object.Equals(node.Min, false) && object.Equals(node.Max, true)
                                   ? (Expression)Expression.Constant(true)
                                   : node,
                           (BinaryExpression node) =>
                               {
                                   if (!node.IsComparison())
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
                                       case ExpressionType.GreaterThan:
                                       case ExpressionType.GreaterThanOrEqual:
                                       case ExpressionType.LessThan:
                                       case ExpressionType.LessThanOrEqual:
                                           sortOrders.Add(
                                               new OrderByExpression(
                                                   ConnectQlExpression.MakeSourceField(field.SourceName, field.FieldName),
                                                   node.NodeType == ExpressionType.GreaterThan || node.NodeType == ExpressionType.GreaterThanOrEqual));
                                           break;
                                   }

                                   return Expression.AndAlso(
                                       Operator.GenerateExpression(ExpressionType.GreaterThanOrEqual, field, Expression.Constant(range.Min, range.Type)),
                                       Operator.GenerateExpression(ExpressionType.LessThanOrEqual, field, Expression.Constant(range.Max, range.Type)));
                               },
                       }.Visit(filter));
        }
    }
}