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

namespace ConnectQl.Internal.Validation.Operators
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Results;

    /// <summary>
    /// The converter.
    /// </summary>
    internal static class Converter
    {
        /// <summary>
        /// The <see cref="AsyncEnumerableExtensions.Convert{T}"/> method.
        /// </summary>
        private static readonly MethodInfo AsyncEnumerableExtensionsConvertMethod = typeof(AsyncEnumerableExtensions).GetGenericMethod(nameof(AsyncEnumerableExtensions.Convert), typeof(IAsyncEnumerable));

        /// <summary>
        /// Converts an expression to the specified type.
        /// </summary>
        /// <param name="from">
        /// The expression to convert.
        /// </param>
        /// <param name="to">
        /// The type to convert to.
        /// </param>
        /// <returns>
        /// The <paramref name="from"/> or a new expression if conversion was necessary.
        /// </returns>
        public static Expression Convert(Expression from, Type to)
        {
            if (from.Type == to)
            {
                return from;
            }

            if (typeof(IAsyncEnumerable).GetTypeInfo().IsAssignableFrom(to.GetTypeInfo()))
            {
                return Expression.Call(
                    Converter.AsyncEnumerableExtensionsConvertMethod.MakeGenericMethod(to.GenericTypeArguments[0]),
                    Expression.Convert(from, typeof(IAsyncEnumerable)));
            }

            if (to != typeof(string))
            {
                return Expression.Convert(from, to);
            }

            var method = typeof(Convert).GetRuntimeMethod("ToString", new[] { from.Type, });

            var paramType = method?.GetParameters()[0].ParameterType;

            return method != null
                       ? Expression.Call(method, from.Type == paramType ? from : Expression.Convert(from, paramType))
                       : Expression.Call(typeof(Convert).GetRuntimeMethod("ToString", new[] { typeof(object), }), Expression.Convert(from, typeof(object)));
        }

        /// <summary>
        /// Checks if conversion can be done to the specified type.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="from">
        /// The type to convert from.
        /// </param>
        /// <param name="to">
        /// The type to convert to.
        /// </param>
        /// <exception cref="NodeException">
        /// Thrown when conversion fails.
        /// </exception>
        public static void ValidateConversion(Node node, Type from, Type to)
        {
            try
            {
                if (from.GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(IDataSource)) &&
                    to == typeof(IAsyncEnumerable<Row>))
                {
                    return;
                }

                Converter.Convert(Expression.Parameter(from), to);
            }
            catch (Exception e)
            {
                throw new NodeException(node, $"Cannot convert type {from} to {to}.", e);
            }
        }
    }
}