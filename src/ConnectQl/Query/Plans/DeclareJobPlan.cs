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
    using System.Threading.Tasks;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The declare job plan.
    /// </summary>
    internal class DeclareJobPlan : IQueryPlan
    {
        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The plan.
        /// </summary>
        private readonly IQueryPlan plan;

        /// <summary>
        /// The triggers factory.
        /// </summary>
        private readonly Func<IExecutionContext, Task<IEnumerable<IJobTrigger>>> triggersFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeclareJobPlan"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="plan">
        /// The plan.
        /// </param>
        /// <param name="triggersFactory">
        /// The trigger factories.
        /// </param>
        public DeclareJobPlan(string name, IQueryPlan plan, Func<IExecutionContext, Task<IEnumerable<IJobTrigger>>> triggersFactory)
        {
            this.name = name;
            this.plan = plan;
            this.triggersFactory = triggersFactory;
        }

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [ItemNotNull]
        public async Task<ExecuteResult> ExecuteAsync(IInternalExecutionContext context)
        {
            return new ExecuteResult(new Job(context, this.name, this.plan, await this.triggersFactory(context)));
        }
    }
}