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

namespace ConnectQl.Parser.Ast.Statements
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Visitors;

    /// <summary>
    /// The select union statement.
    /// </summary>
    internal class SelectUnionStatement : SelectStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectUnionStatement"/> class.
        /// </summary>
        /// <param name="first">
        /// The first.
        /// </param>
        /// <param name="second">
        /// The second.
        /// </param>
        public SelectUnionStatement(SelectStatement first, SelectStatement second)
        {
            this.First = first;
            this.Second = second;
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
            }
        }

        /// <summary>
        /// Gets the first.
        /// </summary>
        public SelectStatement First { get; }

        /// <summary>
        /// Gets the second.
        /// </summary>
        public SelectStatement Second { get; }

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
            return visitor.VisitSelectUnionStatement(this);
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

            return first != this.First || second != this.Second ? new SelectUnionStatement(first, second) : this;
        }
    }
}