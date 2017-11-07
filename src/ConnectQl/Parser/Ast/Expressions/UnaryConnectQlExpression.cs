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

namespace ConnectQl.Parser.Ast.Expressions
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Visitors;

    /// <summary>
    /// The unary expression.
    /// </summary>
    internal class UnaryConnectQlExpression : ConnectQlExpressionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryConnectQlExpression"/> class.
        /// </summary>
        /// <param name="op">
        /// The op.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public UnaryConnectQlExpression([CanBeNull] string op, ConnectQlExpressionBase expression)
        {
            this.Op = op?.ToUpperInvariant();
            this.Expression = expression;
        }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public override IEnumerable<Node> Children
        {
            get
            {
                yield return this.Expression;
            }
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public ConnectQlExpressionBase Expression { get; }

        /// <summary>
        /// Gets the op.
        /// </summary>
        public string Op { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        public override bool Equals(object obj)
        {
            var other = obj as UnaryConnectQlExpression;

            return other != null && string.Equals(this.Op, other.Op, StringComparison.OrdinalIgnoreCase) && object.Equals(this.Expression, other.Expression);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Expression?.GetHashCode() ?? 0) * 397) ^ (this.Op != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Op) : 0);
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NotNull]
        public override string ToString() => $"({this.Op.ToUpperInvariant()} {this.Expression})";

        /// <summary>
        /// Dispatches the visitor to the correct visit-method.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        protected internal override Node Accept([NotNull] NodeVisitor visitor)
        {
            return visitor.VisitUnarySqlExpression(this);
        }

        /// <summary>
        /// Visits the children of this node.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        [NotNull]
        protected internal override Node VisitChildren([NotNull] NodeVisitor visitor)
        {
            var expression = visitor.Visit(this.Expression);

            return expression != this.Expression ? new UnaryConnectQlExpression(this.Op, expression) : this;
        }
    }
}