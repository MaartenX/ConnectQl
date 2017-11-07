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

namespace ConnectQl.Intellisense.Protocol
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// The enumerable comparer.
    /// </summary>
    internal static class EnumerableComparer
    {
        /// <summary>
        /// Checks if two <see cref="IEnumerable{T}"/>s are equal.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="x">
        /// The first value.
        /// </param>
        /// <param name="y">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal, <c>false</c> otherwise.
        /// </returns>
        public static bool Equals<T>(IEnumerable<T> x, IEnumerable<T> y)
        {
            return EnumerableComparer<T>.Equals(x, y);
        }

        /// <summary>
        /// Gets the hash code based on the elements in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="obj">
        /// The <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <returns>
        /// The hash code.
        /// </returns>
        public static int GetHashCode<T>(IEnumerable<T> obj)
        {
            return EnumerableComparer<T>.GetHashCode(obj);
        }
    }

    /// <summary>
    /// The array comparer.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements in the array.
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Allowed when a generic and non-generic class exist.")]
    internal class EnumerableComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableComparer{T}"/> class.
        /// </summary>
        private EnumerableComparer()
        {
        }

        /// <summary>
        /// Gets the default <see cref="IEnumerable{T}"/> comparer.
        /// </summary>
        public static IEqualityComparer<IEnumerable<T>> Default { get; } = new EnumerableComparer<T>();

        /// <summary>
        /// Checks if two <see cref="IEnumerable{T}"/>s are equal.
        /// </summary>
        /// <param name="x">
        /// The first value.
        /// </param>
        /// <param name="y">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal, <c>false</c> otherwise.
        /// </returns>
        public static bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            return EnumerableComparer<T>.Default.Equals(x, y);
        }

        /// <summary>
        /// Gets the hash code based on the elements in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <returns>
        /// The hash code.
        /// </returns>
        public static int GetHashCode(IEnumerable<T> obj)
        {
            return EnumerableComparer<T>.Default.GetHashCode(obj);
        }

        /// <summary>
        /// Checks if two <see cref="IEnumerable{T}"/>s are equal.
        /// </summary>
        /// <param name="x">
        /// The first value.
        /// </param>
        /// <param name="y">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal, <c>false</c> otherwise.
        /// </returns>
        bool IEqualityComparer<IEnumerable<T>>.Equals([CanBeNull] IEnumerable<T> x, [CanBeNull] IEnumerable<T> y)
        {
            return (x == null && y == null) || (x != null && y != null && x.SequenceEqual(y));
        }

        /// <summary>
        /// Gets the hash code based on the elements in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <returns>
        /// The hash code.
        /// </returns>
        int IEqualityComparer<IEnumerable<T>>.GetHashCode([CanBeNull] IEnumerable<T> obj)
        {
            return obj?.Aggregate(0, (hashCode, item) => (hashCode * 397) ^ (item?.GetHashCode() ?? 0)) ?? 0;
        }
    }
}