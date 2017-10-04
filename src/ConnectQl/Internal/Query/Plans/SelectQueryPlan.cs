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

namespace ConnectQl.Internal.Query.Plans
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.DataSources;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;

    using JetBrains.Annotations;

    using AsyncValueFactory = System.Func<ConnectQl.Interfaces.IExecutionContext, ConnectQl.Results.Row, System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>>>;
    using ValueFactory = System.Func<ConnectQl.Interfaces.IExecutionContext, ConnectQl.Results.Row, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>>;

    /// <summary>
    /// The select group query plan.
    /// </summary>
    internal class SelectQueryPlan : IQueryPlan, IDescriptableDataSource
    {
        /// <summary>
        /// The value factories.
        /// </summary>
        private readonly AsyncValueFactory asyncValueFactory;

        /// <summary>
        /// The data source factory.
        /// </summary>
        private readonly Func<IExecutionContext, Task<DataSource>> dataSourceFactory;

        /// <summary>
        /// The field names.
        /// </summary>
        private readonly IEnumerable<string> fieldNames;

        /// <summary>
        /// The query.
        /// </summary>
        private readonly IMultiPartQuery query;

        /// <summary>
        /// The value factories.
        /// </summary>
        private readonly ValueFactory valueFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectQueryPlan"/> class.
        /// </summary>
        /// <param name="dataSourceFactory">
        /// The data source factory.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="fieldNames">
        /// The field names.
        /// </param>
        /// <param name="asyncValueFactory">
        /// Lambda that creates the values for a row.
        /// </param>
        public SelectQueryPlan(Func<IExecutionContext, Task<DataSource>> dataSourceFactory, IMultiPartQuery query, IEnumerable<string> fieldNames, AsyncValueFactory asyncValueFactory)
            : this(dataSourceFactory, query, fieldNames)
        {
            this.asyncValueFactory = asyncValueFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectQueryPlan"/> class.
        /// </summary>
        /// <param name="dataSourceFactory">
        /// The data source factory.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="fieldNames">
        /// The field names.
        /// </param>
        /// <param name="valueFactory">
        /// Lambda that creates the values for a row.
        /// </param>
        public SelectQueryPlan(Func<IExecutionContext, Task<DataSource>> dataSourceFactory, IMultiPartQuery query, IEnumerable<string> fieldNames, ValueFactory valueFactory)
            : this(dataSourceFactory, query, fieldNames)
        {
            this.valueFactory = valueFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectQueryPlan"/> class.
        /// </summary>
        /// <param name="dataSourceFactory">
        /// The data source factory.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="fieldNames">
        /// The field names.
        /// </param>
        private SelectQueryPlan(Func<IExecutionContext, Task<DataSource>> dataSourceFactory, IMultiPartQuery query, IEnumerable<string> fieldNames)
        {
            this.dataSourceFactory = dataSourceFactory;
            this.query = query;
            this.fieldNames = fieldNames;
        }

        /// <summary>
        /// Executes the plan.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        [ItemNotNull]
        public async Task<ExecuteResult> ExecuteAsync(IInternalExecutionContext context)
        {
            var rows = (await this.dataSourceFactory(context).ConfigureAwait(false)).GetRows(context, this.query);
            var rowBuilder = new RowBuilder(new FieldMapping(this.fieldNames));

            rows = this.asyncValueFactory != null
                       ? rows.Select(async row => rowBuilder.CreateRow<object>(row.UniqueId, await this.asyncValueFactory(context, row)))
                       : rows.Select(row => rowBuilder.CreateRow(row.UniqueId, this.valueFactory(context, row)));

#if DEBUG
            rows = await rows.MaterializeAsync();
#endif

            return new ExecuteResult(0, rows);
        }

        /// <summary>
        /// Gets the descriptor for this data source.
        /// </summary>
        /// <param name="sourceAlias">
        /// The data source sourceAlias.
        /// </param>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(string sourceAlias, IExecutionContext context)
        {
            var dataSource = await this.dataSourceFactory(context).ConfigureAwait(false);

            var descriptors = dataSource.GetDataSourceDescriptorsAsync(context).ConfigureAwait(false);

            return Descriptor.ForDataSource(sourceAlias, this.fieldNames.Where(f => !f.Contains("*")).Select(f => Descriptor.ForColumn(f, typeof(object))), this.fieldNames.Any(f => f.Contains("*")));
        }
    }
}