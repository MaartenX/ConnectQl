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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The LEFT SEQUENTIAL JOIN.
    /// </summary>
    internal class LeftSequentialJoin : DataSource
    {
        /// <summary>
        /// Gets the left.
        /// </summary>
        private readonly DataSource left;

        /// <summary>
        /// Gets the right.
        /// </summary>
        private readonly DataSource right;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftSequentialJoin"/> class.
        /// </summary>
        /// <param name="left">
        /// The left data set.
        /// </param>
        /// <param name="right">
        /// The right data set.
        /// </param>
        public LeftSequentialJoin([NotNull] DataSource left, [NotNull] DataSource right)
            : base(new HashSet<string>(left.Aliases.Concat(right.Aliases)))
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Gets the rows for the join.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="multiPartQuery">
        /// The multi part query.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{Row}"/>.
        /// </returns>
        internal override IAsyncEnumerable<Row> GetRows(IInternalExecutionContext context, [NotNull] IMultiPartQuery multiPartQuery)
        {
            var rowBuilder = new RowBuilder();

            //// Build the left part by filtering by parts that contain the fields of the left side.
            var leftQuery = new MultiPartQuery
                                {
                                    Fields = multiPartQuery.Fields.Where(f => this.left.Aliases.Contains(f.SourceAlias)),
                                    WildcardAliases = multiPartQuery.WildcardAliases.Intersect(this.left.Aliases).ToArray(),
                                };

            var rightQuery = new MultiPartQuery
                                 {
                                     Fields = multiPartQuery.Fields.Where(f => this.right.Aliases.Contains(f.SourceAlias)),
                                     WildcardAliases = multiPartQuery.WildcardAliases.Intersect(this.right.Aliases),
                                 };

            return this.left.GetRows(context, leftQuery).ZipAll(this.right.GetRows(context, rightQuery), rowBuilder.CombineRows)
                .Where(multiPartQuery.FilterExpression.GetRowFilter())
                .OrderBy(multiPartQuery.OrderByExpressions)
                .AfterLastElement(count => context.Logger.Verbose($"{this.GetType().Name} returned {count} records."));
        }

        /// <summary>
        /// Gets the descriptors for this data source.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>
        /// All data sources inside this data source.
        /// </returns>
        [ItemNotNull]
        protected internal override async Task<IEnumerable<IDataSourceDescriptor>> GetDataSourceDescriptorsAsync(IExecutionContext context)
        {
            return (await this.left.GetDataSourceDescriptorsAsync(context).ConfigureAwait(false))
                .Concat(await this.right.GetDataSourceDescriptorsAsync(context).ConfigureAwait(false));
        }
    }
}