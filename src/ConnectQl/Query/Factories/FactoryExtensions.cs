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

namespace ConnectQl.Query.Factories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    using ConnectQl.Comparers;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.ExtensionMethods;

    using JetBrains.Annotations;

    /// <summary>
    /// Extensions for the <see cref="Expr"/> classes.
    /// </summary>
    [PublicAPI]
    public static class FactoryExtensions
    {
        /// <summary>
        /// The task unwrap method.
        /// </summary>
        private static readonly MethodInfo TaskUnwrapMethod = typeof(TaskExtensions).GetRuntimeMethods().First(m => m.Name == "Unwrap" && m.IsGenericMethodDefinition);

        /// <summary>
        /// Concatenates two factories that create <see cref="IEnumerable{T}"/>s.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the <see cref="IEnumerable{T}"/>.
        /// </typeparam>
        /// <param name="left">
        /// The left <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <param name="right">
        /// The right <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        public static Expr<IEnumerable<T>> Concat<T>([CanBeNull] this Expr<IEnumerable<T>> left, [CanBeNull] Expr<IEnumerable<T>> right)
        {
            return left == null ? right : right == null ? left : Expr.Call(Enumerable.Concat, left, right);
        }

        /// <summary>
        /// Adds an argument to a function.
        /// </summary>
        /// <typeparam name="TArg">
        /// The type of the argument to add.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="func">
        /// The function to add the argument to.
        /// </param>
        /// <param name="arg">
        /// The argument to add.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<Func<TArg, TResult>> AddArgument<TArg, TResult>([NotNull]this Expr<Func<TResult>> func, ParameterExpr<TArg> arg)
        {
            return new Expr<Func<TArg, TResult>>(Expression.Lambda(((LambdaExpression)func).Body, arg));
        }

        /// <summary>
        /// Adds an argument to a function.
        /// </summary>
        /// <typeparam name="TArg1">
        /// The type of the first argument of the function.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the argument to add.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="func">
        /// The function to add the argument to.
        /// </param>
        /// <param name="arg">
        /// The argument to add.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<Func<TArg1, TArg2, TResult>> AddArgument<TArg1, TArg2, TResult>([NotNull] this Expr<Func<TArg1, TResult>> func, ParameterExpr<TArg2> arg)
        {
            return new Expr<Func<TArg1, TArg2, TResult>>(Expression.Lambda(((LambdaExpression)func).Body, ((LambdaExpression)func).Parameters.Concat(new[] { (ParameterExpression)arg })));
        }

        /// <summary>
        /// Rewrites a expr that may or may not contain any <see cref="TaskExpression"/>s into a
        /// expr that returns a <see cref="Task{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the expr.
        /// </typeparam>
        /// <param name="expr">
        /// The factory to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<Func<TArg, Task<TResult>>> MakeAsync<TArg, TResult>(this Expr<Func<TArg, TResult>> expr)
        {
            var expression = expr.ToExpression();

            return new Expr<Func<TArg, Task<TResult>>>(Expression.Lambda<Func<TArg, Task<TResult>>>(
                (new Expr<TResult>(expression.Body)).MakeAsync(expression.Parameters),
                expression.Parameters));
        }

        private class ParameterList : IEnumerable<ParameterExpression>
        {
            private readonly ParameterExpression obj;

            private readonly List<ParameterExpression> parameters = new List<ParameterExpression>();

            public ParameterList(ParameterExpression obj)
            {
                this.obj = obj;
            }

            public Expression Add(ParameterExpression e)
            {
                var index = this.parameters.FindIndex(p => p == e);

                if (index == -1)
                {
                    index = this.parameters.Count;
                    this.parameters.Add(e);
                }

                return Expression.ArrayIndex(Expression.Convert(this.obj, typeof(object[])), Expression.Constant(index));
            }

            public int Count => parameters.Count;

            public IEnumerator<ParameterExpression> GetEnumerator()
            {
                return this.parameters.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        
        /// <summary>
        /// Rewrites a expr that may or may not contain any <see cref="TaskExpression"/>s into a
        /// expr that returns a <see cref="Task{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the expr.
        /// </typeparam>
        /// <param name="expr">
        /// The expr to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<Task<T>> MakeAsync<T>(this Expr<T> expr, IEnumerable<ParameterExpression> lambdaParameters = null)
        {
            var currentParameters = lambdaParameters == null ? new List<ParameterExpression>() : new List<ParameterExpression>(lambdaParameters);
            var obj = Expression.Parameter(typeof(object), "obj");
            var tasks = new List<Tuple<Expression, ParameterExpression, int>>();
            var array = Expression.Convert(obj, typeof(object[]));

            var parameters = new ParameterList(obj);
            var expression = (Expression)expr;
            var filterExpression = new GenericVisitor
                                          {
                                              (LambdaExpression e) => e,
                                              (GenericVisitor visitor, TaskExpression t) =>
                                              {
                                                  var parameter = Expression.Parameter(t.Expression.Type);

                                                  tasks.Add(
                                                      Tuple.Create(
                                                          visitor.Visit(t.Expression),
                                                          parameter,
                                                          parameters.Count));

                                                  return Expression.Property(parameter, nameof(Task<object>.Result));
                                              },
                                          }.Visit(expression is LambdaExpression lambda ? lambda.Body : expression);
            
            Expression CombineTasks(Expression current, Tuple<Expression, ParameterExpression, int> nextTask)
            {
                MethodInfo continueWith;
                MethodCallExpression call;

                if (nextTask.Item3 > 0)
                {
                    continueWith = typeof(Task<>).MakeGenericType(nextTask.Item1.Type.GenericTypeArguments[0]).GetGenericMethod(nameof(Task<object>.ContinueWith), typeof(Func<,,>), typeof(object));
                    call = Expression.Call(nextTask.Item1, continueWith.MakeGenericMethod(current.Type), Expression.Lambda(current, nextTask.Item2, obj), nextTask == tasks.Last() ? (Expression)Expression.Convert(Expression.Constant(parameters.ToArray()), typeof(object)) : obj);
                }
                else
                {
                    continueWith = typeof(Task<>).MakeGenericType(nextTask.Item1.Type.GenericTypeArguments[0]).GetGenericMethod(nameof(Task<object>.ContinueWith), typeof(Func<,>));
                    call = Expression.Call(nextTask.Item1, continueWith.MakeGenericMethod(current.Type), Expression.Lambda(current, nextTask.Item2));
                }

                return current.Type.IsTask()
                           ? Expression.Call(null, TaskUnwrapMethod.MakeGenericMethod(current.Type.GenericTypeArguments[0]), call)
                           : call;
            }

            tasks.Reverse();

            var taskList = tasks.Select(t => new { Expression = t.Item1, DeclaredParameters = new List<ParameterExpression>(), UsedParameters = new List<ParameterExpression>(), }).ToList();

            taskList.Add(new { Expression = filterExpression, DeclaredParameters = new List<ParameterExpression>(), UsedParameters = new List<ParameterExpression>() });

            var idx = 1;

            foreach (var task in tasks)
            {
                taskList[idx++].DeclaredParameters.Add(task.Item2);
            }

            foreach (var item in taskList)
            {
                new GenericVisitor
                {
                    (BlockExpression b) => item.DeclaredParameters.AddRange(b.Variables),
                    (CatchBlock b) => item.DeclaredParameters.Add(b.Variable)
                }.Visit(item.Expression);
                new GenericVisitor
                {
                    (LambdaExpression e) => e,
                    (GenericVisitor v, BlockExpression b) =>
                    {
                        v.Visit(b.Expressions);
                        return b;
                    },
                    (GenericVisitor v, CatchBlock b) =>
                    {
                        v.Visit(b.Body);
                        return b;
                    },
                    (ParameterExpression e) => item.UsedParameters.Add(e)
                }.Visit(item.Expression);
            }

            if (expression is LambdaExpression lambda2)
            {
                taskList[0].DeclaredParameters.AddRange(lambda2.Parameters);
            }

            
            
                //// Combine tasks into a chain of ContinueWith's.
            filterExpression = tasks.AsEnumerable().Reverse().Aggregate(filterExpression, CombineTasks);

            return new Expr<Task<T>>(filterExpression);
        }

        /// <summary>
        /// Converts the expr to a typed lambda expression..
        /// </summary>
        /// <param name="expr">
        /// The expr.
        /// </param>
        /// <typeparam name="T">
        /// The type of the lambda result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        public static Expression<Func<T>> ToExpression<T>(this Expr<Func<T>> expr)
        {
            return (Expression<Func<T>>)expr;
        }

        /// <summary>
        /// Converts the expr to a typed lambda expression..
        /// </summary>
        /// <param name="expr">
        /// The expr.
        /// </param>
        /// <typeparam name="T">
        /// The type of the lambda result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        public static Expression<Func<TArg, T>> ToExpression<TArg, T>(this Expr<Func<TArg, T>> expr)
        {
            return (Expression<Func<TArg, T>>)expr;
        }
    }
}