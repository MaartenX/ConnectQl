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

namespace ConnectQl.Query.Plans
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.AsyncEnumerables.Policies;
    using ConnectQl.Comparers;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Parser.Ast.Expressions;
    using ConnectQl.Results;
    
    using JetBrains.Annotations;
    
    #region "No Resharper Refactoring here"

    using AsyncGroupValueFactory = System.Func<Interfaces.IExecutionContext, AsyncEnumerables.IAsyncReadOnlyCollection<Results.Row>, System.Threading.Tasks.Task<System.Collections.Generic.KeyValuePair<string, object>[]>>;

    #endregion

    /// <summary>
    /// The select group query source.
    /// </summary>
    internal class SelectGroupByQueryPlan : IQueryPlan
    {
        /// <summary>
        /// The fields.
        /// </summary>
        private readonly string[] fields;

        /// <summary>
        /// The group fields.
        /// </summary>
        private readonly string[] groupFields;

        /// <summary>
        /// The having expression. Null if no having clause is added.
        /// </summary>
        private readonly Expression having;

        /// <summary>
        /// The orders.
        /// </summary>
        private readonly IEnumerable<OrderByExpression> orders;

        /// <summary>
        /// The plan.
        /// </summary>
        private readonly SelectQueryPlan plan;

        /// <summary>
        /// The group factories.
        /// </summary>
        private readonly AsyncGroupValueFactory rowFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectGroupByQueryPlan"/> class.
        /// </summary>
        /// <param name="plan">
        /// The plan.
        /// </param>
        /// <param name="rowFactory">
        /// The factory function to convert a group into the values for a row.
        /// </param>
        /// <param name="groupFields">
        /// The group fields.
        /// </param>
        /// <param name="aliases">
        /// The aliases.
        /// </param>
        /// <param name="having">
        /// The having expression. Null if no having clause is added.
        /// </param>
        /// <param name="orders">
        /// The expressions to order by, or null if none are available.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        public SelectGroupByQueryPlan(SelectQueryPlan plan, AsyncGroupValueFactory rowFactory, [NotNull] IEnumerable<string> groupFields, ReadOnlyCollection<AliasedConnectQlExpression> aliases, Expression having, IEnumerable<OrderByExpression> orders, [NotNull] IEnumerable<string> fields)
        {
            this.plan = plan;
            this.rowFactory = async (c, rows) =>
                {
                    try
                    {
                        return await rowFactory(c, rows);
                    }
                    catch (Exception e)
                    {
                        return aliases.Select(a => new KeyValuePair<string, object>(a.Alias, new Error(e))).ToArray();
                    }
                };
            this.having = having;
            this.orders = orders;
            this.fields = fields.ToArray();
            this.groupFields = groupFields.ToArray();
        }

        /// <summary>
        /// Executes the plan.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ExecuteResult"/>.
        /// </returns>
        [ItemNotNull]
        public async Task<ExecuteResult> ExecuteAsync(IInternalExecutionContext context)
        {
            var rowBuilder = new RowBuilder(new FieldMapping(this.fields));
            var result = await this.plan.ExecuteAsync(context).ConfigureAwait(false);
            var id = 0;
            var grouped = result.QueryResults[0].Rows
                .GroupBy(row => this.groupFields.Select(f => row[f]).ToArray(), ArrayOfObjectComparer.Default)
                .Select(async group => rowBuilder.CreateRow(id++, await this.rowFactory(context, group).ConfigureAwait(false)))
                .Where(this.having.GetRowFilter())
                .OrderBy(this.orders);

#if DEBUG
            grouped = await context.MaterializeAsync(grouped).ConfigureAwait(false);
#endif

            return new ExecuteResult(0, grouped);
        }
    }
}