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
    using System.Collections.ObjectModel;
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
    using ConnectQl.Internal;
    using ConnectQl.Parser.Ast;
    using ConnectQl.Parser.Ast.Expressions;
    using ConnectQl.Parser.Ast.Sources;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Parser.Ast.Visitors;
    using ConnectQl.Query.Factories;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    using static FactoryGenerator;

    /// <summary>
    /// Builds factories for the query builder.
    /// </summary>
    internal class FactoryGenerator
    {
        /// <summary>
        /// The context parameter used in the factories.
        /// </summary>
        public static readonly ParameterExpr<IInternalExecutionContext> Context = Expr.Parameter<IInternalExecutionContext>("context");

        /// <summary>
        /// The row parameter used in the factories.
        /// </summary>
        public static readonly ParameterExpr<Row> Row = Expr.Parameter<Row>("row");

        /// <summary>
        /// The rows parameter used in the factories.
        /// </summary>
        public static readonly ParameterExpr<IAsyncReadOnlyCollection<Row>> Rows = Expr.Parameter<IAsyncReadOnlyCollection<Row>>("rows");

        /// <summary>
        /// The <see cref="Task.FromResult{TResult}"/> method.
        /// </summary>
        private static readonly MethodInfo TaskFromResultMethod = typeof(Task).GetGenericMethod(nameof(Task.FromResult), (Type)null);

        /// <summary>
        /// The <see cref="TaskExtensions.Unwrap"/> method.
        /// </summary>
        private static readonly MethodInfo TaskUnwrapMethod = ((Func<Task<Task<object>>, Task<object>>)TaskExtensions.Unwrap).GetMethodInfo().GetGenericMethodDefinition();

        /// <summary>
        /// A visitor that replaces all <see cref="ExecutionContextExpression"/> with the <see cref="Context"/> parameter.
        /// </summary>
        private static readonly GenericVisitor ContextReplacer = GenericVisitor.Create((ExecutionContextExpression e) => (ParameterExpression)FactoryGenerator.Context);

        /// <summary>
        /// Lambda template to create a <see cref="IMultiPartQuery"/>.
        /// </summary>
        private static readonly Expression<Func<IEnumerable<IField>, Expression, IEnumerable<IOrderByExpression>, IEnumerable<string>, IMultiPartQuery>>
            GetMultiPartQueryTemplate =
                (fields, filterExpression, orderByExpressions, wildcardAliases) =>
                    new MultiPartQuery
                    {
                        Fields = fields,
                        FilterExpression = filterExpression,
                        OrderByExpressions = orderByExpressions,
                        WildcardAliases = wildcardAliases
                    };

        /// <summary>
        /// Lambda template to convert a wildcard with a source to an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{String,Object}"/>.
        /// </summary>
        private static readonly Expression<Func<Row, string, string, IEnumerable<KeyValuePair<string, object>>>>
            SourceWildcardLambdaTemplate =
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
        /// Initializes a new instance of the <see cref="FactoryGenerator"/> class.
        /// </summary>
        /// <param name="nodeData">
        /// The node this.nodeData.
        /// </param>
        public FactoryGenerator(INodeDataProvider nodeData)
        {
            this.NodeData = nodeData;
        }

        /// <summary>
        /// Gets the node data.
        /// </summary>
        public INodeDataProvider NodeData { get; }

        ///// <summary>
        ///// Creates an expr function that returns the job triggers based on an execution context.
        ///// </summary>
        ///// <param name="triggers">
        ///// The trigger AST nodes to convert.
        ///// </param>
        ///// <returns>
        ///// The function.
        ///// </returns>
        //public Func<IExecutionContext, Task<IEnumerable<IJobTrigger>>> CreateTriggerCollectionFactory([NotNull] IEnumerable<Trigger> triggers)
        //{
        //    var expr = Expression.Convert(
        //            Expression.NewArrayInit(
        //                typeof(JobTrigger),
        //                triggers.Select(
        //                    t =>
        //                        Expression.New(
        //                            JobTriggerConstructor,
        //                            this.NodeData.ConvertToLinqExpression(t.Function),
        //                            Expression.Constant(t.GetDisplay())))),
        //            typeof(IEnumerable<JobTrigger>))
        //        .RewriteTasksToAsyncExpression();

        //    expr = EnsureTaskReturnType(expr);

        //    return Expression.Lambda<Func<IExecutionContext, Task<IEnumerable<IJobTrigger>>>>(expr, Context).Compile();
        //}

        /// <summary>
        /// Creates an insert statement.
        /// </summary>
        /// <param name="insert">The INSERT AST node.</param>
        /// <returns>
        /// A expr that returns a <see cref="long"/> value indicating the number of records that were created.
        /// </returns>
        [NotNull]
        public Expr<IExecuteResult> GenerateInsert([NotNull] InsertStatement insert)
        {
            return
                Expr.Create(
                        (a, b) => new ExecuteResult(a, b),
                        this.NodeData.ConvertToDataTarget(insert.Target)
                            .Call(
                                (dt, a1, a2, a3) => dt.WriteRowsAsync(a1, a2, a3),
                                FactoryGenerator.Context,
                                this.GenerateSelectInternal(insert.Select),
                                Expr.Constant(insert.Upsert)),
                        Expr.Constant((IAsyncEnumerable<Row>)null))
                    .ConvertTo<IExecuteResult>();
        }
        
        [NotNull]
        public Expr<IAsyncEnumerable<Row>> CreateSelectGroupBy(SelectStatement selectGroupBy)
        {
            return new Expr<IAsyncEnumerable<Row>>(Expression.Constant(null, typeof(object)));
        }
        
        /// <summary>
        /// Creates a setter for the variable declaration.
        /// </summary>
        /// <param name="data">
        /// The node nodeData provider.
        /// </param>
        /// <param name="declaration">
        /// The variable declaration AST node.
        /// </param>
        /// <returns>
        /// A function that will set the variable on the context.
        /// </returns>
        [NotNull]
        public Expr<ExecuteResult> GenerateVariableSetter([NotNull] VariableDeclaration declaration)
        {
            return Expr.Block(
                Context.Call(
                    (context, variable, value) => context.SetVariable(variable, value),
                    Expr.Constant(declaration.Name),
                    new Expr<object>(this.NodeData.ConvertToLinqExpression(declaration.Expression))),
                Expr.Create<ExecuteResult>());
        }

        /// <summary>
        /// Creates a expr that produces a value based on the <see cref="ConnectQlExpression"/> provided.
        /// </summary>
        /// <param name="data">
        /// The node nodeData provider.
        /// </param>
        /// <param name="expression">
        /// The expression that will be evaluated in the expr.
        /// </param>
        /// <returns>
        /// The result of the expression.
        /// </returns>
        public Func<IExecutionContext, Task<object>> CreateAsyncValueFactory(ConnectQlExpressionBase expression)
        {
            return Expression.Lambda<Func<IExecutionContext, Task<object>>>(EnsureTaskReturnType(RewriteContext(this.NodeData.ConvertToLinqExpression(expression))), (ParameterExpression)Context).Compile();
        }

        /// <summary>
        /// Combines multiple query plans into a single plan. If only one plan is passed in, that plan is returned.
        /// </summary>
        /// <param name="statements">The statements to combine.</param>
        /// <returns>
        /// The combined plan.
        /// </returns>
        [NotNull]
        public Expr<IExecuteResult> GenerateBlock([NotNull] IEnumerable<Node> statements)
        {
            var planList = statements.Select(this.NodeData.GetQueryPlanExpression).ToList();
            
            return Expr.Create(pl => new ExecuteResult(pl), Expr.Enumerable(planList)).ConvertTo<IExecuteResult>();
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
        public static Expression EnsureTaskReturnType([NotNull] Expression expr)
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

        public static Expression RewriteTasks<T>(Expr<T> expr)
        {
            var tasks = new List<Tuple<Expression, ParameterExpression>>();

            Expression filterExpression = new GenericVisitor
                                       {
                                           (GenericVisitor visitor, TaskExpression t) =>
                                               {
                                                   var parameter = Expression.Parameter(t.Expression.Type);
                                                   tasks.Add(Tuple.Create(visitor.Visit(t.Expression), parameter));
                                                   return Expression.Property(parameter, nameof(Task<object>.Result));
                                               },
                                       }.Visit(expr);

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

        //public Expression<Func<IExecutionContext, Task<T>>> Create<T>(Expr<T> expr)
        //{
        //    var expression =
        //        expr.HasTasks
        //            ? RewriteTasks(expr)
        //            : EnsureTaskReturnType(expr);

        //    expression = new GenericVisitor { 
        //        (SourceFieldExpression e) => e.CreateGetter(FactoryGenerator.Row),
        //        (GenericVisitor v, MethodCallExpression e) =>
        //        {
        //            if (e.Method.IsPublic && e.Method.DeclaringType.GetTypeInfo().IsPublic)
        //            {
        //                return null;
        //            }

        //            var type = Type.GetType($"System.Func`{e.Arguments.Count + 1}");
        //            var funcType = type.MakeGenericType(e.Method.GetParameters().Select(p => p.ParameterType).Concat(new[] { e.Method.ReturnType }).ToArray());

        //            var method = e.Method;

        //            if (method.IsGenericMethod)
        //            {
        //                method = method.GetGenericMethodDefinition();
        //            }

        //            Expression<Func<IEnumerable<MethodInfo>, string, MethodInfo>> getMethod = (methods, fullName) => methods.First(m => m.ToString() == fullName);


        //            //var constant = Expression.Convert(Expression.Constant((MethodBase)e.Method), typeof(MethodInfo));

        //            Expression constant = Expression.Call(
        //                ((Func<Type, IEnumerable<MethodInfo>>)RuntimeReflectionExtensions.GetRuntimeMethods).GetMethodInfo(),
        //                Expression.Constant(e.Method.DeclaringType));


        //            constant = getMethod.ReplaceParameter("methods", constant).ReplaceParameter("fullName", Expression.Constant(method.ToString())).Body;


        //            if (e.Method.IsGenericMethod)
        //            {
        //                constant = Expression.Call(constant, typeof(MethodInfo).GetMethod(nameof(MethodInfo.MakeGenericMethod), typeof(Type[])), Expression.NewArrayInit(typeof(Type), e.Method.GetGenericArguments().Select(Expression.Constant)));
        //            }
                    
        //            var delegate1 =
        //                e.Object == null
        //                    ? Expression.Call(constant, typeof(MethodInfo).GetMethod("CreateDelegate", typeof(Type)), Expression.Constant(funcType))
        //                    : Expression.Call(constant, typeof(MethodInfo).GetMethod("CreateDelegate", typeof(Type), typeof(object)), Expression.Constant(funcType), v.Visit(e.Object));
                    
                    
        //            return v.Visit(Expression.Call(Expression.Convert(delegate1,  funcType), funcType.GetRuntimeMethods().First(m => m.Name == "Invoke"), e.Arguments));
        //        },
        //        (GenericVisitor v, NewExpression e) =>
        //        {
        //            if (e.Constructor.IsPublic && e.Constructor.DeclaringType.GetTypeInfo().IsPublic)
        //            {
        //                return null;
        //            }

        //            Expression<Func<Type, object[], object>> activator = (t, p) => Activator.CreateInstance(t, p);

        //            var newObj = Expression.Convert(activator.ReplaceParameter("t", Expression.Constant(e.Constructor.DeclaringType)).ReplaceParameter("p", Expression.NewArrayInit(typeof(object), e.Arguments)).Body, e.Type);

        //            return v.Visit(newObj);
        //        },
        //        (ConstantExpression e) =>
        //        {
        //            if (e.Value == null)
        //            {
        //                return null;
        //            }

        //            if (e.Value is string || e.Value is int || e.Value.GetType().GetTypeInfo().IsEnum)
        //            {
        //                return null;
        //            }

        //            if (e.Value is HashSet<string> hashSet)
        //            {
        //                return Expression.ListInit(Expression.New(typeof(HashSet<string>).GetConstructor()), hashSet.Select(Expression.Constant));
        //            }

        //            if (e.Value is string[] strings)
        //            {
        //                return Expression.Convert(Expression.NewArrayInit(typeof(string), strings.Select(Expression.Constant)), e.Type);
        //            }

        //            if (e.Value is KeyValuePair<string, object>[] pairs)
        //            {
        //                return Expression.Convert((Expression.NewArrayBounds(typeof(string), Expression.Constant(0))), e.Type);
        //            }

        //            if (e.Value is Type type)
        //            {
        //                return Expression.Call(
        //                    ((Func<string, Type>)Type.GetType).GetMethodInfo(),
        //                    Expression.Constant(type.AssemblyQualifiedName));
        //            }

        //            return null;
        //        }}.Visit(expression);

        //    return Expression.Lambda<Func<IExecutionContext, Task<T>>>(RewriteContext(expression), Context);
        //}
        
    }
}
