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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.ExtensionMethods;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// Base class for typed expression factories.
    /// </summary>
    [PublicAPI]
    public abstract class Expr
    {
        /// <summary>
        /// Returns a value indicating whether this expr contains tasks.
        /// </summary>
        private readonly Lazy<bool> hasTasks;

        private static readonly MethodInfo TaskFromResultMethod = ((Func<object, Task<object>>)Task.FromResult).GetMethodInfo().GetGenericMethodDefinition();

        /// <summary>
        /// Initializes a new instance of the <see cref="Expr"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="targetType">
        /// The type the resulting expr will have.
        /// </param>
        protected Expr([NotNull] Expression expression, [NotNull] Type targetType)
        {
            if (targetType.IsTask())
            {
                this.Expression = expression.Type.IsTask() 
                    ? expression 
                    : Expression.Call(Expr.TaskFromResultMethod.MakeGenericMethod(targetType.GenericTypeArguments.First()), expression);
            }
            else
            {
                this.Expression = expression.Type.IsTask()
                                      ? ConnectQlExpression.MakeTask(expression)
                                      : expression;
            }

            Debug.Assert(this.Expression != null, "Null");
         
            var tasksFound = false;

            this.hasTasks = new Lazy<bool>(() => GenericVisitor.Visit(
                                                     (TaskExpression t) =>
                                                     {
                                                         tasksFound = true;
                                                         return t;
                                                     },
                                                     (LambdaExpression e) => e == this.Expression ? null : e,
                                                     expression) != null && tasksFound);
        }

        /// <summary>
        /// Gets a value indicating whether this expr contains tasks.
        /// </summary>
        public bool HasTasks => this.hasTasks.Value;

        /// <summary>
        /// Gets the expression.
        /// </summary>
        protected Expression Expression { get; }

        /// <summary>
        /// Gets the type of this expr.
        /// </summary>
        private Type Type => this.Expression.Type;

        /// <summary>
        /// Converts the expression to a expr that returns an object. When the expression throws an exception,
        /// an <see cref="Error"/> object is returned that wraps the exception.
        /// </summary>
        /// <param name="expression">
        /// The expression to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<object> Object(Expression expression)
        {
            var exception = Expr.Parameter<Exception>("e");

            return new Expr<object>(Expression.TryCatch(
                                        new Expr<object>(expression).ConvertTo<object>(),
                                        Expression.Catch(exception, Expr.Create(e => new Error(e), exception).ConvertTo<object>())));
        }

        /// <summary>
        /// Creates a expr that creates an instance using the default parameterless constructor.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the expr to create.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Create<T>()
            where T : new()
        {
            return new Expr<T>(Expression.New(typeof(T).GetConstructor()));
        }

        /// <summary>
        /// Creates a expr that creates an instance using the specified constructor.
        /// </summary>
        /// <param name="constructor">
        /// The constructor to use.
        /// </param>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <typeparam name="TArg">
        /// The type of the argument.
        /// </typeparam>
        /// <typeparam name="T">
        /// The type of the expr to create.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Create<TArg, T>([NotNull] Expression<Func<TArg, T>> constructor, [NotNull] Expr<TArg> argument)
        {
            return new Expr<T>(Expression.New(GetConstructorFromLambda(constructor), argument.Expression));
        }

        /// <summary>
        /// Creates a expr that creates an instance using the specified constructor.
        /// </summary>
        /// <param name="constructor">
        /// The constructor to use.
        /// </param>
        /// <param name="argument1">
        /// The first argument.
        /// </param>
        /// <param name="argument2">
        /// The second argument.
        /// </param>
        /// <typeparam name="TArg1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="T">
        /// The type of the expr to create.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Create<TArg1, TArg2, T>([NotNull] Expression<Func<TArg1, TArg2, T>> constructor, [NotNull] Expr<TArg1> argument1, Expr<TArg2> argument2)
        {
            return new Expr<T>(Expression.New(GetConstructorFromLambda(constructor), argument1.Expression, argument2.Expression));
        }

        /// <summary>
        /// Creates a expr that creates an instance using the specified constructor.
        /// </summary>
        /// <param name="constructor">
        /// The constructor to use.
        /// </param>
        /// <param name="argument1">
        /// The first argument.
        /// </param>
        /// <param name="argument2">
        /// The second argument.
        /// </param>
        /// <param name="argument3">
        /// The third argument.
        /// </param>
        /// <typeparam name="TArg1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArg3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="T">
        /// The type of the expr to create.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Create<TArg1, TArg2, TArg3, T>([NotNull] Expression<Func<TArg1, TArg2, TArg3, T>> constructor, [NotNull] Expr<TArg1> argument1, Expr<TArg2> argument2, Expr<TArg3> argument3)
        {
            return new Expr<T>(Expression.New(GetConstructorFromLambda(constructor), argument1.Expression, argument2.Expression, argument3.Expression));
        }

        /// <summary>
        /// Creates a expr that returns the default value for the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the default value.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Default<T>()
        {
            return new Expr<T>(Expression.Default(typeof(T)));
        }
        
        /// <summary>
        /// Creates a expr that calls the specified method with the specified arguments.
        /// </summary>
        /// <param name="method">
        /// The method to call.
        /// </param>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <typeparam name="TArg">
        /// The type of the argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result of the method.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<TResult> Call<TArg, TResult>(Func<TArg, TResult> method, Expr<TArg> argument)
        {
            return new Expr<TResult>(Expression.Call(method.GetMethodInfo(), argument));
        }

        /// <summary>
        /// Creates a expr that calls the specified method with the specified arguments.
        /// </summary>
        /// <param name="method">
        /// The method to call.
        /// </param>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <typeparam name="TArg">
        /// The type of the argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result of the method.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<TResult> Call<TArg, TResult>(Func<TArg, Task<TResult>> method, Expr<TArg> argument)
        {
            return new Expr<TResult>(Expression.Call(method.GetMethodInfo(), argument));
        }

        /// <summary>
        /// Creates a expr that calls the specified method with the specified arguments.
        /// </summary>
        /// <param name="method">
        /// The method to call.
        /// </param>
        /// <param name="argument1">
        /// The first argument.
        /// </param>
        /// <param name="argument2">
        /// The second argument.
        /// </param>
        /// <typeparam name="TArg1">
        /// The type of the first argument of the method.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result of the method.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<TResult> Call<TArg1, TArg2, TResult>(Func<TArg1, TArg2, TResult> method, Expr<TArg1> argument1, Expr<TArg2> argument2)
        {
            return new Expr<TResult>(Expression.Call(method.GetMethodInfo(), argument1, argument2));
        }

        /// <summary>
        /// Creates a expr that calls the specified method with the specified arguments.
        /// </summary>
        /// <param name="method">
        /// The method to call.
        /// </param>
        /// <param name="argument1">
        /// The first argument.
        /// </param>
        /// <param name="argument2">
        /// The second argument.
        /// </param>
        /// <param name="argument3">
        /// The third argument.
        /// </param>
        /// <typeparam name="TArg1">
        /// The type of the first argument of the method.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument of the method.
        /// </typeparam>
        /// <typeparam name="TArg3">
        /// The type of the third argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result of the method.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<TResult> Call<TArg1, TArg2, TArg3, TResult>(Func<TArg1, TArg2, TArg3, TResult> method, Expr<TArg1> argument1, Expr<TArg2> argument2, Expr<TArg3> argument3)
        {
            return new Expr<TResult>(Expression.Call(null, method.GetMethodInfo(), argument1, argument2, argument3));
        }

        /// <summary>
        /// Creates a expr that calls the specified method with the specified arguments.
        /// </summary>
        /// <param name="method">
        /// The method to call.
        /// </param>
        /// <param name="argument1">
        /// The first argument.
        /// </param>
        /// <param name="argument2">
        /// The second argument.
        /// </param>
        /// <param name="argument3">
        /// The third argument.
        /// </param>
        /// <param name="argument4">
        /// The fourth argument.
        /// </param>
        /// <typeparam name="TArg1">
        /// The type of the first argument of the method.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument of the method.
        /// </typeparam>
        /// <typeparam name="TArg3">
        /// The type of the third argument of the method.
        /// </typeparam>
        /// <typeparam name="TArg4">
        /// The type of the fourth argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result of the method.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<TResult> Call<TArg1, TArg2, TArg3, TArg4, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TResult> method, Expr<TArg1> argument1, Expr<TArg2> argument2, Expr<TArg3> argument3, Expr<TArg4> argument4)
        {
            return new Expr<TResult>(Expression.Call(method.GetMethodInfo(), argument1, argument2));
        }

        /// <summary>
        /// Creates a expr that creates the constant expression.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the result.
        /// </typeparam>
        /// <param name="value">
        /// The constant expression to return.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Constant<T>(Task<T> value)
        {
            return new Expr<T>(Expression.Constant(value));
        }
        
        /// <summary>
        /// Creates a expr that creates the constant expression.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the result.
        /// </typeparam>
        /// <param name="value">
        /// The constant expression to return.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Constant<T>(T value)
        {
            return new Expr<T>(Expression.Constant(value, typeof(T)));
        }

        /// <summary>
        /// Creates an expr that contains all specified factories.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the last expr that will be the return type.
        /// </typeparam>
        /// <param name="item">
        /// The first expr.
        /// </param>
        /// <param name="returnStatement">
        /// The return statement.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Block<T>([NotNull] Expr item, [NotNull] Expr<T> returnStatement)
        {
            return new Expr<T>(Expression.Block(
                item.Expression,
                returnStatement.Expression));
        }

        /// <summary>
        /// Creates an expr that contains all specified factories.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the last expr that will be the return type.
        /// </typeparam>
        /// <param name="item">
        /// The first expr.
        /// </param>
        /// <param name="item2">
        /// The second expr.
        /// </param>
        /// <param name="returnStatement">
        /// The return statement.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Block<T>([NotNull] Expr item, [NotNull] Expr item2, [NotNull] Expr<T> returnStatement)
        {
            return new Expr<T>(Expression.Block(
                item.Expression,
                item2.Expression,
                returnStatement.Expression));
        }
        
        /// <summary>
        /// Creates an expr that contains all specified factories.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the last expr that will be the return type.
        /// </typeparam>
        /// <param name="items">
        /// The factories.
        /// </param>
        /// <param name="returnStatement">
        /// The return statement.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T> Block<T>([NotNull] IEnumerable<Expr> items, [NotNull] Expr<T> returnStatement)
        {
            return new Expr<T>(Expression.Block(items.Select(f => f.Expression).Concat(new[] { returnStatement.Expression })));
        }

        /// <summary>
        /// Creates an expr that is a lambda parameter.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parameter.
        /// </typeparam>
        /// <param name="name">
        /// The name of the parameter.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static ParameterExpr<T> Parameter<T>([CanBeNull] string name = null)
        {
            return new ParameterExpr<T>(Expression.Parameter(typeof(T), name));
        }

        /// <summary>
        /// Creates a expr that creates an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="items">
        /// The factories that create the elements.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<T[]> Array<T>([NotNull] IEnumerable<Expr<T>> items)
        {
            var itemArray = items.ToArray();

            return new Expr<T[]>(itemArray.Length == 0
                                      ? Expression.NewArrayBounds(typeof(T), Expression.Constant(0)) as Expression
                                      : Expression.NewArrayInit(typeof(T), itemArray.Select<Expr<T>, Expression>(e => e)));
        }

        /// <summary>
        /// Creates a expr that creates an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="items">
        /// The factories that create the elements.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<IEnumerable<T>> Enumerable<T>([NotNull] IEnumerable<Expr<T>> items)
        {
            return Array(items).ConvertTo<IEnumerable<T>>();
        }

        /// <summary>
        /// Replaces the parameters in a lambda expression and returns the body.
        /// </summary>
        /// <typeparam name="TArg1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="lambda">
        /// The lambda expression.
        /// </param>
        /// <param name="replace">
        /// The expr to replace the first parameter with.
        /// </param>
        /// <param name="replace2">
        /// The expr to replace the second parameter with.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<TResult> ReplaceLambdaParameters<TArg1, TArg2, TResult>([NotNull] Expression<Func<TArg1, TArg2, TResult>> lambda, Expr<TArg1> replace, Expr<TArg2> replace2)
        {
            return new Expr<TResult>(lambda.Body.ReplaceParameter(lambda.Parameters[0], replace)
                .ReplaceParameter(lambda.Parameters[1], replace2));
        }

        /// <summary>
        /// Replaces the parameters in a lambda expression and returns the body.
        /// </summary>
        /// <typeparam name="TArg1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArg3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="lambda">
        /// The lambda expression.
        /// </param>
        /// <param name="replace">
        /// The expr to replace the first parameter with.
        /// </param>
        /// <param name="replace2">
        /// The expr to replace the second parameter with.
        /// </param>
        /// <param name="replace3">
        /// The expr to replace the third parameter with.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public static Expr<TResult> ReplaceLambdaParameters<TArg1, TArg2, TArg3, TResult>([NotNull] Expression<Func<TArg1, TArg2, TArg3, TResult>> lambda, Expr<TArg1> replace, Expr<TArg2> replace2, Expr<TArg3> replace3)
        {
            return new Expr<TResult>(lambda.Body.ReplaceParameter(lambda.Parameters[0], replace)
                .ReplaceParameter(lambda.Parameters[1], replace2)
                .ReplaceParameter(lambda.Parameters[2], replace3));
        }

        /// <summary>
        /// Converts the expr to a new type by casting.
        /// </summary>
        /// <typeparam name="T">
        /// The type to convert the result of the expr to.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<T> ConvertTo<T>()
        {
            return this.Type == typeof(T) ? (Expr<T>)this : new Expr<T>(Expression.Convert(this.Expression, typeof(T)));
        }

        /// <summary>
        /// Gets a <see cref="MethodInfo"/> from the lambda expression.
        /// </summary>
        /// <param name="method">
        /// The method to get the expression from.
        /// </param>
        /// <returns>
        /// The method info.
        /// </returns>
        [CanBeNull]
        private static ConstructorInfo GetConstructorFromLambda([NotNull] LambdaExpression method)
        {
            var constructorInfo =
                (method.Body as NewExpression)?.Constructor ??
                ((method.Body as UnaryExpression)?.Operand as NewExpression)?.Constructor;

            return constructorInfo;
        }

        /// <summary>
        /// Implements a expr without a return type.
        /// </summary>
        protected class VoidExpr : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see creExpr.Voidexpr"/> class.
            /// </summary>
            /// <param name="expression">
            /// The expression.
            /// </param>
            public VoidExpr([NotNull] Expression expression)
                : base(expression, typeof(void))
            {
            }
        }
    }
}
