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

namespace ConnectQl.Interfaces
{
    using System;
    using System.Collections.Generic;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Results;

    /// <summary>
    /// Implement this interface to support joins over tables.
    /// </summary>
    public interface ISupportsJoin
    {
        /// <summary>
        /// Gets the types of data sources that are supported when joining.
        /// </summary>
        IReadOnlyList<Type> SupportedDataSourceTypes { get; }

        /// <summary>
        /// Checks if this data source supports the join query.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <param name="query">
        /// The query to check.
        /// </param>
        /// <returns><c>true</c> if the <see cref="ISupportsJoin"/> supports the specified <paramref name="query"/>, <c>false</c> otherwise.</returns>
        bool SupportsJoinQuery(IExecutionContext context, IJoinQuery query);

        /// <summary>
        /// Retrieves the data from multiple sources as an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <param name="query">
        /// The join query expression. Can be <c>null</c>.
        /// </param>
        /// <returns>
        /// A task returning the data set.
        /// </returns>
        IAsyncEnumerable<Row> GetRows(IExecutionContext context, IRowBuilder rowBuilder, IJoinQuery query);
    }
}