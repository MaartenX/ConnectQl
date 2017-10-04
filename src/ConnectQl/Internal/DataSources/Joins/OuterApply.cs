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

namespace ConnectQl.Internal.DataSources.Joins
{
    using System.Linq.Expressions;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The cross apply.
    /// </summary>
    internal class OuterApply : ApplyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OuterApply"/> class.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="rightFactory">
        /// The right Factory.
        /// </param>
        public OuterApply(DataSource left, DataSource right, Expression rightFactory)
            : base(left, right, rightFactory)
        {
        }

        /// <summary>
        /// The cross apply.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="leftData">
        /// The left data.
        /// </param>
        /// <param name="rightData">
        /// The right data.
        /// </param>
        /// <param name="rightQuery">
        /// The right query.
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
                       ? leftData.OuterApply(row => this.RightFactory(context, row).GetRows(context, rightQuery), rowBuilder.CombineRows)
                       : leftData.OuterApply(row => rightData, rowBuilder.CombineRows);
        }
    }
}