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
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables.Policies;
    using ConnectQl.DataSources;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The insert query plan.
    /// </summary>
    internal class InsertQueryPlan : IQueryPlan
    {
        /// <summary>
        /// The data generator.
        /// </summary>
        private readonly IQueryPlan dataGenerator;

        /// <summary>
        /// The data target expr.
        /// </summary>
        private readonly Func<IExecutionContext, DataTarget> dataTargetFactory;

        /// <summary>
        /// The upsert.
        /// </summary>
        private readonly bool upsert;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertQueryPlan"/> class.
        /// </summary>
        /// <param name="dataTargetFactory">
        /// The data target expr.
        /// </param>
        /// <param name="dataGenerator">
        /// The data generator.
        /// </param>
        /// <param name="upsert">
        /// The upsert.
        /// </param>
        public InsertQueryPlan(Func<IExecutionContext, DataTarget> dataTargetFactory, IQueryPlan dataGenerator, bool upsert)
        {
            this.dataTargetFactory = dataTargetFactory;
            this.dataGenerator = dataGenerator;
            this.upsert = upsert;
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
            var dataSet = (await this.dataGenerator.ExecuteAsync(context)).QueryResults.First().Rows;
            var dataTarget = this.dataTargetFactory(context);

            return new ExecuteResult(await dataTarget.WriteRowsAsync(context, dataSet, this.upsert), context.CreateEmptyAsyncEnumerable<Row>());
        }
    }
}