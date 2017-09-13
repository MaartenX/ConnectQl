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

namespace ConnectQl.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Internal;
    using ConnectQl.Internal.Extensions;

    /// <summary>
    /// The ConnectQl functions extensions.
    /// </summary>
    public static class ConnectQlFunctionsExtensions
    {
        /// <summary>
        /// The <see cref="AsyncEnumerableExtensions.ApplyEnumerableFunction{TItem,TResult}"/>.
        /// </summary>
        private static readonly MethodInfo ApplyEnumerableFunction = typeof(AsyncEnumerableExtensions).GetGenericMethod("ApplyEnumerableFunction", typeof(IAsyncEnumerable<>), typeof(Func<,>));

        /// <summary>
        /// Adds a key/value pair of key'1 =&gt; function to the dictionary. The function should not have side effects, because it will be evaluated by IntelliSense.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument">
        /// The type of the function argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the function result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration1 AddWithoutSideEffects<TArgument, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, false, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'0 =&gt; function to the dictionary. The function should not have side effects, because it will be evaluated by IntelliSense.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TResult">
        /// The type of the function result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration AddWithoutSideEffects<TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, false, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'2 =&gt; function to the dictionary. The function should not have side effects, because it will be evaluated by IntelliSense.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration2 AddWithoutSideEffects<TArgument1, TArgument2, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, false, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'3 =&gt; function to the dictionary. The function should not have side effects, because it will be evaluated by IntelliSense.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration3 AddWithoutSideEffects<TArgument1, TArgument2, TArgument3, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TArgument3, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, false, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'4 =&gt; function to the dictionary. The function should not have side effects, because it will be evaluated by IntelliSense.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TArgument4">
        /// The type of the fourth argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration4 AddWithoutSideEffects<TArgument1, TArgument2, TArgument3, TArgument4, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TArgument3, TArgument4, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, false, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'5 =&gt; function to the dictionary. The function should not have side effects, because it will be evaluated by IntelliSense.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TArgument4">
        /// The type of the fourth argument.
        /// </typeparam>
        /// <typeparam name="TArgument5">
        /// The type of the fifth argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration5 AddWithoutSideEffects<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, false, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'6 =&gt; function to the dictionary. The function should not have side effects, because it will be evaluated by IntelliSense.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TArgument4">
        /// The type of the fourth argument.
        /// </typeparam>
        /// <typeparam name="TArgument5">
        /// The type of the fifth argument.
        /// </typeparam>
        /// <typeparam name="TArgument6">
        /// The type of the sixth argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration6 AddWithoutSideEffects<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, false, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'1 =&gt; function to the dictionary. The function has side effects and will not be evaluated by intellisense.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument">
        /// The type of the function argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the function result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration1 AddWithSideEffects<TArgument, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, true, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'0 =&gt; function to the dictionary.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TResult">
        /// The type of the function result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration AddWithSideEffects<TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, true, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'2 =&gt; function to the dictionary.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration2 AddWithSideEffects<TArgument1, TArgument2, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, true, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'3 =&gt; function to the dictionary.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration3 AddWithSideEffects<TArgument1, TArgument2, TArgument3, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TArgument3, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, true, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'4 =&gt; function to the dictionary.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TArgument4">
        /// The type of the fourth argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration4 AddWithSideEffects<TArgument1, TArgument2, TArgument3, TArgument4, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TArgument3, TArgument4, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, true, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'5 =&gt; function to the dictionary.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TArgument4">
        /// The type of the fourth argument.
        /// </typeparam>
        /// <typeparam name="TArgument5">
        /// The type of the fifth argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration5 AddWithSideEffects<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, true, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Adds a key/value pair of key'6 =&gt; function to the dictionary.
        /// </summary>
        /// <param name="functions">
        /// The functions to add the function to.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <typeparam name="TArgument1">
        /// The type of the first argument.
        /// </typeparam>
        /// <typeparam name="TArgument2">
        /// The type of the second argument.
        /// </typeparam>
        /// <typeparam name="TArgument3">
        /// The type of the third argument.
        /// </typeparam>
        /// <typeparam name="TArgument4">
        /// The type of the fourth argument.
        /// </typeparam>
        /// <typeparam name="TArgument5">
        /// The type of the fifth argument.
        /// </typeparam>
        /// <typeparam name="TArgument6">
        /// The type of the sixth argument.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public static IFunctionRegistration6 AddWithSideEffects<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TResult>(this IConnectQlFunctions functions, string key, Expression<Func<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5, TArgument6, TResult>> function)
        {
            var descriptor = new FunctionDescriptor(key, true, ConnectQlFunctionsExtensions.ReplaceEnumerables(functions, key, function));

            return new FunctionRegistration(descriptor, functions.AddFunction(key, descriptor));
        }

        /// <summary>
        /// Replaces all <see cref="IEnumerable{T}"/> with <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="functions">
        /// The functions.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="lambda">
        /// The lambda in which to replace the <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <returns>
        /// The new lambda.
        /// </returns>
        private static LambdaExpression ReplaceEnumerables(IConnectQlFunctions functions, string name, LambdaExpression lambda)
        {
            while (lambda.Parameters.Any(p => p.Type.HasInterface(typeof(IEnumerable<>)) && p.Type != typeof(string)))
            {
                var replacements = lambda.Parameters
                    .Select(p => new
                                     {
                                         Original = p,
                                         ReplaceBy = p.Type.HasInterface(typeof(IEnumerable<>)) && p.Type != typeof(string) ?
                                                         Expression.Parameter(typeof(IAsyncEnumerable<>).MakeGenericType(p.Type.GenericTypeArguments[0]), p.Name) :
                                                         p,
                                     })
                    .ToArray();

                var enumerableParameter = replacements.First(p => p.ReplaceBy.Type.HasInterface(typeof(IAsyncEnumerable<>)));

                functions.Logger?.Warning($"Parameter {enumerableParameter.Original.Name} of function {name} is an IEnumerable<T>, this reduces performance, consider using IAsyncEnumerable<T>.");

                lambda = Expression.Lambda(
                    GenericVisitor.Visit(
                        (MethodCallExpression e) =>
                            e.Object != enumerableParameter.Original && !e.Arguments.Contains(enumerableParameter.Original)
                                ? null
                                : TaskExpression.Task(
                                    Expression.Call(
                                        ConnectQlFunctionsExtensions.ApplyEnumerableFunction.MakeGenericMethod(enumerableParameter.Original.Type.GenericTypeArguments[0], e.Type),
                                        enumerableParameter.ReplaceBy,
                                        Expression.Lambda(e, enumerableParameter.Original))),
                        lambda.Body),
                    replacements.Select(r => r.ReplaceBy));
            }

            return lambda;
        }
    }
}