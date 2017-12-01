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

namespace ConnectQl.Query
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.ExtensionMethods;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Parser.Ast;
    using ConnectQl.Parser.Ast.Expressions;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Parser.Ast.Visitors;

    using JetBrains.Annotations;

    /// <summary>
    /// The group query.
    /// </summary>
    internal class GroupQuery : IGroupQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupQuery"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="select">
        /// The select.
        /// </param>
        public GroupQuery(INodeDataProvider data, SelectFromStatement select)
        {
            var visitor = new GroupQueryVisitor(select, data);

            this.InnerSelect = visitor.Visit(select);

            var expressions = new List<AliasedConnectQlExpression>(visitor.Expressions);

            if (visitor.Having != null)
            {
                expressions.Add(new AliasedConnectQlExpression(visitor.Having, "$having"));
            }

            if (visitor.OrderBy.Any())
            {
                expressions.AddRange(visitor.OrderBy.Select((o, i) => new AliasedConnectQlExpression(o.Expression, $"order{i}")));
            }

            this.Expressions = new ReadOnlyCollection<AliasedConnectQlExpression>(expressions);
            this.Groupings = visitor.Groupings;
            this.Having = visitor.Having;
            this.OrderBy = visitor.OrderBy;
        }
        
        /// <summary>
        /// Gets the expressions.
        /// </summary>
        public ReadOnlyCollection<AliasedConnectQlExpression> Expressions { get; }

        /// <summary>
        /// Gets the groupings.
        /// </summary>
        public ReadOnlyCollection<string> Groupings { get; }

        /// <summary>
        /// Gets the having.
        /// </summary>
        public ConnectQlExpressionBase Having { get; }

        /// <summary>
        /// Gets the visitor order by.
        /// </summary>
        public ReadOnlyCollection<OrderByConnectQlExpression> OrderBy { get; }

        /// <summary>
        /// Gets the row select.
        /// </summary>
        public SelectFromStatement InnerSelect { get; }

        /// <summary>
        /// Used to calculate the <see cref="GroupQuery"/>.
        /// </summary>
        private class GroupQueryVisitor : NodeVisitor
        {
            /// <summary>
            /// The data.
            /// </summary>
            private readonly INodeDataProvider data;

            /// <summary>
            /// The grouped nodes.
            /// </summary>
            private readonly List<ConnectQlExpressionBase> groupedNodes = new List<ConnectQlExpressionBase>();

            /// <summary>
            /// The select.
            /// </summary>
            private readonly SelectFromStatement select;

            /// <summary>
            /// The visiting groupings.
            /// </summary>
            private int visitingGroupings;

            /// <summary>
            /// Initializes a new instance of the <see cref="GroupQueryVisitor"/> class.
            /// </summary>
            /// <param name="select">
            /// The select.
            /// </param>
            /// <param name="data">
            /// The data.
            /// </param>
            public GroupQueryVisitor(SelectFromStatement select, INodeDataProvider data)
            {
                this.data = data;
                this.select = select;
            }

            /// <summary>
            /// Gets the expressions.
            /// </summary>
            public ReadOnlyCollection<AliasedConnectQlExpression> Expressions { get; private set; }

            /// <summary>
            /// Gets the groupings.
            /// </summary>
            public ReadOnlyCollection<string> Groupings { get; private set; }

            /// <summary>
            /// Gets the having.
            /// </summary>
            public ConnectQlExpressionBase Having { get; private set; }

            /// <summary>
            /// Gets the order by.
            /// </summary>
            public ReadOnlyCollection<OrderByConnectQlExpression> OrderBy { get; private set; }

            /// <summary>
            /// Visits a <see cref="T:ConnectQl.Parser.Ast.Expressions.AliasedConnectQlExpression" />.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitAliasedSqlExpression(AliasedConnectQlExpression node)
            {
                if (this.data.GetScope(node.Expression) == NodeScope.Group)
                {
                }

                return base.VisitAliasedSqlExpression(node);
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.BinaryConnectQlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitBinarySqlExpression(BinaryConnectQlExpression node)
            {
                return this.CheckForGroups(node, n => base.VisitBinarySqlExpression(n));
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.ConstConnectQlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitConstSqlExpression(ConstConnectQlExpression node)
            {
                return this.CheckForGroups(node, n => base.VisitConstSqlExpression(n));
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.FieldReferenceConnectQlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitFieldReferenceSqlExpression(FieldReferenceConnectQlExpression node)
            {
                return this.CheckForGroups(node, n => base.VisitFieldReferenceSqlExpression(n));
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.FunctionCallConnectQlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitFunctionCallSqlExpression(FunctionCallConnectQlExpression node)
            {
                return this.CheckForGroups(node, n => base.VisitFunctionCallSqlExpression(n));
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.SelectFromStatement"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitSelectFromStatement(SelectFromStatement node)
            {
                this.Having = this.Visit(node.Having);
                this.Expressions = this.Visit(node.Expressions);
                this.OrderBy = this.Visit(node.Orders);

                this.visitingGroupings++;

                this.Groupings = new ReadOnlyCollection<string>(this.Visit(node.Groupings).Cast<FieldReferenceConnectQlExpression>().Select(f => f.Name).ToArray());

                this.visitingGroupings--;

                return new SelectFromStatement(new ReadOnlyCollection<AliasedConnectQlExpression>(this.groupedNodes.Select((gn, idx) => new AliasedConnectQlExpression(gn, $"group{idx}")).ToArray()), node.Source, node.Where, new ReadOnlyCollection<ConnectQlExpressionBase>(new List<ConnectQlExpressionBase>()), null, new ReadOnlyCollection<OrderByConnectQlExpression>(new List<OrderByConnectQlExpression>()));
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.UnaryConnectQlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitUnarySqlExpression(UnaryConnectQlExpression node)
            {
                return this.CheckForGroups(node, n => base.VisitUnarySqlExpression(n));
            }

            /// <summary>
            /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.VariableConnectQlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitVariableSqlExpression(VariableConnectQlExpression node)
            {
                return this.CheckForGroups(node, n => base.VisitVariableSqlExpression(n));
            }

            /// <summary>
            /// Checks if the node is a grouped node, and adds it to the list of grouped nodes if so.
            /// </summary>
            /// <typeparam name="T">
            /// The type of the node.
            /// </typeparam>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <param name="baseCall">
            /// The method to call when the expression is not a grouped node.
            /// </param>
            /// <returns>
            /// The node or the result of the <paramref name="baseCall"/>.
            /// </returns>
            private Node CheckForGroups<T>(T node, Func<T, Node> baseCall)
                where T : ConnectQlExpressionBase
            {
                var isGrouping = this.select.Groupings.Contains(node);

                if (this.data.GetScope(node) == NodeScope.Row || isGrouping)
                {
                    if (this.groupedNodes.IndexOf(node) == -1)
                    {
                        this.groupedNodes.Add(node);
                    }

                    var field = new FieldReferenceConnectQlExpression($"group{this.groupedNodes.IndexOf(node)}");

                    if (!isGrouping || this.visitingGroupings > 0)
                    {
                        return field;
                    }

                    var fields = new List<ConnectQlExpressionBase>
                                 {
                                     field,
                                 };

                    var result = new FunctionCallConnectQlExpression("$GROUP_FIRST", new ReadOnlyCollection<ConnectQlExpressionBase>(fields));

                    this.data.SetScope(node, NodeScope.Group);
                    this.data.SetFunction(result, new FunctionDescriptor(result.Name, false, (Expression<Func<IAsyncEnumerable<object>, Task<object>>>)(objs => objs.FirstAsync())));

                    return result;
                }

                if (this.data.GetScope(node) == NodeScope.Group && node is FunctionCallConnectQlExpression)
                {
                    this.visitingGroupings++;
                }

                var nodeResult = baseCall(node);

                if (this.data.GetScope(node) == NodeScope.Group && node is FunctionCallConnectQlExpression)
                {
                    this.visitingGroupings--;
                }

                if (!ReferenceEquals(node, nodeResult))
                {
                    this.data.CopyValues(node, nodeResult);
                }

                return nodeResult;
            }
        }
    }
}