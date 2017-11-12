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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.DataSources;
    using ConnectQl.DataSources.Joins;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.ExtensionMethods;
    using ConnectQl.Interfaces;
    using ConnectQl.Parser.Ast;
    using ConnectQl.Parser.Ast.Expressions;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Parser.Ast.Targets;
    using ConnectQl.Query.Factories;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    using static FactoryBuilder;

    /// <summary>
    /// Builds factories for the query builder.
    /// </summary>
    internal static class FactoryBuilder
    {
        private static readonly ConstructorInfo ExecuteResultConstructor = typeof(ExecuteResult).GetConstructor(typeof(ICollection<ExecuteResult>));

        private static readonly ConstructorInfo RowBuilderConstructor = typeof(RowBuilder).GetConstructor(typeof(FieldMapping));

        private static readonly ConstructorInfo FieldMappingConstructor = typeof(FieldMapping).GetConstructor(typeof(IEnumerable<string>));

        private static readonly MethodInfo AsyncEnumerableOfRowSelectMethod =
            ((Func<IAsyncEnumerable<Row>, Func<Row, Row>, IAsyncEnumerable<object>>)AsyncEnumerableExtensions.Select)
            .GetMethodInfo();

        private static readonly MethodInfo AsyncEnumerableOfRowSelectAsyncMethod 
            = ((Func<IAsyncEnumerable<Row>, Func<Row, Task<Row>>, IAsyncEnumerable<object>>)AsyncEnumerableExtensions.Select)
            .GetMethodInfo();

        /// <summary>
        /// Constructor for the <see cref="JobTrigger"/> class.
        /// </summary>
        private static readonly ConstructorInfo JobTriggerConstructor = typeof(JobTrigger).GetConstructor(typeof(ITrigger), typeof(string));

        /// <summary>
        /// Constructor for the <see cref="KeyValuePair{String,Object}"/> tuple.
        /// </summary>
        private static readonly ConstructorInfo KeyValuePairConstructor = typeof(KeyValuePair<string, object>).GetConstructor(typeof(string), typeof(object));

        /// <summary>
        /// The <see cref="Task.FromResult{TResult}"/> method.
        /// </summary>
        private static readonly MethodInfo TaskFromResultMethod = typeof(Task).GetGenericMethod(nameof(Task.FromResult), (Type)null);

        /// <summary>
        /// The <see cref="TaskExtensions.Unwrap"/> method.
        /// </summary>
        private static readonly MethodInfo TaskUnwrapMethod = ((Func<Task<Task<object>>, Task<object>>)TaskExtensions.Unwrap).GetMethodInfo().GetGenericMethodDefinition();

        /// <summary>
        /// The <see cref="IInternalExecutionContext.SetVariable{T}"/> method.
        /// </summary>
        private static readonly MethodInfo InternalExecutionContextSetVariableMethod = typeof(IInternalExecutionContext).GetGenericMethod(nameof(IInternalExecutionContext.SetVariable), typeof(string), null);

        /// <summary>
        /// A factory for an empty <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{String,Object}"/>.
        /// </summary>
        private static readonly Factory<IEnumerable<KeyValuePair<string, object>>> EmptyFieldsEnumerable = Expression.Constant(Enumerable.Empty<KeyValuePair<string, object>>(), typeof(IEnumerable<KeyValuePair<string, object>>));

        /// <summary>
        /// The <see cref="DataTarget.WriteRowsAsync"/> method.
        /// </summary>
        private static readonly MethodInfo DataTargetWriteRowsAsyncMethod = typeof(DataTarget).GetGenericMethod(nameof(DataTarget.WriteRowsAsync), typeof(IInternalExecutionContext), typeof(IAsyncEnumerable<Row>), typeof(bool));

        /// <summary>
        /// The <see cref="DataSource.GetRows"/> method.
        /// </summary>
        private static readonly MethodInfo DataSourceGetRowsAsyncMethod = typeof(DataSource).GetGenericMethod(nameof(DataSource.GetRows), typeof(IInternalExecutionContext), typeof(IMultiPartQuery));

        /// <summary>
        /// The <see cref="Enumerable.Concat{TSource}"/> method.
        /// </summary>
        private static readonly MethodInfo EnumerableConcatMethod =
            ((Func<IEnumerable<object>, IEnumerable<object>, IEnumerable<object>>)Enumerable.Concat).GetMethodInfo().GetGenericMethodDefinition();

        /// <summary>
        /// The context parameter used in the factories.
        /// </summary>
        private static readonly ParameterExpression Context = Expression.Parameter(typeof(IExecutionContext), "context");

        /// <summary>
        /// The row parameter used in the factories.
        /// </summary>
        private static readonly ParameterExpression Row = Expression.Parameter(typeof(Row), "row");
        
        /// <summary>
        /// The internal execution context.
        /// </summary>
        private static readonly Expression InternalContext = Expression.Convert(Context, typeof(IInternalExecutionContext));
                
        /// <summary>
        /// A visitor that replaces all <see cref="ExecutionContextExpression"/> with the <see cref="Context"/> parameter.
        /// </summary>
        private static readonly GenericVisitor ContextReplacer = GenericVisitor.Create((ExecutionContextExpression e) => FactoryBuilder.Context);

        /// <summary>
        /// Lambda template to convert a wildcard with a source to an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{String,Object}"/>.
        /// </summary>
        private static readonly Expression<Func<Row, string, string, IEnumerable<KeyValuePair<string, object>>>> SourceWildcardLambdaTemplate =
            (row, sourcePrefix, index) =>
                row.ToDictionary()
                    .Where(kv => kv.Key.StartsWith(sourcePrefix))
                    .Select(kv => new KeyValuePair<string, object>(string.Concat(index, kv.Key), kv.Value));

        /// <summary>
        /// Lambda template to convert a wildcard without a source to an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{String,Object}"/>.
        /// </summary>
        private static readonly Expression<Func<Row, string, IEnumerable<KeyValuePair<string, object>>>> WildcardLambdaTemplate =
            (row, index) =>
                row.ToDictionary()
                    .Select(kv => new KeyValuePair<string, object>(string.Concat(index, kv.Key), kv.Value));

        /// <summary>
        /// Lambda template to build a new row from an existing row.
        /// </summary>
        private static readonly Expression<Func<Row, RowBuilder, IEnumerable<KeyValuePair<string, object>>, Row>> RowBuilderLambdaTemplate =
            (row, rowBuilder, values) => rowBuilder.CreateRow(row.UniqueId, values);

        /// <summary>
        /// Creates an factory function that returns the job triggers based on an execution context.
        /// </summary>
        /// <param name="data">
        /// The node data provider.
        /// </param>
        /// <param name="triggers">
        /// The trigger AST nodes to convert.
        /// </param>
        /// <returns>
        /// The function.
        /// </returns>
        public static Func<IExecutionContext, Task<IEnumerable<IJobTrigger>>> CreateTriggerCollectionFactory(INodeDataProvider data, [NotNull] IEnumerable<Trigger> triggers)
        {
            var expr = Expression.Convert(
                    Expression.NewArrayInit(
                        typeof(JobTrigger),
                        triggers.Select(
                            t =>
                                Expression.New(
                                    JobTriggerConstructor,
                                    data.ConvertToLinqExpression(t.Function),
                                    Expression.Constant(t.GetDisplay())))),
                    typeof(IEnumerable<JobTrigger>))
                .RewriteTasksToAsyncExpression();

            expr = EnsureTaskReturnType(expr);

            return Expression.Lambda<Func<IExecutionContext, Task<IEnumerable<IJobTrigger>>>>(expr, Context).Compile();
        }

        /// <summary>
        /// Creates a factory for a <see cref="DataTarget"/> based on the AST node.
        /// </summary>
        /// <param name="data">
        /// The node data provider.
        /// </param>
        /// <param name="target">
        /// The AST node to use.
        /// </param>
        /// <returns>
        /// A factory function returning a <see cref="DataTarget"/>.
        /// </returns>
        public static Factory<DataTarget> CreateTargetFactory(INodeDataProvider data, TargetBase target)
        {
            return data.ConvertToDataTarget(target);
        }

        /// <summary>
        /// Creates an insert statement.
        /// </summary>
        /// <param name="data">The node data provider.</param>
        /// <param name="insert">The INSERT AST node.</param>
        /// <returns>
        /// A factory that returns a <see cref="long"/> value indicating the number of records that were created.
        /// </returns>
        public static Factory<long> CreateInsert(INodeDataProvider data, InsertStatement insert)
        {
            return Expression.Call(
                data.ConvertToDataTarget(insert.Target), 
                DataTargetWriteRowsAsyncMethod, 
                InternalContext,
                CreateSelect(data, insert.Select), 
                Expression.Constant(insert.Upsert));
        }

        public static Factory<IMultiPartQuery> CreateMultiPartQuery(INodeDataProvider data, SelectFromStatement select)
        {
            return Expression.Convert(Expression.New(typeof(MultiPartQuery).GetConstructor()), typeof(IMultiPartQuery));
        }

        public static Factory<Func<Row, Row>> CreateRowTransform(INodeDataProvider data, IReadOnlyCollection<AliasedConnectQlExpression> fields)
        {
            return RowBuilderLambdaTemplate
                .ReplaceParameter("row", Row)
                .ReplaceParameter("rowBuilder", CreateRowBuilderFactory(data, fields))
                .ReplaceParameter("values", CreateFieldsFactory(data, fields));
        }

        public static Factory<RowBuilder> CreateRowBuilderFactory(INodeDataProvider data, IReadOnlyCollection<AliasedConnectQlExpression> fields)
        {
            var fieldNames = Expression.Constant(
                fields.Select(f => f.Expression is WildcardConnectQlExpression ? "*" : f.Alias).ToArray(),
                typeof(IEnumerable<string>));

            return Expression.New(
                RowBuilderConstructor,
                Expression.New(FieldMappingConstructor, fieldNames));
        }

        [NotNull]
        private static Factory<IEnumerable<KeyValuePair<string, object>>> CreateFieldsFactory(INodeDataProvider data, IReadOnlyCollection<AliasedConnectQlExpression> fields)
        {
            Factory<IEnumerable<KeyValuePair<string, object>>> fieldsFactory = null;
            var mappedFields = new List<Factory<KeyValuePair<string, object>>>();
            var wildcardIndex = 0;

            foreach (var field in fields)
            {
                if (field.Expression is WildcardConnectQlExpression wildcard)
                {
                    fieldsFactory = FactoryBuilder.Concat(FactoryBuilder.Concat(fieldsFactory, FactoryBuilder.ToEnumerable(mappedFields)), FactoryBuilder.CreateWildcard(wildcardIndex++, wildcard));
                    mappedFields.Clear();
                }
                else
                {
                    mappedFields.Add(FactoryBuilder.CreateKeyValue(data, field));
                }
            }

            if (mappedFields.Any())
            {
                fieldsFactory = FactoryBuilder.Concat(fieldsFactory, FactoryBuilder.ToEnumerable(mappedFields));
            }
            
            return fieldsFactory ?? EmptyFieldsEnumerable;
        }

        public static Factory<IEnumerable<KeyValuePair<string, object>>> CreateWildcard(int index, WildcardConnectQlExpression wildcard)
        {
            if (string.IsNullOrEmpty(wildcard.Source))
            {
                return WildcardLambdaTemplate
                    .ReplaceParameter("row", Row)
                    .ReplaceParameter("index", Expression.Constant(index + "!")).Body;
            }

            return SourceWildcardLambdaTemplate
                .ReplaceParameter("row", Row)
                .ReplaceParameter("sourcePrefix", Expression.Constant(wildcard.Source + "."))
                .ReplaceParameter("index", Expression.Constant(index + "!")).Body;
        }

        private static Factory<IEnumerable<T>> ToEnumerable<T>(IEnumerable<Factory<T>> items)
        {
            var itemArray = items.ToArray();

            return itemArray.Length == 0
                       ? (Expression)Expression.Constant(Enumerable.Empty<T>(), typeof(IEnumerable<T>))
                       : Expression.Convert(Expression.NewArrayInit(typeof(T), itemArray.Select<Factory<T>, Expression>(e => e)), typeof(IEnumerable<T>));
        }

        /// <summary>
        /// Concatenates two factories that produce an <see cref="IEnumerable{T}"/> and returns the resulting factory that returns the combined <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="first">
        /// The factory for the first <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <param name="second">
        /// The factory for the second <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <returns></returns>
        private static Factory<IEnumerable<T>> Concat<T>(Factory<IEnumerable<T>> first, Factory<IEnumerable<T>> second)
        {
            return first == null ? second : second == null ? first : Expression.Call(EnumerableConcatMethod.MakeGenericMethod(typeof(T)), first, second);
        }
        
        public static Factory<IAsyncEnumerable<Row>> CreateSelect(INodeDataProvider data, SelectStatement select)
        {
            if (select is SelectFromStatement selectFrom)
            {
                var rowTransform = CreateRowTransform(data, selectFrom.Expressions);

                var rows = Expression.Call(
                    data.ConvertToDataSource(selectFrom.Source),
                    DataSourceGetRowsAsyncMethod,
                    InternalContext,
                    CreateMultiPartQuery(data, selectFrom));

                return Expression.Call(
                    rowTransform.HasTasks ? FactoryBuilder.AsyncEnumerableOfRowSelectAsyncMethod : FactoryBuilder.AsyncEnumerableOfRowSelectMethod,
                    rows,
                    rowTransform);
            }

            return null;
        }

        public static Factory<IAsyncEnumerable<Row>> CreateSelectGroupBy(INodeDataProvider data, SelectStatement selectGroupBy)
        {
            return Expression.Constant(null, typeof(object));
        }

        public static Factory<KeyValuePair<string, object>> CreateKeyValue(INodeDataProvider data, AliasedConnectQlExpression aliased)
        {
            return Expression.New(
                KeyValuePairConstructor,
                Expression.Constant(aliased.Alias),
                CreateValue(data, aliased.Expression)
            );
        }

        public static Factory<object> CreateValue(INodeDataProvider data, ConnectQlExpressionBase expression)
        {
            return data.ConvertToLinqExpression(expression);
        }

        /// <summary>
        /// Creates a factory that produces a value based on the <see cref="ConnectQlExpression"/> provided.
        /// </summary>
        /// <param name="data">
        /// The node data provider.
        /// </param>
        /// <param name="expression">
        /// The expression that will be evaluated in the factory.
        /// </param>
        /// <returns>
        /// The result of the expression.
        /// </returns>
        public static Func<IExecutionContext, object> CreateValueFactory(INodeDataProvider data, ConnectQlExpressionBase expression)
        {
            return Expression.Lambda<Func<IExecutionContext, object>>(RewriteContext(data.ConvertToLinqExpression(expression)), Context).Compile();
        }

        /// <summary>
        /// Creates a setter for the variable declaration.
        /// </summary>
        /// <param name="data">
        /// The node data provider.
        /// </param>
        /// <param name="declaration">
        /// The variable declaration AST node.
        /// </param>
        /// <returns>
        /// A function that will set the variable on the context.
        /// </returns>
        public static Func<IExecutionContext, Task> CreateVariableSetter(INodeDataProvider data, VariableDeclaration declaration)
        {
            return Expression.Lambda<Func<IExecutionContext, Task>>(
                    Expression.Call(
                        Context,
                        InternalExecutionContextSetVariableMethod,
                        Expression.Constant(declaration.Name),
                        RewriteContext(data.ConvertToLinqExpression(declaration.Expression)),
                        Context))
                .Compile();
        }

        /// <summary>
        /// Creates a factory that produces a value based on the <see cref="ConnectQlExpression"/> provided.
        /// </summary>
        /// <param name="data">
        /// The node data provider.
        /// </param>
        /// <param name="expression">
        /// The expression that will be evaluated in the factory.
        /// </param>
        /// <returns>
        /// The result of the expression.
        /// </returns>
        public static Func<IExecutionContext, Task<object>> CreateAsyncValueFactory(INodeDataProvider data, ConnectQlExpressionBase expression)
        {
            return Expression.Lambda<Func<IExecutionContext, Task<object>>>(EnsureTaskReturnType(RewriteContext(data.ConvertToLinqExpression(expression))), Context).Compile();
        }

        /// <summary>
        /// Combines multiple query plans into a single plan. If only one plan is passed in, that plan is returned.
        /// </summary>
        /// <param name="plans">The plans to combine.</param>
        /// <returns>
        /// The combined plan.
        /// </returns>
        public static Expression CombinePlans([NotNull] IEnumerable<Expression> plans)
        {
            var planList = plans.ToList();

            return planList.Count == 1
                       ? planList[0]
                       : Expression.New(ExecuteResultConstructor, Expression.NewArrayInit(typeof(ExecuteResult), planList.Select(EnsureNonTaskReturnType)));
        }

        /// <summary>
        /// Replaces all expressions of type <see cref="ExecutionContextExpression"/> with the parameter expression in
        /// <see cref="Context"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression possibly containing <see cref="ExecutionContextExpression"/>s.
        /// </param>
        /// <returns>
        /// The expression without <see cref="ExecutionContextExpression"/>
        /// </returns>
        private static Expression RewriteContext(Expression expression)
        {
            return ContextReplacer.Visit(expression);
        }

        /// <summary>
        /// Makes sure that this expression will always return a <see cref="Task{T}"/> by optionally wrapping it
        /// using <see cref="Task.FromResult{TResult}"/>.
        /// </summary>
        /// <param name="expr">
        /// The expression to ensure.
        /// </param>
        /// <returns>
        /// An expression that returns a <see cref="Task{T}"/>.
        /// </returns>
        private static Expression EnsureTaskReturnType([NotNull] Expression expr)
        {
            if (expr.Type.IsConstructedGenericType && expr.Type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return expr;
            }

            var fromResult = TaskFromResultMethod.MakeGenericMethod(expr.Type);

            return Expression.Call(null, fromResult, expr);
        }

        /// <summary>
        /// Makes sure that this expression will always return a <see cref="Task{T}"/> by optionally wrapping it
        /// using <see cref="Task.FromResult{TResult}"/>.
        /// </summary>
        /// <param name="expr">
        /// The expression to ensure.
        /// </param>
        /// <returns>
        /// An expression that returns a <see cref="Task{T}"/>.
        /// </returns>
        [NotNull]
        private static Expression EnsureNonTaskReturnType([NotNull] Expression expr)
        {
            if (expr.Type.IsConstructedGenericType && expr.Type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return ConnectQlExpression.MakeTask(expr);
            }
            
            return expr;
        }

        private static Expression RewriteTasks<T>(Factory<T> factory)
        {
            var tasks = new List<Tuple<Expression, ParameterExpression>>();

            var filterExpression = new GenericVisitor
                                       {
                                           (GenericVisitor visitor, TaskExpression t) =>
                                               {
                                                   var parameter = Expression.Parameter(t.Expression.Type);
                                                   tasks.Add(Tuple.Create(visitor.Visit(t.Expression), parameter));
                                                   return Expression.Property(parameter, nameof(Task<object>.Result));
                                               },
                                       }.Visit(factory);

            Expression CombineTasks(Expression current, Tuple<Expression, ParameterExpression> nextTask)
            {
                var continueWith = typeof(Task<>).MakeGenericType(nextTask.Item1.Type.GenericTypeArguments[0]).GetGenericMethod(nameof(Task<object>.ContinueWith), typeof(Func<,>));

                Debug.Assert(continueWith != null, nameof(continueWith) + " != null");

                var call = Expression.Call(nextTask.Item1, continueWith.MakeGenericMethod(current.Type), Expression.Lambda(current, nextTask.Item2));

                return current.Type.IsConstructedGenericType && current.Type.GetGenericTypeDefinition() == typeof(Task<>) ? Expression.Call(null, TaskUnwrapMethod.MakeGenericMethod(current.Type.GenericTypeArguments[0]), call) : call;
            }

            //// Combine tasks into a chain of ContinueWith's.
            filterExpression = tasks.AsEnumerable().Reverse().Aggregate(filterExpression, CombineTasks);

            return filterExpression;
        }

        public static Expression<Func<IExecutionContext, Task<T>>> Create<T>(Factory<T> factory)
        {
            var expression =
                factory.HasTasks
                    ? RewriteTasks(factory)
                    : EnsureTaskReturnType(factory);

            expression = new GenericVisitor { 
                (SourceFieldExpression e) => e.CreateGetter(FactoryBuilder.Row),
                (GenericVisitor v, MethodCallExpression e) =>
                {
                    if (e.Method.IsPublic && e.Method.DeclaringType.GetTypeInfo().IsPublic)
                    {
                        return null;
                    }

                    var type = Type.GetType($"System.Func`{e.Arguments.Count + 1}");
                    var funcType = type.MakeGenericType(e.Method.GetParameters().Select(p => p.ParameterType).Concat(new[] { e.Method.ReturnType }).ToArray());

                    var method = e.Method;

                    if (method.IsGenericMethod)
                    {
                        method = method.GetGenericMethodDefinition();
                    }

                    Expression<Func<IEnumerable<MethodInfo>, string, MethodInfo>> getMethod = (methods, fullName) => methods.First(m => m.ToString() == fullName);


                    //var constant = Expression.Convert(Expression.Constant((MethodBase)e.Method), typeof(MethodInfo));

                    Expression constant = Expression.Call(
                        ((Func<Type, IEnumerable<MethodInfo>>)RuntimeReflectionExtensions.GetRuntimeMethods).GetMethodInfo(),
                        Expression.Constant(e.Method.DeclaringType));


                    constant = getMethod.ReplaceParameter("methods", constant).ReplaceParameter("fullName", Expression.Constant(method.ToString())).Body;


                    if (e.Method.IsGenericMethod)
                    {
                        constant = Expression.Call(constant, typeof(MethodInfo).GetMethod(nameof(MethodInfo.MakeGenericMethod), typeof(Type[])), Expression.NewArrayInit(typeof(Type), e.Method.GetGenericArguments().Select(Expression.Constant)));
                    }
                    
                    var delegate1 =
                        e.Object == null
                            ? Expression.Call(constant, typeof(MethodInfo).GetMethod("CreateDelegate", typeof(Type)), Expression.Constant(funcType))
                            : Expression.Call(constant, typeof(MethodInfo).GetMethod("CreateDelegate", typeof(Type), typeof(object)), Expression.Constant(funcType), v.Visit(e.Object));
                    
                    
                    return v.Visit(Expression.Call(Expression.Convert(delegate1,  funcType), funcType.GetRuntimeMethods().First(m => m.Name == "Invoke"), e.Arguments));
                },
                (GenericVisitor v, NewExpression e) =>
                {
                    if (e.Constructor.IsPublic && e.Constructor.DeclaringType.GetTypeInfo().IsPublic)
                    {
                        return null;
                    }

                    Expression<Func<Type, object[], object>> activator = (t, p) => Activator.CreateInstance(t, p);

                    var newObj = Expression.Convert(activator.ReplaceParameter("t", Expression.Constant(e.Constructor.DeclaringType)).ReplaceParameter("p", Expression.NewArrayInit(typeof(object), e.Arguments)).Body, e.Type);

                    return v.Visit(newObj);
                },
                (ConstantExpression e) =>
                {
                    if (e.Value == null)
                    {
                        return null;
                    }

                    if (e.Value is string || e.Value is int || e.Value.GetType().GetTypeInfo().IsEnum)
                    {
                        return null;
                    }

                    if (e.Value is HashSet<string> hashSet)
                    {
                        return Expression.ListInit(Expression.New(typeof(HashSet<string>).GetConstructor()), hashSet.Select(Expression.Constant));
                    }

                    if (e.Value is string[] strings)
                    {
                        return Expression.Convert(Expression.NewArrayInit(typeof(string), strings.Select(Expression.Constant)), e.Type);
                    }

                    if (e.Value is KeyValuePair<string, object>[] pairs)
                    {
                        return Expression.Convert((Expression.NewArrayBounds(typeof(string), Expression.Constant(0))), e.Type);
                    }

                    if (e.Value is Type type)
                    {
                        return Expression.Call(
                            ((Func<string, Type>)Type.GetType).GetMethodInfo(),
                            Expression.Constant(type.AssemblyQualifiedName));
                    }

                    return null;
                }}.Visit(expression);

            return Expression.Lambda<Func<IExecutionContext, Task<T>>>(RewriteContext(expression), Context);
        }
    }
}
