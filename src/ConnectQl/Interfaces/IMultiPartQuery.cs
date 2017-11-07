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
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// The InternalQuery interface.
    /// </summary>
    internal interface IMultiPartQuery
    {
        /// <summary>
        /// Gets the filter expression.
        /// </summary>
        Expression FilterExpression { get; }

        /// <summary>
        /// Gets the order by expressions.
        /// </summary>
        IEnumerable<IOrderByExpression> OrderByExpressions { get; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        IEnumerable<IField> Fields { get; }

        /// <summary>
        /// Gets the wildcard aliases.
        /// </summary>
        IEnumerable<string> WildcardAliases { get; }

        /// <summary>
        /// Gets the number of records to retrieve. When this is <c>null</c>, all records are retrieved.
        /// </summary>
        int? Count { get; }
    }
}