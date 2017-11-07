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

namespace ConnectQl.Intellisense.Protocol
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The serializable execute result.
    /// </summary>
    internal class SerializableExecuteResult : IExecuteResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableExecuteResult"/> class.
        /// </summary>
        [UsedImplicitly]
        public SerializableExecuteResult()
        {
        }

        /// <summary>
        /// Gets or sets the jobs.
        /// </summary>
        public SerializableJob[] Jobs { get; set; }

        /// <summary>
        /// Gets or sets the query results.
        /// </summary>
        public SerializableQueryResult[] QueryResults { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        public SerializableMessage[] Warnings { get; set; }

        /// <summary>
        /// Gets the jobs.
        /// </summary>
        IReadOnlyList<IJob> IExecuteResult.Jobs => this.Jobs;

        /// <summary>
        /// Gets the query results.
        /// </summary>
        IReadOnlyList<IQueryResult> IExecuteResult.QueryResults => this.QueryResults;

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        IReadOnlyList<IMessage> IExecuteResult.Warnings => this.Warnings;

        /// <summary>
        /// Creates an execute result from an existing execute result asynchronously.
        /// </summary>
        /// <param name="executeResult">
        /// The existing execute result.
        /// </param>
        /// <returns>
        /// A <see cref="Task{T}"/> returning a serializable execute result.
        /// </returns>
        public static async Task<SerializableExecuteResult> CreateAync([NotNull] IExecuteResult executeResult)
        {
            return new SerializableExecuteResult
                       {
                           Jobs = executeResult.Jobs.Select(job => new SerializableJob(job)).ToArray(),
                           Warnings = executeResult.Warnings.Select(warning => new SerializableMessage(warning)).ToArray(),
                           QueryResults = await Task.WhenAll(executeResult.QueryResults.Select(SerializableQueryResult.CreateAsync))
                       };
        }
    }
}