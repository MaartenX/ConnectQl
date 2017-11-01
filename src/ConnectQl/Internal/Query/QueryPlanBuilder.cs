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
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.DataSources;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Sources;
    using ConnectQl.Internal.Ast.Statements;
    using ConnectQl.Internal.Ast.Visitors;
    using ConnectQl.Internal.DataSources;
    using ConnectQl.Internal.DataSources.Joins;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Query.Plans;
    using ConnectQl.Internal.Validation;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    using AsyncGroupValueFactory = System.Func<ConnectQl.Interfaces.IExecutionContext, ConnectQl.AsyncEnumerables.IAsyncReadOnlyCollection<ConnectQl.Results.Row>, System.Threading.Tasks.Task<System.Collections.Generic.KeyValuePair<string, object>[]>>;
    using AsyncValueFactory = System.Func<ConnectQl.Interfaces.IExecutionContext, ConnectQl.Results.Row, System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>>>;
    using ValueFactory = System.Func<ConnectQl.Interfaces.IExecutionContext, ConnectQl.Results.Row, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>>;

    /// <summary>
    /// The query plan.
    /// </summary>
    internal class QueryPlanBuilder : NodeVisitor
    {
        /// <summary>
        /// Lambda that concatenates two <see cref="IEnumerable{T}"/>s of key/value pairs.
        /// </summary>
        private static readonly Expression<Func<IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>>> Concatenate =
            (left, right) => left.Concat(right);

        /// <summary>
        /// The constructor for a <see cref="T:KeyValuePair{string,object}"/>.
        /// </summary>
        private static readonly ConstructorInfo KeyValuePairConstructor = typeof(KeyValuePair<string, object>).GetTypeInfo().DeclaredConstructors.First(c => c.GetParameters().Length == 2);

        /// <summary>
        /// The data.
        /// </summary>
        private readonly INodeDataProvider data;

        /// <summary>
        /// The messages.
        /// </summary>
        private readonly IMessageWriter messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryPlanBuilder"/> class.
        /// </summary>
        /// <param name="messages">
        /// The messages.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        private QueryPlanBuilder(IMessageWriter messages, INodeDataProvider data)
        {
            this.messages = messages;
            this.data = data;
        }

        /// <summary>
        /// Builds a query plan.
        /// </summary>
        /// <param name="messages">
        /// The messages.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="node">
        /// The script.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static IQueryPlan Build(IMessageWriter messages, INodeDataProvider data, Node node)
        {
            var generator = new QueryPlanBuilder(messages, data);

            generator.Visit(node);

            return data.GetQueryPlan(node);
        }

        /// <summary>
        /// Visits a <see cref="Block"/>, creates query plans for each statement and combines them in a
        ///     <see cref="CombinedQueryPlan"/>.
        ///     This <see cref="CombinedQueryPlan"/> is than attached to the current node in the <see cref="data"/> store.
        /// </summary>
        /// <param name="node">
        /// The node to visit.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitBlock(Block node)
        {
            node = (Block)base.VisitBlock(node);

            this.data.SetQueryPlan(node, new CombinedQueryPlan(node.Statements.Select(this.data.GetQueryPlan)));

            return node;
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
        protected internal override Node VisitDeclareJobStatement([NotNull] DeclareJobStatement node)
        {
            base.VisitDeclareJobStatement(node);

            var context = Expression.Parameter(typeof(IExecutionContext), "context");
            var replaceContext = GenericVisitor.Create((ExecutionContextExpression e) => context);

            var expr = Expression.Convert(
                Expression.NewArrayInit(
                    typeof(JobTrigger),
                    node.Triggers.Select(t =>
                        Expression.New(
                            typeof(JobTrigger).GetTypeInfo().DeclaredConstructors.First(),
                            replaceContext.Visit(this.data.ConvertToLinqExpression(t.Function)),
                            Expression.Constant(t.GetDisplay())))),
                typeof(IEnumerable<JobTrigger>)).RewriteTasksToAsyncExpression();

            if (!expr.Type.IsConstructedGenericType || expr.Type.GetGenericTypeDefinition() != typeof(Task<>))
            {
                var fromResult = typeof(Task).GetGenericMethod(nameof(Task.FromResult), (Type)null).MakeGenericMethod(typeof(IEnumerable<JobTrigger>));

                expr = Expression.Call(null, fromResult, expr);
            }

            var factories = Expression.Lambda<Func<IExecutionContext, Task<IEnumerable<IJobTrigger>>>>(expr, context).Compile();

            var plan = node.Statements.Count == 1
                           ? this.data.GetQueryPlan(node.Statements[0])
                           : new CombinedQueryPlan(node.Statements.Select(this.data.GetQueryPlan));

            this.data.SetQueryPlan(node, new DeclareJobPlan(node.Name, plan, factories));

            return base.VisitDeclareJobStatement(node);
        }

        /// <summary>
        /// Visits a declare statement and attaches a <see cref="DeclareStatement"/> to the <paramref name="node"/> in the
        ///     <see cref="data"/> store.
        ///     When multiple variables are declared in the <see cref="DeclareStatement"/>, a <see cref="CombinedQueryPlan"/> is
        ///     created containing all query plans for declarations.
        /// </summary>
        /// <param name="node">
        /// The node to visit.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitDeclareStatement(DeclareStatement node)
        {
            node = (DeclareStatement)base.VisitDeclareStatement(node);

            var plan = node.Declarations.Count == 1
                           ? this.data.GetQueryPlan(node.Declarations[0])
                           : new CombinedQueryPlan(node.Declarations.Select(this.data.GetQueryPlan));

            this.data.SetQueryPlan(node, plan);

            return node;
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
        protected internal override Node VisitFunctionSource([NotNull] FunctionSource node)
        {
            this.data.SetAlias(node.Function, node.Alias);

            return base.VisitFunctionSource(node);
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
        [NotNull]
        protected internal override Node VisitInsertStatement(InsertStatement node)
        {
            node = (InsertStatement)base.VisitInsertStatement(node);

            var context = Expression.Parameter(typeof(IExecutionContext), "context");
            var replaceContext = new GenericVisitor
                                     {
                                         (ExecutionContextExpression e) => context,
                                     };

            var targetFactory = Expression.Lambda<Func<IExecutionContext, DataTarget>>(replaceContext.Visit(this.data.ConvertToDataTarget(node.Target)), context).Compile();

            this.data.SetQueryPlan(node, new InsertQueryPlan(targetFactory, this.data.GetQueryPlan(node.Select), node.Upsert));

            return node;
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
        [NotNull]
        protected internal override Node VisitSelectFromStatement(SelectFromStatement node)
        {
            node = (SelectFromStatement)base.VisitSelectFromStatement(node);

            if (node.Groupings.Any())
            {
                var groupQuery = GroupQueryVisitor.GetGroupQuery(node, this.data);
                var groupFactories = this.GetGroupValueFactory(groupQuery.Expressions.Concat(new[]
                                                                                                 {
                                                                                                     new AliasedConnectQlExpression(groupQuery.Having, "$having"),
                                                                                                 }).Concat(groupQuery.OrderBy.Select((o, i) => new AliasedConnectQlExpression(o.Expression, $"$order{i}"))).Where(e => e.Expression != null));
                var plan = this.CreateSelectQueryPlan(groupQuery.RowSelect);
                var fields = node.Expressions.Select(f => f.Expression is WildcardConnectQlExpression ? "*" : f.Alias);
                var orders = groupQuery.OrderBy.Select((o, i) => new OrderByExpression(CustomExpression.MakeSourceField(null, $"$order{i}", true), o.Ascending));
                var having = groupQuery.Having == null ? null : CustomExpression.MakeSourceField(null, "$having", true, typeof(bool));

                var aliases = groupQuery.Expressions;

                this.data.SetQueryPlan(node, new SelectGroupByQueryPlan(plan, groupFactories, groupQuery.Groupings, aliases, having, orders, fields));
            }
            else
            {
                this.data.SetQueryPlan(node, this.CreateSelectQueryPlan(node));
            }

            return node;
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
        protected internal override Node VisitSelectSource([NotNull] SelectSource node)
        {
            this.data.SetAlias(node.Select, node.Alias);

            return base.VisitSelectSource(node);
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
        [NotNull]
        protected internal override Node VisitUseStatement(UseStatement node)
        {
            node = (UseStatement)base.VisitUseStatement(node);

            var context = Expression.Parameter(typeof(IExecutionContext), "context");
            var getValue = Expression.Lambda<Func<IExecutionContext, object>>(
                GenericVisitor.Visit(
                    (ExecutionContextExpression e) => context,
                    this.data.ConvertToLinqExpression(node.SettingFunction)),
                context).Compile();

            this.data.SetQueryPlan(node, new UseDefaultQueryPlan(node.SettingFunction.Name, node.FunctionName, getValue));

            return node;
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
        [NotNull]
        protected internal override Node VisitVariableDeclaration(VariableDeclaration node)
        {
            node = (VariableDeclaration)base.VisitVariableDeclaration(node);

            this.data.SetQueryPlan(node, new DeclareVariableQueryPlan(node.Name, this.data.ConvertToLinqExpression(node.Expression)));

            return node;
        }

        /// <summary>
        /// The concatenate lambdas.
        /// </summary>
        /// <param name="first">
        /// The first.
        /// </param>
        /// <param name="second">
        /// The second.
        /// </param>
        /// <typeparam name="T">
        /// The type of the expressions.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private static Expression<T> ConcatenateLambdas<T>([NotNull] Expression<T> first, [NotNull] Expression<T> second)
        {
            return Expression.Lambda<T>(
                QueryPlanBuilder.Concatenate.Body
                    .ReplaceParameter(
                        QueryPlanBuilder.Concatenate.Parameters[0],
                        first.Body)
                    .ReplaceParameter(
                        QueryPlanBuilder.Concatenate.Parameters[1],
                        second.Body.ReplaceParameters(second.Parameters, first.Parameters)),
                first.Parameters);
        }

        /// <summary>
        /// Converts a list of lambda expressions tot.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the array elements.
        /// </typeparam>
        /// <param name="values">
        /// The array values.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private static Expression<T> ToArrayInit<T>([NotNull] IList<LambdaExpression> values)
        {
            var parameters = values.First().Parameters;

            return Expression.Lambda<T>(Expression.Convert(Expression.NewArrayInit(typeof(KeyValuePair<string, object>), values.Select(f => f.Body.ReplaceParameters(f.Parameters, parameters))), typeof(IEnumerable<KeyValuePair<string, object>>)), parameters);
        }

        /// <summary>
        /// The create select query plan async.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [NotNull]
        private SelectQueryPlan CreateSelectQueryPlan([NotNull] SelectFromStatement node)
        {
            var usedFields = new HashSet<IField>();
            var wildcardAliases = new HashSet<string>();
            var fieldNames = node.Expressions.Select(f => f.Expression is WildcardConnectQlExpression ? "*" : f.Alias);
            var valueFactory = this.GetValueFactory(node.Expressions, usedFields, wildcardAliases, SourceAliasesRetriever.GetAllSources(node));
            var sourceFactory = this.GetSourceFactory(node.Source);

            var query = new MultiPartQuery
                            {
                                Fields = usedFields,
                                FilterExpression = this.data.ConvertToLinqExpression(node.Where, false),
                                OrderByExpressions = node.Orders.Select(o => new OrderByExpression(this.data.ConvertToLinqExpression(o.Expression, false), o.Ascending)),
                                WildcardAliases = wildcardAliases,
                            };

            var func = valueFactory as ValueFactory;
            var queryPlan = func != null
                                ? new SelectQueryPlan(sourceFactory, query, fieldNames, func)
                                : new SelectQueryPlan(sourceFactory, query, fieldNames, (AsyncValueFactory)valueFactory);

            return queryPlan;
        }

        /// <summary>
        /// The get group value factory async.
        /// </summary>
        /// <param name="expressions">
        /// The expressions.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private AsyncGroupValueFactory GetGroupValueFactory([NotNull] IEnumerable<AliasedConnectQlExpression> expressions)
        {
            var rows = Expression.Parameter(typeof(IAsyncReadOnlyCollection<Row>), "rows");
            var context = Expression.Parameter(typeof(IExecutionContext));

            var lambdaVisitor = new GenericVisitor
                                    {
                                        (ExecutionContextExpression e) => context,
                                        (SourceFieldExpression e) => e.CreateGroupGetter(rows),
                                        (UnaryExpression e) => e.NodeType == ExpressionType.Convert ? (e.Operand as SourceFieldExpression)?.CreateGroupGetter(rows) : null,
                                    };

            var fields = expressions.Select(
                expression =>
                    Expression.New(
                        QueryPlanBuilder.KeyValuePairConstructor,
                        Expression.Constant(expression.Alias),
                        Expression.Convert(lambdaVisitor.Visit(this.data.ConvertToLinqExpression(expression.Expression)), typeof(object)))).ToArray();

            var rowGetter = Expression.NewArrayInit(typeof(KeyValuePair<string, object>), fields);

            // Expression.Call(TaskExpression.Task(Expression.Call(null, typeof(AsyncEnumerableExtensions))))
            var result =
                Expression.Lambda<AsyncGroupValueFactory>(
                    rowGetter.RewriteTasksToAsyncExpression(),
                    context,
                    rows);

            return result.Compile();
        }

        /// <summary>
        /// Gets a factory function for the data source of this.
        /// </summary>
        /// <param name="source">
        /// The <see cref="SourceBase"/> AST node to generate the factory for.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private Func<IExecutionContext, Task<DataSource>> GetSourceFactory(SourceBase source)
        {
            if (source == null)
            {
                return e => Task.FromResult<DataSource>(new NoDataSource());
            }

            var context = Expression.Parameter(typeof(IExecutionContext), "context");
            var row = Expression.Parameter(typeof(Row), "row");
            var replaceContext = new GenericVisitor
                                     {
                                         (ExecutionContextExpression e) => context,
                                         (SourceFieldExpression e) => e.CreateGetter(row),
                                         (UnaryExpression e) => e.NodeType == ExpressionType.Convert && e.Operand is SourceFieldExpression
                                                                    ? ((SourceFieldExpression)e.Operand).CreateGetter(row, e.Type)
                                                                    : null,
                                     };

            var sourceFactoryLambda = Expression.Lambda(replaceContext.Visit(this.data.ConvertToDataSource(source)), context).RewriteTasksToAsyncExpression();

            if (sourceFactoryLambda.ReturnType != typeof(Task<DataSource>))
            {
                sourceFactoryLambda = Expression.Lambda(Expression.Call(((Func<DataSource, Task<DataSource>>)Task.FromResult).GetMethodInfo(), sourceFactoryLambda.Body), sourceFactoryLambda.Parameters);
            }

            return ((Expression<Func<IExecutionContext, Task<DataSource>>>)sourceFactoryLambda).Compile();
        }

        /// <summary>
        /// Gets a function that creates the values for the selected records from a row.
        /// </summary>
        /// <param name="expressions">
        /// The <see cref="AliasedConnectQlExpression"/>s to get the values for.
        /// </param>
        /// <param name="fieldList">
        /// A collection that will be filled with all the fields that are used in the select statement.
        /// </param>
        /// <param name="wildCardAliasList">
        /// A collection that will be filled with all the wildcards that are used in the select statement.
        /// </param>
        /// <param name="allSourceAliases">
        /// Contains all source aliases in the query, so when a global wildcard is found, all fields are added.
        /// </param>
        /// <returns>
        /// A delegate.
        /// </returns>
        private Delegate GetValueFactory([NotNull] IEnumerable<AliasedConnectQlExpression> expressions, ICollection<IField> fieldList, ICollection<string> wildCardAliasList, IEnumerable<string> allSourceAliases)
        {
            var row = Expression.Parameter(typeof(Row), "row");
            var context = Expression.Parameter(typeof(IExecutionContext), "context");
            var fieldFactories = new List<LambdaExpression>();

            var lambdaVisitor = new GenericVisitor
                                    {
                                        (ExecutionContextExpression e) => context,
                                        (SourceFieldExpression e) =>
                                            {
                                                fieldList?.Add(new Field(e.SourceName, e.FieldName));

                                                return e.CreateGetter(row);
                                            },
                                        (UnaryExpression e) => e.NodeType == ExpressionType.Convert ? (e.Operand as SourceFieldExpression)?.CreateGetter(row, e.Type) : null,
                                    };

            Func<string, Expression, Expression<Func<IExecutionContext, Row, KeyValuePair<string, object>>>> convertToLambda = (alias, expression) =>
                {
                    var lambda = lambdaVisitor.Visit(expression.EvaluateAsValue());
                    lambda = lambda.Type == typeof(object) ? lambda : Expression.Convert(lambda, typeof(object));
                    return Expression.Lambda<Func<IExecutionContext, Row, KeyValuePair<string, object>>>(Expression.New(QueryPlanBuilder.KeyValuePairConstructor, Expression.Constant(alias), lambda.CatchErrors()), context, row);
                };

            Expression<ValueFactory> result = null;

            var wildcardIndex = 0;
            var hasGlobalAlias = false;

            foreach (var expression in expressions)
            {
                if (expression.Expression is WildcardConnectQlExpression wildcard)
                {
                    if (wildcard.Source != null)
                    {
                        wildCardAliasList.Add(wildcard.Source);
                    }
                    else
                    {
                        hasGlobalAlias = true;
                    }

                    var sourcePrefix = $"{wildcard.Source}.";
                    var index = wildcardIndex++;
                    var lambda = string.IsNullOrEmpty(wildcard.Source)
                                     ? (Expression<ValueFactory>)((c, r) => r.ColumnNames.Select(cn => new KeyValuePair<string, object>($"{index}!{cn}", r[cn])))
                                     : (c, r) => r.ColumnNames.Where(cn => cn.StartsWith(sourcePrefix)).Select(cn => new KeyValuePair<string, object>($"{index}!{cn}", r[cn]));

                    lambda = lambda.ReplaceParameter(lambda.Parameters[0], context).ReplaceParameter(lambda.Parameters[1], row);

                    if (fieldFactories.Count != 0)
                    {
                        var fields = QueryPlanBuilder.ToArrayInit<ValueFactory>(fieldFactories);

                        result = result == null ? fields : QueryPlanBuilder.ConcatenateLambdas(result, fields);

                        fieldFactories.Clear();
                    }

                    result = result == null ? lambda : QueryPlanBuilder.ConcatenateLambdas(result, lambda);
                }
                else
                {
                    fieldFactories.Add(convertToLambda(expression.Alias, this.data.ConvertToLinqExpression(expression.Expression)));
                }
            }

            if (fieldFactories.Count != 0)
            {
                var fields = QueryPlanBuilder.ToArrayInit<ValueFactory>(fieldFactories);

                result = result == null ? fields : QueryPlanBuilder.ConcatenateLambdas(result, fields);
            }

            if (hasGlobalAlias)
            {
                foreach (var alias in allSourceAliases)
                {
                    wildCardAliasList.Add(alias);
                }
            }

            var asyncResult = (result ?? ((c, r) => Enumerable.Empty<KeyValuePair<string, object>>())).RewriteTasksToAsyncExpression();

            return asyncResult.ReturnType == typeof(IEnumerable<KeyValuePair<string, object>>)
                       ? ((Expression<ValueFactory>)asyncResult).Compile()
                       : (Delegate)((Expression<AsyncValueFactory>)asyncResult).Compile();
        }
    }
}