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

namespace ConnectQl.Internal.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Sources;
    using ConnectQl.Internal.Ast.Statements;
    using ConnectQl.Internal.Ast.Visitors;
    using ConnectQl.Internal.Interfaces;

    /// <summary>
    /// Changes classifications for the tokens.
    /// </summary>
    internal class Classifier : NodeVisitor
    {
        /// <summary>
        /// The data.
        /// </summary>
        private readonly INodeDataProvider data;

        /// <summary>
        /// The tokens.
        /// </summary>
        private readonly IList<ConnectQlContext.ClassifiedToken> tokens;

        /// <summary>
        /// Initializes a new instance of the <see cref="Classifier"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        private Classifier(INodeDataProvider data, List<ConnectQlContext.ClassifiedToken> tokens)
        {
            this.data = data;
            this.tokens = tokens;
        }

        /// <summary>
        /// Classifies the tokens.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="block">
        /// The block.
        /// </param>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        /// <returns>
        /// The classified tokens.
        /// </returns>
        public static IReadOnlyCollection<IClassifiedToken> Classify(IValidationContext context, Block block, IList<Token> tokens)
        {
            var tokenList = tokens.Select(t => new ConnectQlContext.ClassifiedToken(t.CharPos, t.CharPos + t.Val.Length, Classification.Comment /* ClassifyToken(t)*/, t.Kind, t.Val)).ToList();
            var classifier = new Classifier(context.NodeData, tokenList);

            classifier.Visit(block);

            return tokenList;
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
        {/*
            if (node != null && this.data.TryGet(node, "Context", out IParserContext context) && this.data.TryGet(node, "ProductionStack", out Parser.Production[] productionStack) && context.Start.TokenIndex != -1)
            {
                var scope = GetScope(productionStack);

                for (var i = context.Start.TokenIndex; i <= context.End.TokenIndex; i++)
                {
                    this.tokens[i].Scope = scope ?? this.tokens[i].Scope;
                }
            }
            */
            return base.Visit(node);
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
            if (node.Source != null)
            {
                var context = this.data.Get<IParserContext>(node, "Context");

                this.tokens[context.Start.TokenIndex].Classification = Classification.Source;
            }

            return base.VisitFieldReferenceSqlExpression(node);
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
            if (this.data.TryGet(node, "Context", out IParserContext context))
            {
                this.tokens[context.Start.TokenIndex].Classification = Classification.Function;
            }

            return base.VisitFunctionCallSqlExpression(node);
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
            if (this.data.TryGet(node, "Context", out IParserContext context))
            {
                this.tokens[context.End.TokenIndex].Classification = Classification.Source;
            }

            return base.VisitFunctionSource(node);
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
            var result = base.VisitSelectFromStatement(node);

            if (this.data.TryGet(node, "Context", out IParserContext context))
            {
                for (var i = context.Start.TokenIndex + 1; i <= context.End.TokenIndex; i++)
                {
                    if ("FROM".Equals(this.tokens[i].Value, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    this.tokens[i].Scope = ClassificationScope.SelectExpression;
                }
            }

            return result;
        }

        /// <summary>
        /// The visit select source.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        protected internal override Node VisitSelectSource(SelectSource node)
        {
            if (this.data.TryGet(node, "Context", out IParserContext context))
            {
                this.tokens[context.End.TokenIndex].Classification = Classification.Source;
            }

            return base.VisitSelectSource(node);
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
        protected internal override Node VisitWildCardSqlExpression(WildcardSqlExpression node)
        {
            if (node.Source != null)
            {
                var context = this.data.Get<IParserContext>(node, "Context");

                this.tokens[context.Start.TokenIndex].Classification = Classification.Source;
            }

            return base.VisitWildCardSqlExpression(node);
        }

        /*
        /// <summary>
        /// The classify token.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Classification"/>.
        /// </returns>
        private static Classification ClassifyToken(Token token)
        {
            switch (token.kind)
            {
                case Parser.Comment0Token:
                case Parser.Comment1Token:
                case Parser.Comment2Token:
                    return Classification.Comment;
                case Parser.IdentifierToken:
                case Parser.BracketedidentifierToken:
                    return Classification.Identifier;
                case Parser.NumberToken:
                    return Classification.Number;
                case Parser.StringToken:
                    return Classification.String;
                case Parser.VariableToken:
                    return Classification.Variable;

                case Parser.UseLiteral:
                case Parser.DefaultLiteral:
                case Parser.ForLiteral:
                case Parser.DeclareLiteral:
                case Parser.JobLiteral:
                case Parser.TriggeredLiteral:
                case Parser.EveryLiteral:
                case Parser.AfterLiteral:
                case Parser.ByLiteral:
                case Parser.OrLiteral:
                case Parser.BeginLiteral:
                case Parser.EndLiteral:
                case Parser.TriggerLiteral:
                case Parser.ImportLiteral:
                case Parser.PluginLiteral:
                case Parser.InsertLiteral:
                case Parser.UpsertLiteral:
                case Parser.IntoLiteral:
                case Parser.UnionLiteral:
                case Parser.SelectLiteral:
                case Parser.FromLiteral:
                case Parser.WhereLiteral:
                case Parser.GroupLiteral:
                case Parser.HavingLiteral:
                case Parser.OrderLiteral:
                case Parser.AscLiteral:
                case Parser.DescLiteral:
                case Parser.JoinLiteral:
                case Parser.InnerLiteral:
                case Parser.LeftLiteral:
                case Parser.OnLiteral:
                case Parser.CrossLiteral:
                case Parser.ApplyLiteral:
                case Parser.OuterLiteral:
                case Parser.SequentialLiteral:
                case Parser.AsLiteral:
                case Parser.AndLiteral:
                case Parser.NotLiteral:
                    return Classification.Keyword;
                case Parser.LeftParenLiteral:
                case Parser.RightParenLiteral:
                    return Classification.Parens;

                case Parser.EqualsLiteral:
                case Parser.CommaLiteral:
                case Parser.GreaterThanLiteral:
                case Parser.GreaterThanEqualsLiteral:
                case Parser.LessThanEqualsLiteral:
                case Parser.LessThanLiteral:
                case Parser.LessThanGreaterThanLiteral:
                case Parser.PlusLiteral:
                case Parser.MinusLiteral:
                case Parser.MulLiteral:
                case Parser.DivLiteral:
                case Parser.ModLiteral:
                case Parser.CircumflexLiteral:
                case Parser.ExclamationMarkLiteral:
                    return Classification.Operator;

                case Parser.TrueLiteral:
                case Parser.FalseLiteral:
                case Parser.NullLiteral:
                    return Classification.Number;

                case Parser.DotLiteral:
                    return Classification.Identifier;
                case Parser.UnknownIdentifierLiteral:
                    return Classification.Unknown;
            }

            return Classification.Unknown;
        }

        /// <summary>
        /// Gets the scope for the production stack.
        /// </summary>
        /// <param name="productions">
        /// The productions.
        /// </param>
        /// <returns>
        /// The <see cref="ClassificationScope"/>.
        /// </returns>
        private static ClassificationScope? GetScope(Parser.Production[] productions)
        {
            foreach (var production in productions)
            {
                switch (production)
                {
                    case Parser.Production.ImportStatement:
                        return ClassificationScope.Import;
                    case Parser.Production.VariableDeclaration:
                        return ClassificationScope.Expression;
                    case Parser.Production.Expression:
                        return ClassificationScope.Expression;
                    case Parser.Production.FunctionName:
                        return ClassificationScope.FunctionName;
                    case Parser.Production.Function:
                        return ClassificationScope.Function;
                    case Parser.Production.Join:
                        return ClassificationScope.Source;
                }
            }

            return null;
        }*/
    }
}