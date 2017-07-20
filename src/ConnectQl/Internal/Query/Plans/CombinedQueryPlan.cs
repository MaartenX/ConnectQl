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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;

    /// <summary>
    /// The combined query plan.
    /// </summary>
    internal class CombinedQueryPlan : IQueryPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedQueryPlan"/> class.
        /// </summary>
        /// <param name="subQueries">
        /// The sub queries.
        /// </param>
        public CombinedQueryPlan(IEnumerable<IQueryPlan> subQueries)
        {
            this.SubQueries = new ReadOnlyCollection<IQueryPlan>(subQueries.ToList());
        }

        /// <summary>
        /// Gets the sub queries.
        /// </summary>
        public ReadOnlyCollection<IQueryPlan> SubQueries { get; }

        /// <summary>
        /// Executes the plan.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ExecuteResult"/>.
        /// </returns>
        public async Task<ExecuteResult> ExecuteAsync(IInternalExecutionContext context)
        {
            return new ExecuteResult(await this.SubQueries.Where(p => p != null).AggregateAsync(
                                         new List<ExecuteResult>(),
                                         async (result, plan) =>
                                             {
                                                 var planResult = await plan.ExecuteAsync(context);

                                                 result.Add(planResult);

                                                 return result;
                                             }));
        }
    }
}