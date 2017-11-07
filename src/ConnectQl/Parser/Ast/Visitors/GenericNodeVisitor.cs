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

namespace ConnectQl.Parser.Ast.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    using Expressions;

    using JetBrains.Annotations;

    using Sources;

    using Statements;

    using Targets;

    /// <summary>
    /// The generic node visitor.
    /// </summary>
    internal class GenericNodeVisitor : NodeVisitor
    {
        /// <summary>
        /// The implementations.
        /// </summary>
        private readonly Dictionary<Type, Func<Node, Node>> implementations = new Dictionary<Type, Func<Node, Node>>();

        /// <summary>
        /// Adds a lambda to handle a node.
        /// </summary>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <typeparam name="T">
        /// The type of the node to add a handler for.
        /// </typeparam>
        /// <returns>
        /// This visitor.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public GenericNodeVisitor Add<T>(Action<T> implementation)
            where T : Node
        {
            this.implementations[typeof(T)] = node =>
                {
                    implementation((T)node);

                    return null;
                };

            return this;
        }

        /// <summary>
        /// Adds a lambda to handle a node.
        /// </summary>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <typeparam name="T">
        /// The type of the node to add a handler for.
        /// </typeparam>
        /// <returns>
        /// This visitor.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public GenericNodeVisitor Add<T>(Func<T, Node> implementation)
            where T : Node
        {
            this.implementations[typeof(T)] = node => implementation((T)node);

            return this;
        }

        /// <summary>
        /// Adds a lambda to handle a node.
        /// </summary>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <typeparam name="T">
        /// The type of the node to add a handler for.
        /// </typeparam>
        /// <returns>
        /// This visitor.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public GenericNodeVisitor Add<T>(Func<GenericNodeVisitor, T, Node> implementation)
            where T : Node
        {
            this.implementations[typeof(T)] = node => implementation(this, (T)node);

            return this;
        }

        /// <summary>
        /// Visits the node and ensures the result is of type <typeparamref name="T"/>. When node is <c>null</c>, returns
        ///     <c>null</c>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the result.
        /// </typeparam>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <typeparamref name="T"/>.
        /// </returns>
        protected internal override T Visit<T>(T node)
        {
            return (this.VisitImplementation(node) ?? base.Visit(node)) as T;
        }

        /// <summary>
        /// Visits a <see cref="AliasedConnectQlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitAliasedSqlExpression(AliasedConnectQlExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitAliasedSqlExpression(node);
        }

        /// <summary>
        /// Visits a <see cref="ApplySource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitApplySource(ApplySource node)
        {
            return this.VisitImplementation(node) ?? base.VisitApplySource(node);
        }

        /// <summary>
        /// Visits a <see cref="BinaryConnectQlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitBinarySqlExpression(BinaryConnectQlExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitBinarySqlExpression(node);
        }

        /// <summary>
        /// Visits a <see cref="Block"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitBlock(Block node)
        {
            return this.VisitImplementation(node) ?? base.VisitBlock(node);
        }

        /// <summary>
        /// Visits a <see cref="ConstConnectQlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitConstSqlExpression(ConstConnectQlExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitConstSqlExpression(node);
        }

        /// <summary>
        /// Visits a <see cref="DeclareJobStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitDeclareJobStatement(DeclareJobStatement node)
        {
            return this.VisitImplementation(node) ?? base.VisitDeclareJobStatement(node);
        }

        /// <summary>
        /// Visits a declare statement.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitDeclareStatement(DeclareStatement node)
        {
            return this.VisitImplementation(node) ?? base.VisitDeclareStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="FieldReferenceConnectQlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitFieldReferenceSqlExpression(FieldReferenceConnectQlExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitFieldReferenceSqlExpression(node);
        }

        /// <summary>
        /// Visits a <see cref="FunctionCallConnectQlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitFunctionCallSqlExpression(FunctionCallConnectQlExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitFunctionCallSqlExpression(node);
        }

        /// <summary>
        /// Visits a <see cref="FunctionSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitFunctionSource(FunctionSource node)
        {
            return this.VisitImplementation(node) ?? base.VisitFunctionSource(node);
        }

        /// <summary>
        /// Visits a <see cref="FunctionTarget"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitFunctionTarget(FunctionTarget node)
        {
            return this.VisitImplementation(node) ?? base.VisitFunctionTarget(node);
        }

        /// <summary>
        /// Visits a <see cref="ImportPluginStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitImportPluginStatement(ImportPluginStatement node)
        {
            return this.VisitImplementation(node) ?? base.VisitImportPluginStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="ImportStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitImportStatement(ImportStatement node)
        {
            return this.VisitImplementation(node) ?? base.VisitImportStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="InsertStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitInsertStatement(InsertStatement node)
        {
            return this.VisitImplementation(node) ?? base.VisitInsertStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="JoinSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitJoinSource(JoinSource node)
        {
            return this.VisitImplementation(node) ?? base.VisitJoinSource(node);
        }

        /// <summary>
        /// Visits a <see cref="OrderByConnectQlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitOrderBySqlExpression(OrderByConnectQlExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitOrderBySqlExpression(node);
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
            return this.VisitImplementation(node) ?? base.VisitSelectFromStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="SelectSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitSelectSource(SelectSource node)
        {
            return this.VisitImplementation(node) ?? base.VisitSelectSource(node);
        }

        /// <summary>
        /// Visits a <see cref="SelectUnionStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitSelectUnionStatement(SelectUnionStatement node)
        {
            return this.VisitImplementation(node) ?? base.VisitSelectUnionStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="Trigger"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitTrigger(Trigger node)
        {
            return this.VisitImplementation(node) ?? base.VisitTrigger(node);
        }

        /// <summary>
        /// Visits a <see cref="TriggerStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitTriggerStatement(TriggerStatement node)
        {
            return this.VisitImplementation(node) ?? base.VisitTriggerStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="UnaryConnectQlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitUnarySqlExpression(UnaryConnectQlExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitUnarySqlExpression(node);
        }

        /// <summary>
        /// Visits a <see cref="UseStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitUseStatement(UseStatement node)
        {
            return this.VisitImplementation(node) ?? base.VisitUseStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="VariableDeclaration"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitVariableDeclaration(VariableDeclaration node)
        {
            return this.VisitImplementation(node) ?? base.VisitVariableDeclaration(node);
        }

        /// <summary>
        /// Visits a <see cref="VariableSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitVariableSource(VariableSource node)
        {
            return this.VisitImplementation(node) ?? base.VisitVariableSource(node);
        }

        /// <summary>
        /// Visits a <see cref="VariableConnectQlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitVariableSqlExpression(VariableConnectQlExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitVariableSqlExpression(node);
        }

        /// <summary>
        /// Visits a <see cref="VariableTarget"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitVariableTarget(VariableTarget node)
        {
            return this.VisitImplementation(node) ?? base.VisitVariableTarget(node);
        }

        /// <summary>
        /// Visits a <see cref="WildcardConnectQlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitWildCardSqlExpression(WildcardConnectQlExpression node)
        {
            return this.VisitImplementation(node) ?? base.VisitWildCardSqlExpression(node);
        }

        /// <summary>
        /// Visits the specified node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <typeparam name="T">
        /// The type of the expression.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "C# 7 features not yet supported by StyleCop.")]
        [CanBeNull]
        private Node VisitImplementation<T>(T node)
            where T : Node
        {
            return this.implementations.TryGetValue(typeof(T), out var implementation) ? implementation(node) : null;
        }

        /// <summary>
        /// Visits the specified node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [CanBeNull]
        private Node VisitImplementation([CanBeNull] Node node)
        {
            return node == null ? null : this.VisitImplementation<Node>(node);
        }
    }
}
