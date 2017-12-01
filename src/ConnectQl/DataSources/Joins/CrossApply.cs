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

namespace ConnectQl.DataSources.Joins
{
    using System.Linq.Expressions;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The cross apply.
    /// </summary>
    internal class CrossApply : ApplyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrossApply"/> class.
        /// </summary>
        /// <param name="left">
        /// The left data source.
        /// </param>
        /// <param name="right">
        /// The right data source.
        /// </param>
        /// <param name="rightFactory">
        /// The right expr.
        /// </param>
        public CrossApply(DataSource left, DataSource right, Expression rightFactory)
            : base(left, right, rightFactory)
        {
        }

        /// <summary>
        /// Combines the left and right result sets.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="leftData">
        /// The left result set.
        /// </param>
        /// <param name="rightData">
        /// The right result set.
        /// </param>
        /// <param name="rightQuery">
        /// The query for the right side.
        /// </param>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{Row}"/>.
        /// </returns>
        protected override IAsyncEnumerable<Row> CombineResults(IInternalExecutionContext context, IAsyncReadOnlyCollection<Row> leftData, IAsyncReadOnlyCollection<Row> rightData, MultiPartQuery rightQuery, [NotNull] RowBuilder rowBuilder)
        {
            return this.RightFactory != null
                       ? leftData.CrossApply(row => this.RightFactory(context, row).GetRows(context, rightQuery), rowBuilder.CombineRows)
                       : leftData.CrossApply(row => rightData, rowBuilder.CombineRows);
        }
    }
}