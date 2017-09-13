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
    using System;
    using System.Collections.Generic;

    using ConnectQl.Internal.Ast.Visitors;

    /// <summary>
    /// The binary expression.
    /// </summary>
    internal class BinarySqlExpression : SqlExpressionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySqlExpression"/> class.
        /// </summary>
        /// <param name="first">
        /// The first operand.
        /// </param>
        /// <param name="op">
        /// The operator.
        /// </param>
        /// <param name="second">
        /// The second operand.
        /// </param>
        public BinarySqlExpression(SqlExpressionBase first, string op, SqlExpressionBase second)
        {
            this.First = first;
            this.Op = op;
            this.Second = second;
        }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public override IEnumerable<Node> Children
        {
            get
            {
                yield return this.First;
                yield return this.Second;
            }
        }

        /// <summary>
        /// Gets the first operand.
        /// </summary>
        public SqlExpressionBase First { get; }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        public string Op { get; }

        /// <summary>
        /// Gets the second operand.
        /// </summary>
        public SqlExpressionBase Second { get; }

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
            var other = obj as BinarySqlExpression;

            return other != null && object.Equals(this.First, other.First) && string.Equals(this.Op, other.Op, StringComparison.OrdinalIgnoreCase) && object.Equals(this.Second, other.Second);
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
                return ((((this.First?.GetHashCode() ?? 0) * 397) ^ (this.Op != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Op) : 0)) * 397) ^ (this.Second?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString() => $"({this.First} {this.Op.ToUpperInvariant()} {this.Second})";

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
            return visitor.VisitBinarySqlExpression(this);
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
            var first = visitor.Visit(this.First);
            var second = visitor.Visit(this.Second);

            return first != this.First || second != this.Second ? new BinarySqlExpression(first, this.Op, second) : this;
        }
    }
}