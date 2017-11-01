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

namespace ConnectQl.Internal.Query
{
    using System.Collections.ObjectModel;

    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Statements;

    /// <summary>
    /// The GroupQuery interface.
    /// </summary>
    internal interface IGroupQuery
    {
        /// <summary>
        /// Gets the expressions.
        /// </summary>
        ReadOnlyCollection<AliasedConnectQlExpression> Expressions { get; }

        /// <summary>
        /// Gets the groupings.
        /// </summary>
        ReadOnlyCollection<string> Groupings { get; }

        /// <summary>
        /// Gets the having.
        /// </summary>
        ConnectQlExpressionBase Having { get; }

        /// <summary>
        /// Gets the row select.
        /// </summary>
        SelectFromStatement RowSelect { get; }

        /// <summary>
        /// Gets the visitor order by.
        /// </summary>
        ReadOnlyCollection<OrderByConnectQlExpression> OrderBy { get; }
    }
}