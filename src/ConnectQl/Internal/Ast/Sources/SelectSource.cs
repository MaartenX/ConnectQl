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

    using ConnectQl.Internal.Ast.Statements;
    using ConnectQl.Internal.Ast.Visitors;

    /// <summary>
    /// The select source.
    /// </summary>
    internal class SelectSource : SourceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectSource"/> class.
        /// </summary>
        /// <param name="select">
        /// The select.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        public SelectSource(SelectStatement select, string alias)
        {
            this.Select = select;
            this.Alias = alias;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public override IEnumerable<Node> Children
        {
            get
            {
                yield return this.Select;
            }
        }

        /// <summary>
        /// Gets the select.
        /// </summary>
        public SelectStatement Select { get; }

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
            return visitor.VisitSelectSource(this);
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
            var select = visitor.Visit(this.Select);

            return select != this.Select ? new SelectSource(select, this.Alias) : this;
        }
    }
}