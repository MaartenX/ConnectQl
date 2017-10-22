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

namespace ConnectQl.Tools.Extensions
{
    using System;
    using System.Linq;

    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The classification token extensions.
    /// </summary>
    public static class ClassificationTokenExtensions
    {
        /// <summary>
        /// Checks if the token has the specified value.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool In(this IClassifiedToken token, params string[] values)
        {
            return token.In(StringComparison.OrdinalIgnoreCase, values);
        }

        /// <summary>
        /// Checks if the token has the specified value.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        /// <param name="values">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool In(this IClassifiedToken token, StringComparison comparison, [NotNull] params string[] values)
        {
            return values.Any(v => token.Value.Equals(v, comparison));
        }

        /// <summary>
        /// Checks if the token has the specified value.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Is([NotNull] this IClassifiedToken token, [NotNull] string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return value.Equals(token.Value, comparison);
        }
    }
}