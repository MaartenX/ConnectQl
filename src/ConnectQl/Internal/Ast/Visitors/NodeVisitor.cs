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

namespace ConnectQl.Internal.Ast.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Sources;
    using ConnectQl.Internal.Ast.Statements;
    using ConnectQl.Internal.Ast.Targets;

    /// <summary>
    /// The node visitor.
    /// </summary>
    internal abstract class NodeVisitor
    {
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
        [DebuggerHidden]
        protected internal virtual T Visit<T>(T node)
            where T : Node
        {
            if (node == null)
            {
                return null;
            }

            var result = (T)node.Accept(this);

            if (result == null)
            {
                throw new InvalidOperationException($"Node was not a valid {typeof(T).Name} node.");
            }

            return result;
        }

        /// <summary>
        /// Visits a collection of nodes.
        /// </summary>
        /// <param name="list">
        /// The list to visit.
        /// </param>
        /// <typeparam name="T">
        /// The type of the nodes.
        /// </typeparam>
        /// <returns>
        /// The list, or a copy of the list if any of the elements was changed.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a visitor doesn't return the correct type.
        /// </exception>
        protected internal virtual ReadOnlyCollection<T> Visit<T>(ReadOnlyCollection<T> list)
            where T : Node
        {
            List<T> result = null;

            for (var i = 0; i < list.Count; i++)
            {
                var item = this.Visit(list[i]);

                if (item == null && list[i] != null)
                {
                    throw new InvalidOperationException($"Item {i} was not a {typeof(T).Name}.");
                }

                if (result == null && item != list[i])
                {
                    result = new List<T>(list.Count);

                    for (var j = 0; j < i; j++)
                    {
                        result.Add(list[j]);
                    }
                }

                result?.Add(item);
            }

            return result == null ? list : new ReadOnlyCollection<T>(result);
        }

        /// <summary>
        /// Visits a <see cref="AliasedSqlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal virtual Node VisitAliasedSqlExpression(AliasedSqlExpression node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitApplySource(ApplySource node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitBinarySqlExpression(BinarySqlExpression node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitBlock(Block node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitConstSqlExpression(ConstSqlExpression node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitDeclareJobStatement(DeclareJobStatement node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitDeclareStatement(DeclareStatement node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitFieldReferenceSqlExpression(FieldReferenceSqlExpression node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitFunctionCallSqlExpression(FunctionCallSqlExpression node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitFunctionSource(FunctionSource node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitFunctionTarget(FunctionTarget node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitImportPluginStatement(ImportPluginStatement node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitImportStatement(ImportStatement node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitInsertStatement(InsertStatement node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitJoinSource(JoinSource node)
        {
            return node.VisitChildren(this);
        }

        /// <summary>
        /// Visits a <see cref="OrderBySqlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal virtual Node VisitOrderBySqlExpression(OrderBySqlExpression node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitSelectFromStatement(SelectFromStatement node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitSelectSource(SelectSource node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitSelectUnionStatement(SelectUnionStatement node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitTrigger(Trigger node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitTriggerStatement(TriggerStatement node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitUnarySqlExpression(UnarySqlExpression node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitUseStatement(UseStatement node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitVariableDeclaration(VariableDeclaration node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitVariableSource(VariableSource node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitVariableSqlExpression(VariableSqlExpression node)
        {
            return node.VisitChildren(this);
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
        protected internal virtual Node VisitVariableTarget(VariableTarget node)
        {
            return node.VisitChildren(this);
        }

        /// <summary>
        /// Visits a <see cref="WildcardSqlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal virtual Node VisitWildCardSqlExpression(WildcardSqlExpression node)
        {
            return node.VisitChildren(this);
        }
    }
}