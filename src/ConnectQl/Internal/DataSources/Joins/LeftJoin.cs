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
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Internal.Comparers;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The left join source.
    /// </summary>
    internal class LeftJoin : JoinBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeftJoin"/> class.
        /// </summary>
        /// <param name="left">
        /// The left data source.
        /// </param>
        /// <param name="right">
        /// The right data source.
        /// </param>
        /// <param name="filter">
        /// The join filter.
        /// </param>
        public LeftJoin(DataSource left, DataSource right, Expression filter)
            : base(left, right, filter)
        {
        }

        /// <summary>
        /// Combines the results of the left and right parts into the query result.
        /// </summary>
        /// <param name="joinsParts">
        /// An array of array of compare expressions.
        /// </param>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <param name="leftData">
        /// The left data.
        /// </param>
        /// <param name="rightData">
        /// The right data.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{Row}"/>.
        /// </returns>
        protected override IAsyncEnumerable<Row> CombineResults([NotNull] CompareExpression[][] joinsParts, RowBuilder rowBuilder, IAsyncReadOnlyCollection<Row> leftData, IAsyncReadOnlyCollection<Row> rightData)
        {
            return joinsParts
                .Select(joinPart =>
                    leftData.LeftJoin(
                        rightData,
                        joinPart[0].Left.GetRowExpression<Row>(),
                        joinPart[0].CompareType,
                        joinPart[0].Right.GetRowExpression<Row>(),
                        joinPart.Skip(1).DefaultIfEmpty().Aggregate<Expression>(Expression.AndAlso).GetJoinFunction(this.Left),
                        rowBuilder.CombineRows,
                        joinPart[0].Comparer))
                .Aggregate((result, joinResult) => result.Union(joinResult, RowIdComparer.Default));
        }
    }
}