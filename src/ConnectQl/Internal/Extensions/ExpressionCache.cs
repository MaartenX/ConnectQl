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

#if DEBUG

namespace ConnectQl.Internal.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    ///     The expression cache.
    /// </summary>
    internal static class ExpressionCache
    {
        /// <summary>
        ///     The expressions.
        /// </summary>
        private static readonly Dictionary<Delegate, LambdaExpression> Expressions = new Dictionary<Delegate, LambdaExpression>();

        /// <summary>
        ///     The get.
        /// </summary>
        /// <param name="func">
        ///     The func.
        /// </param>
        /// <returns>
        ///     The <see cref="LambdaExpression" />.
        /// </returns>
        [CanBeNull]
        public static LambdaExpression Get(Delegate func)
        {
            return ExpressionCache.Expressions.TryGetValue(func, out LambdaExpression result) ? result : null;
        }

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <param name="func">
        ///     The func.
        /// </param>
        /// <typeparam name="T">
        /// The type of the function.
        /// </typeparam>
        /// <returns>
        ///     The <typeparamref name="T" />.
        /// </returns>
        public static T Set<T>(LambdaExpression expression, T func)
        {
            ExpressionCache.Expressions[(Delegate)(object)func] = expression;

            return func;
        }
    }
}

#endif
