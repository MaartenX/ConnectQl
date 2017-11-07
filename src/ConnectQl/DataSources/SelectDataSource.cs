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

namespace ConnectQl.DataSources
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.AsyncEnumerables.Policies;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Query;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The select data source.
    /// </summary>
    internal class SelectDataSource : DataSource
    {
        /// <summary>
        /// The select plan.
        /// </summary>
        private readonly IQueryPlan selectPlan;

        /// <summary>
        /// The sourceAlias.
        /// </summary>
        private readonly string alias;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectDataSource"/> class.
        /// </summary>
        /// <param name="selectPlan">
        /// The select query plan.
        /// </param>
        /// <param name="aliases">
        /// The aliases.
        /// </param>
        public SelectDataSource(IQueryPlan selectPlan, [NotNull] HashSet<string> aliases)
            : base(aliases)
        {
            this.selectPlan = selectPlan;
            this.alias = aliases.Single();
        }

        /// <summary>
        /// The get data async.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="multiPartQuery">
        /// The multi part query.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal override IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, [NotNull] IMultiPartQuery multiPartQuery)
        {
            var fieldReplacer = GenericVisitor.Create((SourceFieldExpression e) => ConnectQlExpression.MakeField(e.SourceName, e.FieldName, e.Type));

            var query =
                multiPartQuery.WildcardAliases.Contains(this.alias) ?
                    new Query(
                        fieldReplacer.Visit(multiPartQuery.FilterExpression),
                        multiPartQuery.OrderByExpressions.Select(o => new OrderByExpression(fieldReplacer.Visit(o.Expression), o.Ascending)),
                        multiPartQuery.Count)
                    : new Query(
                        multiPartQuery.Fields.Where(f => f.SourceAlias == this.alias).Select(f => f.FieldName),
                        fieldReplacer.Visit(multiPartQuery.FilterExpression),
                        multiPartQuery.OrderByExpressions.Select(o => new OrderByExpression(fieldReplacer.Visit(o.Expression), o.Ascending)),
                        multiPartQuery.Count);

            var rowBuilder = new RowBuilder(this.alias);

            return context
                .CreateAsyncEnumerable(async () => (await this.selectPlan.ExecuteAsync(context)).QueryResults.First().Rows)
                .Select(rowBuilder.Attach)
                .Where(query.GetFilter(context)?.GetRowFilter())
                .OrderBy(query.GetSortOrders(context));
        }

        /// <summary>
        /// Gets the descriptor for this data source.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [ItemNotNull]
        protected internal override async Task<IEnumerable<IDataSourceDescriptor>> GetDataSourceDescriptorsAsync(IExecutionContext context)
        {
            var source = this.selectPlan as IDescriptableDataSource;

            return new[]
                       {
                           source != null
                               ? await source.GetDataSourceDescriptorAsync(this.alias, context)
                               : Descriptor.DynamicDataSource(this.alias),
                       };
        }
    }
}