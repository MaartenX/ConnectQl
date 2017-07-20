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

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.DataSources;
    using ConnectQl.Internal.Extensions;

    /// <summary>
    /// The query extensions.
    /// </summary>
    internal static class SourceQueryExtensions
    {
        /// <summary>
        /// Appends a filter to the query.
        /// </summary>
        /// <param name="query">
        /// The query to append the filter to.
        /// </param>
        /// <param name="filterExpression">
        /// The filter expression.
        /// </param>
        /// <returns>
        /// The <see cref="IQuery"/>.
        /// </returns>
        public static IQuery AppendFilter(this IQuery query, Expression filterExpression)
        {
            return filterExpression == null
                       ? query
                       : query.ReplaceFilter(query.FilterExpression == null ? filterExpression : Expression.AndAlso(query.FilterExpression, filterExpression));
        }

        /// <summary>
        /// Gets the fields of the data source that are used in the query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="source">
        /// The data source to get the fields for.
        /// </param>
        /// <param name="filterExpression">
        /// The expression to use when looking for fields. When omitted, the <see cref="IQuery.FilterExpression"/> is used.
        /// </param>
        /// <returns>
        /// The fields that are used in the query.
        /// </returns>
        public static IField[] GetUsedFields(this IQuery query, DataSource source, Expression filterExpression = null)
        {
            return
                query.OrderByExpressions
                    .SelectMany(o => o.Expression.GetDataSourceFields(source))
                    .Concat((filterExpression ?? query.FilterExpression)
                    ?.GetDataSourceFields(source) ?? Enumerable.Empty<IField>())
                    .Distinct()
                    .ToArray();
        }

        /// <summary>
        /// Replaces the <see cref="IQuery.FilterExpression"/> with a new one.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filterExpression">
        /// The filter expression.
        /// </param>
        /// <returns>
        /// A new <see cref="Query"/>.
        /// </returns>
        public static IQuery ReplaceFilter(this IQuery query, Expression filterExpression)
        {
            return new Query(query.Fields, filterExpression, query.OrderByExpressions, query.Count);
        }

        /// <summary>
        /// Replaces the <see cref="IQuery.OrderByExpressions"/> with new ones.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="orderByExpressions">
        /// The ORDER BY expressions.
        /// </param>
        /// <returns>
        /// A new <see cref="Query"/>.
        /// </returns>
        public static IQuery ReplaceOrderBy(this IQuery query, IEnumerable<OrderByExpression> orderByExpressions)
        {
            return new Query(query.Fields, query.FilterExpression, orderByExpressions, query.Count);
        }
    }
}