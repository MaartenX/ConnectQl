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

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    ///     The execute result.
    /// </summary>
    internal class ExecuteResult : IExecuteResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExecuteResult" /> class.
        /// </summary>
        /// <param name="affectedRecords">
        ///     The affected records.
        /// </param>
        /// <param name="rows">
        ///     The rows.
        /// </param>
        public ExecuteResult(long affectedRecords, IAsyncEnumerable<Row> rows)
        {
            this.QueryResults = new[]
                                    {
                                        new QueryResult(affectedRecords, rows),
                                    };
            this.Jobs = new Job[0];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExecuteResult" /> class.
        /// </summary>
        public ExecuteResult()
        {
            this.QueryResults = new QueryResult[0];
            this.Jobs = new Job[0];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExecuteResult" /> class.
        /// </summary>
        /// <param name="job">
        ///     The job.
        /// </param>
        public ExecuteResult(IJob job)
        {
            this.QueryResults = new QueryResult[0];
            this.Jobs = new[]
                            {
                                job,
                            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExecuteResult" /> class.
        /// </summary>
        /// <param name="combinedResults">
        ///     The combined results.
        /// </param>
        internal ExecuteResult(ICollection<ExecuteResult> combinedResults)
            : this(combinedResults, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExecuteResult" /> class.
        /// </summary>
        /// <param name="combinedResults">
        ///     The combined Results.
        /// </param>
        /// <param name="messages">
        ///     The messages.
        /// </param>
        internal ExecuteResult([NotNull] ICollection<ExecuteResult> combinedResults, [CanBeNull] MessageWriter messages)
        {
            this.QueryResults = combinedResults.SelectMany(c => c.QueryResults).ToArray();
            this.Jobs = combinedResults.SelectMany(c => c.Jobs).ToArray();
            this.Warnings = messages?.Where(msg => msg.Type == ResultMessageType.Warning).ToArray() ?? new Message[0];
        }

        /// <summary>
        ///     Gets the jobs.
        /// </summary>
        public IReadOnlyList<IJob> Jobs { get; }

        /// <summary>
        ///     Gets the query results.
        /// </summary>
        public IReadOnlyList<IQueryResult> QueryResults { get; }

        /// <summary>
        ///     Gets or sets the warnings.
        /// </summary>
        public IReadOnlyList<IMessage> Warnings { get; set; }
    }
}