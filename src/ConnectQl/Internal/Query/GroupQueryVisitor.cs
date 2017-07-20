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

namespace ConnectQl.Internal.Query
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Statements;
    using ConnectQl.Internal.Ast.Visitors;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Validation;

    /// <summary>
    /// Converts a select query to a group query.
    /// </summary>
    internal class GroupQueryVisitor : NodeVisitor
    {
        /// <summary>
        /// The data.
        /// </summary>
        private readonly INodeDataProvider data;

        /// <summary>
        /// The grouped nodes.
        /// </summary>
        private readonly List<SqlExpressionBase> groupedNodes = new List<SqlExpressionBase>();

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
        private GroupQueryVisitor(SelectFromStatement select, INodeDataProvider data)
        {
            this.data = data;
            this.select = select;
        }

        /// <summary>
        /// Gets the expressions.
        /// </summary>
        public ReadOnlyCollection<AliasedSqlExpression> Expressions { get; private set; }

        /// <summary>
        /// Gets the groupings.
        /// </summary>
        public ReadOnlyCollection<string> Groupings { get; private set; }

        /// <summary>
        /// Gets the having.
        /// </summary>
        public SqlExpressionBase Having { get; private set; }

        /// <summary>
        /// Gets the order by.
        /// </summary>
        public ReadOnlyCollection<OrderBySqlExpression> OrderBy { get; private set; }

        /// <summary>
        /// Gets the group query from the select statement.
        /// </summary>
        /// <param name="select">
        /// The select.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="IGroupQuery"/>.
        /// </returns>
        public static IGroupQuery GetGroupQuery(SelectFromStatement select, INodeDataProvider data)
        {
            var visitor = new GroupQueryVisitor(select, data);

            return new GroupQuery(visitor.Visit(select), visitor.Expressions, visitor.Groupings, visitor.Having, visitor.OrderBy);
        }

        /// <summary>
        /// Visits a <see cref="T:ConnectQl.Internal.Ast.Expressions.AliasedSqlExpression" />.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitAliasedSqlExpression(AliasedSqlExpression node)
        {
            if (this.data.GetScope(node.Expression) == NodeScope.Group)
            {
            }

            return base.VisitAliasedSqlExpression(node);
        }

        /// <summary>
        /// Visits a <see cref="BinarySqlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitBinarySqlExpression(BinarySqlExpression node)
        {
            return this.CheckForGroups(node, n => base.VisitBinarySqlExpression(n));
        }

        /// <summary>
        /// Visits a <see cref="ConstSqlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitConstSqlExpression(ConstSqlExpression node)
        {
            return this.CheckForGroups(node, n => base.VisitConstSqlExpression(n));
        }

        /// <summary>
        /// Visits a <see cref="FieldReferenceSqlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitFieldReferenceSqlExpression(FieldReferenceSqlExpression node)
        {
            return this.CheckForGroups(node, n => base.VisitFieldReferenceSqlExpression(n));
        }

        /// <summary>
        /// Visits a <see cref="FunctionCallSqlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitFunctionCallSqlExpression(FunctionCallSqlExpression node)
        {
            return this.CheckForGroups(node, n => base.VisitFunctionCallSqlExpression(n));
        }

        /// <summary>
        /// Visits a <see cref="SelectFromStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitSelectFromStatement(SelectFromStatement node)
        {
            this.Having = this.Visit(node.Having);
            this.Expressions = this.Visit(node.Expressions);
            this.OrderBy = this.Visit(node.Orders);

            this.visitingGroupings++;

            this.Groupings = new ReadOnlyCollection<string>(this.Visit(node.Groupings).Cast<FieldReferenceSqlExpression>().Select(f => f.Name).ToArray());

            this.visitingGroupings--;

            return new SelectFromStatement(new ReadOnlyCollection<AliasedSqlExpression>(this.groupedNodes.Select((gn, idx) => new AliasedSqlExpression(gn, $"group{idx}")).ToArray()), node.Source, node.Where, new ReadOnlyCollection<SqlExpressionBase>(new List<SqlExpressionBase>()), null, new ReadOnlyCollection<OrderBySqlExpression>(new List<OrderBySqlExpression>()));
        }

        /// <summary>
        /// Visits a <see cref="UnarySqlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitUnarySqlExpression(UnarySqlExpression node)
        {
            return this.CheckForGroups(node, n => base.VisitUnarySqlExpression(n));
        }

        /// <summary>
        /// Visits a <see cref="VariableSqlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitVariableSqlExpression(VariableSqlExpression node)
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
            where T : SqlExpressionBase
        {
            var isGrouping = this.select.Groupings.Contains(node);

            if ((this.data.GetScope(node) == NodeScope.Row) || isGrouping)
            {
                if (this.groupedNodes.IndexOf(node) == -1)
                {
                    this.groupedNodes.Add(node);
                }

                var field = new FieldReferenceSqlExpression($"group{this.groupedNodes.IndexOf(node)}");

                if (!isGrouping || this.visitingGroupings > 0)
                {
                    return field;
                }

                var fields = new List<SqlExpressionBase>
                                 {
                                     field,
                                 };

                var result = new FunctionCallSqlExpression("$GROUP_FIRST", new ReadOnlyCollection<SqlExpressionBase>(fields));

                this.data.MarkAsGroupFunction(result);
                this.data.SetScope(node, NodeScope.Group);
                this.data.SetFunction(result, new FunctionDescriptor(result.Name, false, (Expression<Func<IAsyncEnumerable<object>, Task<object>>>)(objs => objs.FirstAsync())));

                return result;
            }

            if (this.data.GetScope(node) == NodeScope.Group && node is FunctionCallSqlExpression)
            {
                this.visitingGroupings++;
            }

            var nodeResult = baseCall(node);

            if (this.data.GetScope(node) == NodeScope.Group && node is FunctionCallSqlExpression)
            {
                this.visitingGroupings--;
            }

            if (!object.ReferenceEquals(node, nodeResult))
            {
                this.data.CopyValues(node, nodeResult);
            }

            return nodeResult;
        }

        /// <summary>
        /// The group query.
        /// </summary>
        private class GroupQuery : IGroupQuery
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GroupQuery"/> class.
            /// </summary>
            /// <param name="rowSelect">
            /// The row select.
            /// </param>
            /// <param name="expressions">
            /// The expressions.
            /// </param>
            /// <param name="groupings">
            /// The groupings.
            /// </param>
            /// <param name="having">
            /// The having.
            /// </param>
            /// <param name="orderBy">
            /// The visitor order by.
            /// </param>
            public GroupQuery(SelectFromStatement rowSelect, ReadOnlyCollection<AliasedSqlExpression> expressions, ReadOnlyCollection<string> groupings, SqlExpressionBase having, ReadOnlyCollection<OrderBySqlExpression> orderBy)
            {
                this.RowSelect = rowSelect;
                this.Expressions = expressions;
                this.Groupings = groupings;
                this.Having = having;
                this.OrderBy = orderBy;
            }

            /// <summary>
            /// Gets the expressions.
            /// </summary>
            public ReadOnlyCollection<AliasedSqlExpression> Expressions { get; }

            /// <summary>
            /// Gets the groupings.
            /// </summary>
            public ReadOnlyCollection<string> Groupings { get; }

            /// <summary>
            /// Gets the having.
            /// </summary>
            public SqlExpressionBase Having { get; }

            /// <summary>
            /// Gets the visitor order by.
            /// </summary>
            public ReadOnlyCollection<OrderBySqlExpression> OrderBy { get; }

            /// <summary>
            /// Gets the row select.
            /// </summary>
            public SelectFromStatement RowSelect { get; }
        }
    }
}