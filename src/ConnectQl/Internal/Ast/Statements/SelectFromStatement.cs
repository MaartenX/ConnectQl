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
    using System.Diagnostics;

    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Sources;
    using ConnectQl.Internal.Ast.Visitors;

    using JetBrains.Annotations;

    /// <summary>
    /// The select from statement.
    /// </summary>
    internal class SelectFromStatement : SelectStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectFromStatement"/> class.
        /// </summary>
        /// <param name="expressions">
        /// The expressions.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="where">
        /// The where.
        /// </param>
        /// <param name="groupings">
        /// The groupings.
        /// </param>
        /// <param name="having">
        /// The having.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        public SelectFromStatement(ReadOnlyCollection<AliasedConnectQlExpression> expressions, SourceBase source, ConnectQlExpressionBase where, ReadOnlyCollection<ConnectQlExpressionBase> groupings, ConnectQlExpressionBase having, ReadOnlyCollection<OrderByConnectQlExpression> orders)
        {
            this.Expressions = expressions;
            this.Source = source;
            this.Where = where;
            this.Groupings = groupings;
            this.Orders = orders;
            this.Having = having;
        }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IEnumerable<Node> Children
        {
            get
            {
                foreach (var expression in this.Expressions)
                {
                    yield return expression;
                }

                if (this.Source != null)
                {
                    yield return this.Source;
                }

                if (this.Where != null)
                {
                    yield return this.Where;
                }

                foreach (var grouping in this.Groupings)
                {
                    yield return grouping;
                }

                if (this.Having != null)
                {
                    yield return this.Having;
                }

                foreach (var order in this.Orders)
                {
                    yield return order;
                }
            }
        }

        /// <summary>
        /// Gets the expressions.
        /// </summary>
        public ReadOnlyCollection<AliasedConnectQlExpression> Expressions { get; }

        /// <summary>
        /// Gets the groupings.
        /// </summary>
        public ReadOnlyCollection<ConnectQlExpressionBase> Groupings { get; }

        /// <summary>
        /// Gets the having.
        /// </summary>
        public ConnectQlExpressionBase Having { get; }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        public ReadOnlyCollection<OrderByConnectQlExpression> Orders { get; }

        /// <summary>
        /// Gets the sources.
        /// </summary>
        public SourceBase Source { get; }

        /// <summary>
        /// Gets the where.
        /// </summary>
        public ConnectQlExpressionBase Where { get; }

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
            return visitor.VisitSelectFromStatement(this);
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
            var source = visitor.Visit(this.Source);
            var where = visitor.Visit(this.Where);
            var groupings = visitor.Visit(this.Groupings);
            var having = visitor.Visit(this.Having);
            var expressions = visitor.Visit(this.Expressions);
            var orders = visitor.Visit(this.Orders);

            return expressions != this.Expressions || source != this.Source || where != this.Where ||
                   groupings != this.Groupings || having != this.Having || orders != this.Orders
                       ? new SelectFromStatement(expressions, source, where, groupings, having, orders)
                       : this;
        }
    }
}