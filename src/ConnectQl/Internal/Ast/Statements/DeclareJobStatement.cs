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

namespace ConnectQl.Internal.Ast.Statements
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using ConnectQl.Internal.Ast.Visitors;

    /// <summary>
    /// The declare job statement.
    /// </summary>
    internal class DeclareJobStatement : StatementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeclareJobStatement"/> class.
        /// </summary>
        /// <param name="name">
        /// The job Uri.
        /// </param>
        /// <param name="statements">
        /// The statements.
        /// </param>
        /// <param name="triggers">
        /// The triggers.
        /// </param>
        public DeclareJobStatement(string name, ReadOnlyCollection<StatementBase> statements, ReadOnlyCollection<Trigger> triggers)
        {
            this.Name = name;
            this.Statements = statements;
            this.Triggers = triggers;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public override IEnumerable<Node> Children
        {
            get
            {
                foreach (var trigger in this.Triggers)
                {
                    yield return trigger;
                }

                foreach (var statement in this.Statements)
                {
                    yield return statement;
                }
            }
        }

        /// <summary>
        /// Gets the job name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the statements.
        /// </summary>
        public ReadOnlyCollection<StatementBase> Statements { get; }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        public ReadOnlyCollection<Trigger> Triggers { get; }

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
            return visitor.VisitDeclareJobStatement(this);
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
            var triggers = visitor.Visit(this.Triggers);
            var statements = visitor.Visit(this.Statements);

            return triggers != this.Triggers || statements != this.Statements ? new DeclareJobStatement(this.Name, statements, triggers) : this;
        }
    }
}