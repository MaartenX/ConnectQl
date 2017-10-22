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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Sources;
    using ConnectQl.Internal.Ast.Targets;
    using ConnectQl.Internal.Ast.Visitors;
    using ConnectQl.Internal.DataSources;
    using ConnectQl.Internal.DataSources.Joins;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Validation;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    using JoinSource = Ast.Sources.JoinSource;

    /// <summary>
    ///     The node data provider data source converter.
    /// </summary>
    internal static class NodeDataProviderDataSourceConverter
    {
        /// <summary>
        ///     The <see cref="Row.Get{T}" /> method.
        /// </summary>
        private static readonly MethodInfo GetMethod = typeof(Row).GetGenericMethod(nameof(Row.Get), typeof(string));

        /// <summary>
        /// The convert to data source.
        /// </summary>
        /// <param name="dataProvider">
        /// The data Provider.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression ConvertToRows(this INodeDataProvider dataProvider, Expression expression)
        {
            return Expression.Call(
                Evaluator.CreateDataSource(expression, null),
                typeof(DataSource).GetTypeInfo().GetDeclaredMethod(nameof(DataSource.GetRows)),
                Expression.Convert(CustomExpression.ExecutionContext(), typeof(IInternalExecutionContext)),
                Expression.Constant(new MultiPartQuery
                                        {
                                            WildcardAliases = new string[] { null },
                                        }));
        }

        /// <summary>
        ///     Converts the <paramref name="source" /> to a lambda returning a <see cref="DataSource" />.
        /// </summary>
        /// <param name="dataProvider">
        ///     The data provider.
        /// </param>
        /// <param name="source">
        ///     The source to convert.
        /// </param>
        /// <param name="messages">
        ///     The messages.
        /// </param>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        public static Expression ConvertToDataSource(this INodeDataProvider dataProvider, SourceBase source, [CanBeNull] IMessageWriter messages = null)
        {
            new Evaluator(dataProvider, messages ?? new MessageWriter("null")).Visit(source);

            var factoryExpression = dataProvider.GetFactoryExpression(source);

            return factoryExpression.Type == typeof(DataSource) ? factoryExpression : Expression.Convert(factoryExpression, typeof(DataSource));
        }

        /// <summary>
        ///     Converts the <paramref name="target" /> to a lambda returning a <see cref="DataTarget" />.
        /// </summary>
        /// <param name="dataProvider">
        ///     The data provider.
        /// </param>
        /// <param name="target">
        ///     The target to convert.
        /// </param>
        /// <param name="messages">
        ///     The messages.
        /// </param>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        public static Expression ConvertToDataTarget(this INodeDataProvider dataProvider, TargetBase target, [CanBeNull] IMessageWriter messages = null)
        {
            new Evaluator(dataProvider, messages ?? new MessageWriter("null")).Visit(target);

            return dataProvider.GetFactoryExpression(target);
        }

        /// <summary>
        ///     The evaluator.
        /// </summary>
        private class Evaluator : NodeVisitor
        {
            /// <summary>
            ///     Lambda that registers a data source.
            /// </summary>
            private static readonly Expression<Func<IDataSource, HashSet<string>, DataSource>> CreateDataSourceDefinition
                = (source, alias) => new ExternalDataSource(source, alias);

            /// <summary>
            ///     Lambda that registers a data target.
            /// </summary>
            private static readonly Expression<Func<IDataTarget, DataTarget>> CreateDataTargetDefinition
                = target => new DataTarget(target);

            /// <summary>
            ///     The data.
            /// </summary>
            private readonly INodeDataProvider data;

            /// <summary>
            ///     The messages.
            /// </summary>
            private readonly IMessageWriter messages;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Evaluator" /> class.
            /// </summary>
            /// <param name="data">
            ///     The data.
            /// </param>
            /// <param name="messages">
            ///     The message writer.
            /// </param>
            public Evaluator(INodeDataProvider data, IMessageWriter messages)
            {
                this.data = data;
                this.messages = messages;
            }

            /// <summary>
            ///     Registers the data source with the alias.
            /// </summary>
            /// <param name="expression">
            ///     The expression.
            /// </param>
            /// <param name="alias">
            ///     The alias.
            /// </param>
            /// <returns>
            ///     The <see cref="Expression" />.
            /// </returns>
            internal static Expression CreateDataSource(Expression expression, string alias)
            {
                var aliases = new HashSet<string>
                                  {
                                      alias,
                                  };

                var result = Evaluator.CreateDataSourceDefinition.Body
                    .ReplaceParameter(Evaluator.CreateDataSourceDefinition.Parameters[0], expression)
                    .ReplaceParameter(Evaluator.CreateDataSourceDefinition.Parameters[1], Expression.Constant(aliases));

                return result;
            }

            /// <summary>
            ///     Visits a <see cref="ApplySource" />.
            /// </summary>
            /// <param name="node">
            ///     The node.
            /// </param>
            /// <returns>
            ///     The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitApplySource(ApplySource node)
            {
                node = (ApplySource)base.VisitApplySource(node);

                Expression factory = Expression.Constant(null, typeof(Func<Row, DataSource>));

                var left = this.data.GetFactoryExpression(node.Left);
                var right = this.data.GetFactoryExpression(node.Right);
                var row = Expression.Parameter(typeof(Row), "row");
                var replaced = GenericVisitor.Visit(
                    (SourceFieldExpression e) => Expression.Call(row, NodeDataProviderDataSourceConverter.GetMethod.MakeGenericMethod(e.Type), Expression.Constant($"{e.SourceName}.{e.FieldName}")),
                    (UnaryExpression e) => e.NodeType == ExpressionType.Convert && e.Operand is SourceFieldExpression
                                               ? Expression.Call(row, NodeDataProviderDataSourceConverter.GetMethod.MakeGenericMethod(e.Type), Expression.Constant($"{((SourceFieldExpression)e.Operand).SourceName}.{((SourceFieldExpression)e.Operand).FieldName}"))
                                               : null,
                    right);

                if (!ReferenceEquals(replaced, right))
                {
                    factory = Expression.Constant(right);
                    right = GenericVisitor.Visit(
                        (SourceFieldExpression e) => Expression.Default(e.Type),
                        (UnaryExpression e) => e.NodeType == ExpressionType.Convert && e.Operand is SourceFieldExpression
                                                   ? Expression.Default(e.Type)
                                                   : null,
                        right);
                }

                var apply =
                    node.IsOuterApply
                        ? Evaluator.CreateJoin(
                            typeof(OuterApply).GetTypeInfo().DeclaredConstructors.First(),
                            left,
                            right,
                            factory)
                        : Evaluator.CreateJoin(
                            typeof(CrossApply).GetTypeInfo().DeclaredConstructors.First(),
                            left,
                            right,
                            factory);

                this.data.SetFactoryExpression(node, apply);

                return node;
            }

            /// <summary>
            ///     Visits a <see cref="FunctionSource" />.
            /// </summary>
            /// <param name="node">
            ///     The node.
            /// </param>
            /// <returns>
            ///     The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitFunctionSource(FunctionSource node)
            {
                node = (FunctionSource)base.VisitFunctionSource(node);

                var function = this.data.ConvertToLinqExpression(node.Function);

                this.data.SetFactoryExpression(node, Evaluator.CreateDataSource(function, node.Alias));

                return node;
            }

            /// <summary>
            ///     Visits a <see cref="FunctionTarget" />.
            /// </summary>
            /// <param name="node">
            ///     The node.
            /// </param>
            /// <returns>
            ///     The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitFunctionTarget(FunctionTarget node)
            {
                node = (FunctionTarget)base.VisitFunctionTarget(node);

                this.data.SetFactoryExpression(node, Evaluator.CreateDataTarget(this.data.ConvertToLinqExpression(node.Function)));

                return node;
            }

            /// <summary>
            ///     Visits a <see cref="Ast.Sources.JoinSource" />.
            /// </summary>
            /// <param name="node">
            ///     The node.
            /// </param>
            /// <returns>
            ///     The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitJoinSource(JoinSource node)
            {
                node = (JoinSource)base.VisitJoinSource(node);

                Expression factory;

                switch (node.JoinType)
                {
                    case JoinType.Cross:
                        factory = Evaluator.CreateJoin(
                            typeof(CrossJoin).GetTypeInfo().DeclaredConstructors.First(),
                            this.data.GetFactoryExpression(node.First),
                            this.data.GetFactoryExpression(node.Second));
                        break;
                    case JoinType.Inner:
                        factory = Evaluator.CreateJoin(
                            typeof(InnerJoin).GetTypeInfo().DeclaredConstructors.First(),
                            this.data.GetFactoryExpression(node.First),
                            this.data.GetFactoryExpression(node.Second),
                            Expression.Constant(this.data.ConvertToLinqExpression(node.Expression, false)));
                        break;
                    case JoinType.Left:
                        factory = Evaluator.CreateJoin(
                            typeof(LeftJoin).GetTypeInfo().DeclaredConstructors.First(),
                            this.data.GetFactoryExpression(node.First),
                            this.data.GetFactoryExpression(node.Second),
                            Expression.Constant(this.data.ConvertToLinqExpression(node.Expression, false)));
                        break;
                    case JoinType.NearestInner:
                        factory = Evaluator.CreateJoin(
                            typeof(NearestJoinSource).GetTypeInfo().DeclaredConstructors.First(),
                            this.data.GetFactoryExpression(node.First),
                            this.data.GetFactoryExpression(node.Second),
                            Expression.Constant(this.data.ConvertToLinqExpression(node.Expression, false)),
                            Expression.Constant(true));
                        break;
                    case JoinType.NearestLeft:
                        factory = Evaluator.CreateJoin(
                            typeof(NearestJoinSource).GetTypeInfo().DeclaredConstructors.First(),
                            this.data.GetFactoryExpression(node.First),
                            this.data.GetFactoryExpression(node.Second),
                            Expression.Constant(this.data.ConvertToLinqExpression(node.Expression, false)),
                            Expression.Constant(false));
                        break;
                    case JoinType.SequentialInner:
                        factory = Evaluator.CreateJoin(
                            typeof(InnerSequentialJoin).GetTypeInfo().DeclaredConstructors.First(),
                            this.data.GetFactoryExpression(node.First),
                            this.data.GetFactoryExpression(node.Second));
                        break;
                    case JoinType.SequentialLeft:
                        factory = Evaluator.CreateJoin(
                            typeof(LeftSequentialJoin).GetTypeInfo().DeclaredConstructors.First(),
                            this.data.GetFactoryExpression(node.First),
                            this.data.GetFactoryExpression(node.Second));
                        break;

                    default:
                        throw new NodeException(node, $"Invalid join type {node.JoinType}.");
                }

                this.data.SetFactoryExpression(node, factory);

                return node;
            }

            /// <summary>
            ///     Visits a <see cref="SelectSource" />.
            /// </summary>
            /// <param name="node">
            ///     The node.
            /// </param>
            /// <returns>
            ///     The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitSelectSource(SelectSource node)
            {
                node = (SelectSource)base.VisitSelectSource(node);

                var selectQuery = QueryPlanBuilder.Build(this.messages, this.data, node.Select);
                var aliases = new HashSet<string>(
                    new[]
                        {
                            node.Alias,
                        });

                var factory = Expression.New(
                    typeof(SelectDataSource).GetTypeInfo().DeclaredConstructors.First(),
                    Expression.Constant(selectQuery),
                    Expression.Constant(aliases));

                this.data.SetFactoryExpression(node, factory);

                return node;
            }

            /// <summary>
            ///     Visits a <see cref="VariableSource" />.
            /// </summary>
            /// <param name="node">
            ///     The node.
            /// </param>
            /// <returns>
            ///     The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitVariableSource(VariableSource node)
            {
                node = (VariableSource)base.VisitVariableSource(node);

                this.data.SetFactoryExpression(node, Evaluator.CreateDataSource(Expression.Convert(this.data.ConvertToLinqExpression(new VariableSqlExpression(node.Variable)), typeof(IDataSource)), node.Alias));

                return node;
            }

            /// <summary>
            ///     Visits a <see cref="VariableTarget" />.
            /// </summary>
            /// <param name="node">
            ///     The node.
            /// </param>
            /// <returns>
            ///     The node, or a new version of the node.
            /// </returns>
            [NotNull]
            protected internal override Node VisitVariableTarget(VariableTarget node)
            {
                node = (VariableTarget)base.VisitVariableTarget(node);

                this.data.SetFactoryExpression(node, Evaluator.CreateDataTarget(Expression.Convert(this.data.ConvertToLinqExpression(new VariableSqlExpression(node.Variable)), typeof(IDataTarget))));

                return node;
            }

            /// <summary>
            ///     Registers the data target.
            /// </summary>
            /// <param name="expression">
            ///     The expression.
            /// </param>
            /// <returns>
            ///     The <see cref="Expression" />.
            /// </returns>
            private static Expression CreateDataTarget(Expression expression)
            {
                return Evaluator.CreateDataTargetDefinition.Body
                    .ReplaceParameter(Evaluator.CreateDataTargetDefinition.Parameters[0], expression);
            }

            /// <summary>
            ///     Creates join expression and registers all aliases.
            /// </summary>
            /// <param name="constructor">
            ///     The constructor for the join expression.
            /// </param>
            /// <param name="first">
            ///     The left side of the join expression.
            /// </param>
            /// <param name="second">
            ///     The right side of the join expression.
            /// </param>
            /// <param name="rest">
            ///     The rest of the parameters.
            /// </param>
            /// <returns>
            ///     The join-expression.
            /// </returns>
            private static Expression CreateJoin(ConstructorInfo constructor, Expression first, Expression second, [NotNull] params Expression[] rest)
            {
                var arguments = new[]
                                    {
                                        first, second,
                                    };

                return Expression.New(constructor, arguments.Concat(rest));
            }
        }
    }
}