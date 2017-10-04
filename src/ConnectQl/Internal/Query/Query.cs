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

namespace ConnectQl.Internal.Query
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The query.
    /// </summary>
    internal class Query : IQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="orderBy">
        /// The order by.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        public Query([CanBeNull] IEnumerable<string> fields, Expression filter, [CanBeNull] IEnumerable<IOrderByExpression> orderBy, int? count = null)
        {
            this.Fields = fields?.ToArray() ?? new string[0];
            this.RetrieveAllFields = false;
            this.FilterExpression = filter;
            this.OrderByExpressions = orderBy?.ToArray() ?? new OrderByExpression[0];
            this.Count = count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class, retrieving all fields.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="orderBy">
        /// The order by.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        public Query(Expression filter, [CanBeNull] IEnumerable<IOrderByExpression> orderBy, int? count = null)
        {
            this.Fields = new string[0];
            this.RetrieveAllFields = true;
            this.FilterExpression = filter;
            this.OrderByExpressions = orderBy?.ToArray() ?? new OrderByExpression[0];
            this.Count = count;
        }

        /// <summary>
        /// Gets the number of records to retrieve. When this is <c>null</c>, all records will be retrieved.
        /// </summary>
        public int? Count { get; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        public IEnumerable<string> Fields { get; }

        /// <summary>
        /// Gets the filter expression.
        /// </summary>
        public Expression FilterExpression { get; }

        /// <summary>
        /// Gets the order by expressions.
        /// </summary>
        public IEnumerable<IOrderByExpression> OrderByExpressions { get; }

        /// <summary>
        /// Gets a value indicating whether retrieve all fields.
        /// </summary>
        public bool RetrieveAllFields { get; }

        /// <summary>
        /// Gets the wildcard source aliases.
        /// </summary>
        public IEnumerable<string> WildcardAliases { get; }

        /// <summary>
        /// Retrieves the filter for the query.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>
        /// The filter for this query, or <c>null</c> when no filter exists.
        /// </returns>
        [CanBeNull]
        public Expression GetFilter(IExecutionContext context)
        {
            return this.FilterExpression == null ? null : GenericVisitor.Visit((ExecutionContextExpression e) => Expression.Constant(context), this.FilterExpression);
        }

        /// <summary>
        /// Retrieves the sort orders for the query.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>
        /// A collection of sort orders.
        /// </returns>
        [NotNull]
        public IEnumerable<IOrderByExpression> GetSortOrders(IExecutionContext context)
        {
            var visitor = GenericVisitor.Create((ExecutionContextExpression e) => Expression.Constant(context));

            return this.OrderByExpressions.Select(o => new OrderByExpression(visitor.Visit(o.Expression), o.Ascending));
        }
    }
}