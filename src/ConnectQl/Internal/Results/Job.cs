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

namespace ConnectQl.Internal.Results
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Results;

    /// <summary>
    /// The job.
    /// </summary>
    internal class Job : IJob
    {
        /// <summary>
        /// The plan.
        /// </summary>
        private readonly IQueryPlan plan;

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        /// <param name="executionContext">
        /// The execution context.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="plan">
        /// The plan.
        /// </param>
        /// <param name="triggers">
        /// The triggers.
        /// </param>
        internal Job(IInternalExecutionContext executionContext, string name, IQueryPlan plan, IEnumerable<IJobTrigger> triggers)
        {
            this.Name = name;
            this.Triggers = triggers.ToArray();
            this.ExecutionContext = executionContext;
            this.plan = plan;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        public IReadOnlyList<IJobTrigger> Triggers { get; }

        /// <summary>
        /// Gets the execution context.
        /// </summary>
        internal IInternalExecutionContext ExecutionContext { get; }

        /// <summary>
        /// Runs the job.
        /// </summary>
        /// <param name="jobContext">
        /// The job context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IExecuteResult> RunAsync(IJobContext jobContext)
        {
            var result = await this.plan.ExecuteAsync(this.ExecutionContext);

            return result;
        }
    }
}