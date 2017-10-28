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
    using System.Linq.Expressions;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// Contains methods used in binary and unary operators.
    /// </summary>
    internal abstract class Operator
    {
        /// <summary>
        /// The string comparision mode that will be used in operators.
        /// </summary>
        protected static readonly StringComparison StringComparisionMode = StringComparison.Ordinal;

        /// <summary>
        /// When two expressions do not have the same type, one of the two has to be cast to the other type. This array holds
        ///     the most important type to the least important one.
        /// </summary>
        protected static readonly Type[] CommonTypeOrder =
            {
                typeof(double), typeof(decimal), typeof(float), typeof(ulong), typeof(long), typeof(uint), typeof(int), typeof(ushort), typeof(short), typeof(char), typeof(byte)
            };

        /// <summary>
        /// Converts an expression into an object.
        /// </summary>
        /// <param name="expression">
        /// The expression to convert.
        /// </param>
        /// <returns>
        /// The expression, or a new one if conversion is needed.
        /// </returns>
        public static Expression ToObject([NotNull] Expression expression)
        {
            return Operator.ToType(expression, typeof(object));
        }

        /// <summary>
        /// Converts an expression into a string.
        /// </summary>
        /// <param name="expression">
        /// The expression to convert.
        /// </param>
        /// <returns>
        /// The expression, or a new one if conversion is needed.
        /// </returns>
        protected static Expression ToString([NotNull] Expression expression)
        {
            if (expression.Type == typeof(string))
            {
                return expression;
            }

            var method = typeof(Convert).GetRuntimeMethod("ToString", new[] { expression.Type, });

            return method != null
                       ? Expression.Call(method, expression)
                       : Expression.Call(typeof(Convert).GetRuntimeMethod("ToString", new[] { typeof(object) }), Expression.Convert(expression, typeof(object)));
        }

        /// <summary>
        /// Converts an expression into another type.
        /// </summary>
        /// <param name="expression">
        /// The expression to convert.
        /// </param>
        /// <param name="type">
        /// The type to convert to.
        /// </param>
        /// <returns>
        /// The expression, or a new one if conversion is needed.
        /// </returns>
        protected static Expression ToType([NotNull] Expression expression, Type type)
        {
            return expression.Type == type ? expression : Expression.Convert(expression, type);
        }
    }
}