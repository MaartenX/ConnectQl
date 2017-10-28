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

namespace ConnectQl.Internal.DataSources
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Comparers;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Query;
    using ConnectQl.Results;

    /// <summary>
    /// The data source.
    /// </summary>
    [DebuggerDisplay("{source.GetType().Name,nq}")]
    internal abstract class DataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class.
        /// </summary>
        /// <param name="aliases">
        /// The aliases.
        /// </param>
        protected DataSource(HashSet<string> aliases)
        {
            this.Aliases = aliases;
        }

        /// <summary>
        /// Gets the aliases for this data source.
        /// </summary>
        public HashSet<string> Aliases { get; }

        /// <summary>
        /// When a query contains Or/OrElse, we retrieve the results for each part and UNION the result.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <param name="filter">
        /// The filter to split into Or/OrElse parts.
        /// </param>
        /// <param name="orderByExpressions">
        /// When the parts are UNIONed, they need to be resorted by these expressions. This only happens if the filter contains
        ///     Or/OrElse expressions.
        /// </param>
        /// <param name="retrieveSubQuery">
        /// A function that retrieves the data from a data source.
        /// </param>
        /// <returns>
        /// A data set containing the UNION of all parts.
        /// </returns>
        public static IAsyncEnumerable<Row> GetDataParts(IExecutionContext context, Expression filter, IEnumerable<OrderByExpression> orderByExpressions, Func<Expression, IEnumerable<OrderByExpression>, IAsyncEnumerable<Row>> retrieveSubQuery)
        {
            orderByExpressions = orderByExpressions.ToArray();

            var expressions = filter.SplitByOrExpressions().Distinct(new ExpressionComparer()).ToArray();
            var orderBy = orderByExpressions;

            if (expressions.Length > 1)
            {
                orderBy = Enumerable.Empty<OrderByExpression>();
                context.Logger.Verbose($"Expression contains or, splitting in {expressions.Length} parts.");
            }

            var result = expressions.Select(subFilter => retrieveSubQuery(subFilter, orderBy))
                .Aggregate((current, next) => current.Union(next, new RowIdComparer()));

            if (expressions.Length > 1)
            {
                result = result.OrderBy(orderByExpressions);
            }

            return result;
        }

        /// <summary>
        /// Gets the rows of this datasource that conform to the query..
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{Row}"/> containing all returned rows.
        /// </returns>
        internal abstract IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, IMultiPartQuery query);

        /// <summary>
        /// Gets the descriptors for this data source.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>
        /// All data sources inside this data source.
        /// </returns>
        protected internal abstract Task<IEnumerable<IDataSourceDescriptor>> GetDataSourceDescriptorsAsync(IExecutionContext context);
    }
}