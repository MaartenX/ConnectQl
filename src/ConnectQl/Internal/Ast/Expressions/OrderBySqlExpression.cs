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

namespace ConnectQl.Internal.Ast.Expressions
{
    using System.Collections.Generic;

    using ConnectQl.Internal.Ast.Visitors;

    /// <summary>
    /// The order by.
    /// </summary>
    internal class OrderBySqlExpression : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBySqlExpression"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="ascending">
        /// <c>true</c> to sort ascending, <c>false</c> otherwise.
        /// </param>
        public OrderBySqlExpression(SqlExpressionBase expression, bool ascending)
        {
            this.Expression = expression;
            this.Ascending = ascending;
        }

        /// <summary>
        /// Gets a value indicating whether ascending.
        /// </summary>
        public bool Ascending { get; }

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
        public SqlExpressionBase Expression { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// True if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        public override bool Equals(object obj)
        {
            var other = obj as OrderBySqlExpression;

            return other != null && other.Ascending == this.Ascending && Equals(other.Expression, this.Expression);
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
                return (this.Ascending.GetHashCode() * 397) ^ (this.Expression?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString() => this.Expression + " " + (this.Ascending ? "ASC" : "DESC");

        /// <summary>
        /// Dispatches the visitor to the correct visit-method.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        protected internal override Node Accept(NodeVisitor visitor)
        {
            return visitor.VisitOrderBySqlExpression(this);
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
        protected internal override Node VisitChildren(NodeVisitor visitor)
        {
            var expression = visitor.Visit(this.Expression);

            return expression != this.Expression
                       ? new OrderBySqlExpression(expression, this.Ascending)
                       : this;
        }
    }
}