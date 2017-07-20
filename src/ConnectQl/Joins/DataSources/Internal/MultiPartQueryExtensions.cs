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

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Interfaces;

    /// <summary>
    /// The multi part query extensions.
    /// </summary>
    internal static class MultiPartQueryExtensions
    {
        /// <summary>
        /// The get filter.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression GetFilter(this IMultiPartQuery query, IExecutionContext context)
        {
            return query.FilterExpression == null ? null : GenericVisitor.Visit((ExecutionContextExpression e) => Expression.Constant(context), query.FilterExpression);
        }

        /// <summary>
        /// The get used fields.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="System.Collections.IEnumerable"/>.
        /// </returns>
        public static IEnumerable<string> GetUsedFields(this IQuery query, object left, object filter)
        {
            return query.Fields.Concat(query.FilterExpression.GetFields().Select(f => f.FieldName)).Concat(query.OrderByExpressions.SelectMany(o => o.Expression.GetFields().Select(f => f.FieldName))).Distinct();
        }

        /// <summary>
        /// The get used fields.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="System.Collections.IEnumerable"/>.
        /// </returns>
        public static IEnumerable<IField> GetUsedFields(this IMultiPartQuery query, object left, object filter)
        {
            return query.Fields.Concat(query.FilterExpression.GetFields().Concat(query.OrderByExpressions.SelectMany(o => o.Expression.GetFields()))).Distinct();
        }

        /// <summary>
        /// The replace filter.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="IMultiPartQuery"/>.
        /// </returns>
        public static IMultiPartQuery ReplaceFilter(this IMultiPartQuery expression, Expression filter)
        {
            return new MultiPartQuery
                       {
                           Count = expression.Count,
                           Fields = expression.Fields,
                           OrderByExpressions = expression.OrderByExpressions,
                           FilterExpression = filter,
                           WildcardAliases = expression.WildcardAliases,
                       };
        }

        /// <summary>
        /// The replace order by.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="orderby">
        /// The orderby.
        /// </param>
        /// <returns>
        /// The <see cref="IMultiPartQuery"/>.
        /// </returns>
        public static IMultiPartQuery ReplaceOrderBy(this IMultiPartQuery expression, IEnumerable<IOrderByExpression> orderby)
        {
            return new MultiPartQuery
                       {
                           Count = expression.Count,
                           Fields = expression.Fields,
                           OrderByExpressions = orderby,
                           FilterExpression = expression.FilterExpression,
                           WildcardAliases = expression.WildcardAliases,
                       };
        }
    }
}