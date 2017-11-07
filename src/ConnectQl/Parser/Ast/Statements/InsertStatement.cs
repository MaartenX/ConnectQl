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

    using Targets;

    using Visitors;

    /// <summary>
    /// The insert statement.
    /// </summary>
    internal class InsertStatement : StatementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertStatement"/> class.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="upsert">
        /// True if this is an upsert statement.
        /// </param>
        /// <param name="select">
        /// The select.
        /// </param>
        public InsertStatement(TargetBase target, bool upsert, SelectStatement select)
        {
            this.Upsert = upsert;
            this.Target = target;
            this.Select = select;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public override IEnumerable<Node> Children
        {
            get
            {
                yield return this.Target;
                yield return this.Select;
            }
        }

        /// <summary>
        /// Gets or sets the select.
        /// </summary>
        public SelectStatement Select { get; set; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        public TargetBase Target { get; }

        /// <summary>
        /// Gets a value indicating whether this is an upsert statement.
        /// </summary>
        public bool Upsert { get; }

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
            return visitor.VisitInsertStatement(this);
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
            var target = visitor.Visit(this.Target);
            var select = visitor.Visit(this.Select);

            return select != this.Select || target != this.Target ? new InsertStatement(target, this.Upsert, select) : this;
        }
    }
}