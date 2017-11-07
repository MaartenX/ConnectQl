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

namespace ConnectQl.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.DataSources;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Intellisense.Protocol;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Parser.Ast;
    using ConnectQl.Parser.Ast.Expressions;
    using ConnectQl.Parser.Ast.Sources;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Parser.Ast.Visitors;
    using ConnectQl.Query;

    using JetBrains.Annotations;

    /// <summary>
    /// The interpreter.
    /// </summary>
    internal class Evaluator : NodeVisitor
    {
        /// <summary>
        /// The parsed script.
        /// </summary>
        private ParsedDocument parsedScript;

        /// <summary>
        /// The statements.
        /// </summary>
        private EvaluationResult statements;

        /// <summary>
        /// Gets the sources.
        /// </summary>
        public IReadOnlyList<SerializableDataSourceDescriptorRange> Sources => this.statements.Sources;

        /// <summary>
        /// Gets the variables.
        /// </summary>
        public IReadOnlyList<SerializableVariableDescriptorRange> Variables => this.statements.Variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="Evaluator"/> class.
        /// </summary>
        /// <param name="parsedScript">
        /// The parsed script.
        /// </param>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        /// <returns>
        /// The <see cref="EvaluationResult"/>.
        /// </returns>
        internal static IEvaluationResult GetIntellisenseData([NotNull] ParsedDocument parsedScript, IReadOnlyList<IClassifiedToken> tokens)
        {
            var evaluator = new Evaluator
                                {
                                    parsedScript = parsedScript,
                                    statements = new EvaluationResult(parsedScript.Context, tokens),
                                };

            foreach (var statement in parsedScript.Root.Statements)
            {
                evaluator.statements.SetActiveStatement(statement);
                evaluator.Visit(statement);
            }

            return evaluator.statements;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.UseStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitUseStatement(UseStatement node)
        {
            try
            {
                ((IInternalExecutionContext)this.statements).RegisterDefault(node.SettingFunction.Name, node.FunctionName, this.Evaluate(node.SettingFunction, out var sideEffects));
            }
            catch
            {
                // Ignore.
            }

            return base.VisitUseStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.VariableDeclaration"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitVariableDeclaration([NotNull] VariableDeclaration node)
        {
            this.statements.SetVariable(node.Name, this.Evaluate(node.Expression, out var sideEffects), !sideEffects);

            return base.VisitVariableDeclaration(node);
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
        protected internal override Node VisitSelectFromStatement([NotNull] SelectFromStatement node)
        {
            var descriptors = this.Evaluate(node.Source, out var hasSideEffects)?.GetDataSourceDescriptorsAsync(this.statements).Result.ToArray();

            if (descriptors != null)
            {
                foreach (var descriptor in descriptors)
                {
                    this.statements.SetSource(descriptor.Alias, descriptor);
                }
            }

            return base.VisitSelectFromStatement(node);
        }

        /// <summary>
        /// Evaluates the <see cref="ConnectQl.Parser.Ast.Expressions.ConnectQlExpressionBase"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression to evaluate.
        /// </param>
        /// <param name="sideEffects">
        /// <c>true</c> if the expression has side effects, <c>false</c> otherwise.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object Evaluate(ConnectQlExpressionBase expression, out bool sideEffects)
        {
            try
            {
                if (this.parsedScript.Context.NodeData.HasSideEffects(expression, variable => this.statements.HasVariableSideEffects(variable)))
                {
                    sideEffects = true;

                    return null;
                }

                sideEffects = false;

                var linqExpression = GenericVisitor.Visit(
                    (ExecutionContextExpression e) => Expression.Constant(this.statements),
                    this.parsedScript.Context.NodeData.ConvertToLinqExpression(expression));

                return Expression.Lambda(linqExpression).Compile().DynamicInvoke();
            }
            catch
            {
                sideEffects = true;

                return null;
            }
        }

        /// <summary>
        /// Evaluates the <see cref="ConnectQl.Parser.Ast.Sources.SourceBase"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression to evaluate.
        /// </param>
        /// <param name="sideEffects">
        /// <c>true</c> if the expression has side effects, <c>false</c> otherwise.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private DataSource Evaluate(SourceBase expression, out bool sideEffects)
        {
            try
            {
                if (this.parsedScript.Context.NodeData.HasSideEffects(expression, variable => this.statements.HasVariableSideEffects(variable)))
                {
                    sideEffects = true;

                    return null;
                }

                sideEffects = false;

                var linqExpression = GenericVisitor.Visit(
                    (ExecutionContextExpression e) => Expression.Constant(this.statements),
                    this.parsedScript.Context.NodeData.ConvertToDataSource(expression));

                return Expression.Lambda<Func<DataSource>>(linqExpression).Compile().Invoke();
            }
            catch
            {
                sideEffects = true;

                return null;
            }
        }
    }
}