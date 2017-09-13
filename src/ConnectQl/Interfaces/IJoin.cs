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
    using System.Linq.Expressions;

    /// <summary>
    /// Specifies a join with a data source.
    /// </summary>
    public interface IJoin
    {
        /// <summary>
        /// Gets the source to join with.
        /// </summary>
        IAliasedSource JoinWith { get; }

        /// <summary>
        /// Gets the type of the join.
        /// </summary>
        JoinType Type { get; }

        /// <summary>
        /// Gets the expression to use when joining.
        /// </summary>
        Expression JoinExpresion { get; }

        /// <summary>
        /// Gets the expression for the specified context. In this expressions, variables are evaluated, and the
        /// epression is simplified as much as possible.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>The <see cref="Expression"/> that is used to join.</returns>
        Expression GetJoin(IExecutionContext context);
    }
}