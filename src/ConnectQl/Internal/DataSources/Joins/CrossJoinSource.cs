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

    /// <summary>
    /// The cross join source.
    /// </summary>
    internal class CrossJoinSource : JoinSourceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrossJoinSource"/> class.
        /// </summary>
        /// <param name="left">
        /// The left <see cref="DataSource"/>.
        /// </param>
        /// <param name="right">
        /// The right <see cref="DataSource"/>.
        /// </param>
        public CrossJoinSource(DataSource left, DataSource right)
            : base(left, right)
        {
        }

        /// <summary>
        /// Retrieves the data from the source as an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="query">
        /// The query expression.
        /// </param>
        /// <returns>
        /// A task returning the rows.
        /// </returns>
        protected override IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, JoinQuery query)
        {
            var rowBuilder = new RowBuilder();

            return this.Left.GetRows(context, query.LeftQuery)
                .CrossJoin(this.Right.GetRows(context, query.RightQuery), rowBuilder.CombineRows)
                .Where(query.ResultFilter?.GetRowFilter())
                .OrderBy(query.OrderBy);
        }
    }
}