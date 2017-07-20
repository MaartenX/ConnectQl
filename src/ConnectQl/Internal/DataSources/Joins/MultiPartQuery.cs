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
    using System.Linq.Expressions;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Query;

    /// <summary>
    /// The multi part query.
    /// </summary>
    internal class MultiPartQuery : IMultiPartQuery
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        public IEnumerable<IField> Fields { get; set; }

        /// <summary>
        /// Gets or sets the filter expression.
        /// </summary>
        public Expression FilterExpression { get; set; }

        /// <summary>
        /// Gets or sets the order by expressions.
        /// </summary>
        public IEnumerable<IOrderByExpression> OrderByExpressions { get; set; } = new OrderByExpression[0];

        /// <summary>
        /// Gets or sets the wildcard aliases.
        /// </summary>
        public IEnumerable<string> WildcardAliases { get; set; } = new string[0];
    }
}