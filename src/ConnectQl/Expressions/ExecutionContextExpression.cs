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

namespace ConnectQl.Expressions
{
    using System;
    using System.Linq.Expressions;

    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The execution context expression.
    /// </summary>
    public class ExecutionContextExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContextExpression"/> class.
        /// </summary>
        internal ExecutionContextExpression()
        {
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [NotNull]
        public override Type Type => typeof(IExecutionContext);

        /// <summary>
        /// Gets the node type of this <see cref="T:System.Linq.Expressions.Expression"/>.
        /// </summary>
        /// <returns>
        /// <see cref="ExpressionType.Extension"/>.
        /// </returns>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NotNull]
        public override string ToString() => "ExecutionContext";

        /// <summary>
        /// The visit children.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }
    }
}