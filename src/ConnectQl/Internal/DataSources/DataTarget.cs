﻿// MIT License
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
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Results;

    /// <summary>
    /// The data target.
    /// </summary>
    internal class DataTarget
    {
        /// <summary>
        /// The target.
        /// </summary>
        private readonly IDataTarget target;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTarget"/> class.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        public DataTarget(IDataTarget target)
        {
            this.target = target;
        }

        /// <summary>
        /// The write rows async.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="upsert">
        /// The upsert.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<long> WriteRowsAsync(IInternalExecutionContext context, IAsyncEnumerable<Row> rows, bool upsert)
        {
            context.Log.Verbose($"Writing to {context.GetDisplayName(this.target)}...");

            if (context.WriteProgressInterval != 0)
            {
                var i = 0L;
                rows = rows.Select(
                    a =>
                        {
                            i++;

                            if (i % context.WriteProgressInterval == 0)
                            {
                                context.Log.Information($"Wrote {i} items.");
                            }

                            return a;
                        });
            }

            var result = await this.target.WriteRowsAsync(context, rows, upsert);

            context.Log.Verbose($"Wrote {result} rows to {context.GetDisplayName(this.target)}");

            return result;
        }
    }
}