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
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// The expr.
    /// </summary>
    /// <typeparam name="T">
    /// The return type.
    /// </typeparam>
    public class Expr<T> : Expr
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Expr{T}"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public Expr([NotNull] Expression expression)
            : base(expression, typeof(T))
        {
        }
        
        /// <summary>
        /// Implicitly converts a expr to an expression.
        /// </summary>
        /// <param name="expr">
        /// The expr.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>
        /// </returns>
        [NotNull]
        public static implicit operator Expression([NotNull] Expr<T> expr)
        {
            return expr.Expression;
        }

        ///// <summary>
        ///// Implicitly converts an expression to a expr.
        ///// </summary>
        ///// <param name="expression">
        ///// The expression.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Expr{T}"/>.
        ///// </returns>
        //[NotNull]
        //public static explicit operator Expr<T>([NotNull] Expression expression)
        //{
        //    return new Expr<T>(expression);
        //}

        /// <summary>
        /// Creates a expr that calls a method on a expr.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="method">
        /// The method to call on the result of this expr.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TResult> Call<TResult>([NotNull] Expression<Func<T, TResult>> method)
        {
            return new Expr<TResult>(Expression.Call(this.Expression, GetMethodFromLambda(method)));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="property">
        /// The property to call on the result of this expr.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TResult> Property<TResult>([NotNull] Expression<Func<T, TResult>> property)
        {
            return new Expr<TResult>(Expression.Property(this.Expression, GetPropertyFromLambda(property)));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
        /// </summary>
        /// <typeparam name="TArg">
        /// The type of the argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TResult> Property<TArg, TResult>([NotNull] Expression<Func<T, TArg, TResult>> property, Expr<TArg> index)
        {
            return new Expr<TResult>(Expression.Property(this.Expression, GetPropertyFromLambda(property), index));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
        /// </summary>
        /// <param name="method">
        /// The method to call on the result of this expr.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr Call([NotNull] Expression<Action<T>> method)
        {
            return new VoidExpr(Expression.Call(this.Expression, GetMethodFromLambda(method)));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
        /// </summary>
        /// <typeparam name="TArg">
        /// The type of the argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="method">
        /// The method to call on the result of this expr.
        /// </param>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TResult> Call<TArg, TResult>([NotNull] Expression<Func<T, TArg, TResult>> method, Expr<TArg> argument)
        {
            return new Expr<TResult>(Expression.Call(this.Expression, GetMethodFromLambda(method), argument));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
        /// </summary>
        /// <typeparam name="TArg">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="method">
        /// The method to call on the result of this expr.
        /// </param>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr Call<TArg>([NotNull] Expression<Action<T, TArg>> method, Expr<TArg> argument)
        {
            return new VoidExpr(Expression.Call(this.Expression, GetMethodFromLambda(method), argument));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
        /// </summary>
        /// <typeparam name="TArg">
        /// The type of the argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="method">
        /// The method to call on the result of this expr.
        /// </param>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TResult> Call<TArg, TResult>([NotNull] Expression<Func<T, TArg, Task<TResult>>> method, Expr<TArg> argument)
        {
            return new Expr<TResult>(Expression.Call(this.Expression, GetMethodFromLambda(method), argument));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
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
        /// <param name="method">
        /// The method to call on the result of this expr.
        /// </param>
        /// <param name="argument1">
        /// The first argument.
        /// </param>
        /// <param name="argument2">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TResult> Call<TArg1, TArg2, TResult>([NotNull] Expression<Func<T, TArg1, TArg2, TResult>> method, Expr<TArg1> argument1, Expr<TArg2> argument2)
        {
            return new Expr<TResult>(Expression.Call(this.Expression, GetMethodFromLambda(method), argument1, argument2));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
        /// </summary>
        /// <typeparam name="TArg1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument.
        /// </typeparam>
        /// <param name="method">
        /// The method to call on the result of this expr.
        /// </param>
        /// <param name="argument1">
        /// The first argument.
        /// </param>
        /// <param name="argument2">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr Call<TArg1, TArg2>([NotNull] Expression<Action<T, TArg1, TArg2>> method, Expr<TArg1> argument1, Expr<TArg2> argument2)
        {
            return new VoidExpr(Expression.Call(this.Expression, GetMethodFromLambda(method), argument1, argument2));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
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
        /// <param name="method">
        /// The method to call on the result of this expr.
        /// </param>
        /// <param name="argument1">
        /// The first argument.
        /// </param>
        /// <param name="argument2">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TResult> Call<TArg1, TArg2, TResult>([NotNull] Expression<Func<T, TArg1, TArg2, Task<TResult>>> method, Expr<TArg1> argument1, Expr<TArg2> argument2)
        {
            return new Expr<TResult>(Expression.Call(this.Expression, GetMethodFromLambda(method), argument1, argument2));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
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
        /// <param name="method">
        /// The method to call on the result of this expr.
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
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TResult> Call<TArg1, TArg2, TArg3, TResult>([NotNull] Expression<Func<T, TArg1, TArg2, TArg3, TResult>> method, Expr<TArg1> argument1, Expr<TArg2> argument2, Expr<TArg3> argument3)
        {
            return new Expr<TResult>(Expression.Call(this.Expression, GetMethodFromLambda(method), argument1, argument2, argument3));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
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
        /// <param name="method">
        /// The method to call on the result of this expr.
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
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr Call<TArg1, TArg2, TArg3>([NotNull] Expression<Action<T, TArg1, TArg2, TArg3>> method, Expr<TArg1> argument1, Expr<TArg2> argument2, Expr<TArg3> argument3)
        {
            return new VoidExpr(Expression.Call(this.Expression, GetMethodFromLambda(method), argument1, argument2, argument3));
        }

        /// <summary>
        /// Creates a expr that calls a method on a expr.
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
        /// <param name="method">
        /// The method to call on the result of this expr.
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
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TResult> Call<TArg1, TArg2, TArg3, TResult>([NotNull] Expression<Func<T, TArg1, TArg2, TArg3, Task<TResult>>> method, Expr<TArg1> argument1, Expr<TArg2> argument2, Expr<TArg3> argument3)
        {
            return new Expr<TResult>(Expression.Call(this.Expression, GetMethodFromLambda(method), argument1, argument2, argument3));
        }

        /// <summary>
        /// Converts the expr into a expr that generates a function that returns the result of this expr.
        /// </summary>
        /// <returns>
        /// The <see cref="Expr"/>
        /// </returns>
        [NotNull]
        public Expr<Func<T>> ToFunc()
        {
            return new Expr<Func<T>>(Expression.Lambda(this));
        }

        /// <summary>
        /// Interprets this expr as another type.
        /// </summary>
        /// <typeparam name="TInterpret">
        /// The type to interpet this expr as.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Expr"/>.
        /// </returns>
        [NotNull]
        public Expr<TInterpret> InterpretAs<TInterpret>()
        {
            return new Expr<TInterpret>(this);
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
        private static MethodInfo GetMethodFromLambda([NotNull] LambdaExpression method)
        {
            var methodInfo =
                (method.Body as MethodCallExpression)?.Method ??
                ((method.Body as UnaryExpression)?.Operand as MethodCallExpression)?.Method;

            return methodInfo;
        }

        /// <summary>
        /// Gets a <see cref="PropertyInfo"/> from the lambda expression.
        /// </summary>
        /// <param name="property">
        /// The property to get the expression from.
        /// </param>
        /// <returns>
        /// The property info.
        /// </returns>
        [CanBeNull]
        private static PropertyInfo GetPropertyFromLambda([NotNull] LambdaExpression property)
        {
            var propertyInfo =
                (property.Body as MemberExpression)?.Member as PropertyInfo ??
                ((property.Body as UnaryExpression)?.Operand as MemberExpression)?.Member as PropertyInfo;

            return propertyInfo;
        }
    }
}
