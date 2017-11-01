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

namespace ConnectQl.Internal.Ast.Sources
{
    using System.Collections.Generic;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Visitors;

    using JetBrains.Annotations;

    /// <summary>
    /// The inner join source.
    /// </summary>
    internal class JoinSource : SourceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinSource"/> class.
        /// </summary>
        /// <param name="joinType">
        /// The join type.
        /// </param>
        /// <param name="first">
        /// The first.
        /// </param>
        /// <param name="second">
        /// The second.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public JoinSource(JoinType joinType, SourceBase first, SourceBase second, [CanBeNull] ConnectQlExpressionBase expression = null)
        {
            this.JoinType = joinType;
            this.First = first;
            this.Second = second;
            this.Expression = expression;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public override IEnumerable<Node> Children
        {
            get
            {
                yield return this.First;
                yield return this.Second;

                if (this.Expression != null)
                {
                    yield return this.Expression;
                }
            }
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public ConnectQlExpressionBase Expression { get; }

        /// <summary>
        /// Gets the first joined source.
        /// </summary>
        public SourceBase First { get; }

        /// <summary>
        /// Gets the join type.
        /// </summary>
        public JoinType JoinType { get; }

        /// <summary>
        /// Gets the second joined source.
        /// </summary>
        public SourceBase Second { get; }

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
            return visitor.VisitJoinSource(this);
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
            var first = visitor.Visit(this.First);
            var second = visitor.Visit(this.Second);
            var expression = visitor.Visit(this.Expression);

            return first != this.First || second != this.Second || expression != this.Expression
                       ? new JoinSource(this.JoinType, first, second, expression)
                       : this;
        }
    }
}