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

namespace ConnectQl.Internal.Comparers
{
    using System.Collections.Generic;

    /// <summary>
    /// Helper to create default comparers.
    /// </summary>
    public static class DefaultComparer
    {
        /// <summary>
        /// Creates the comparer instance.
        /// </summary>
        /// <typeparam name="T">
        /// The type to create the comparer for.
        /// </typeparam>
        /// <returns>
        /// The comparer.
        /// </returns>
        public static IComparer<T> Create<T>()
        {
            return typeof(T) == typeof(object) ? (Comparer<T>)(object)new ObjectComparer() : Comparer<T>.Default;
        }

        /// <summary>
        /// Comparer for objects, compares them by string representation.
        /// </summary>
        private class ObjectComparer : Comparer<object>
        {
            /// <summary>
            /// The string comparer.
            /// </summary>
            private static readonly Comparer<string> StringComparer = Comparer<string>.Default;

            /// <summary>
            /// Performs a comparison of two objects of the same type and returns a value indicating whether one object is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero <paramref name="x" /> is less than <paramref name="y" />.Zero <paramref name="x" /> equals <paramref name="y" />.Greater than zero <paramref name="x" /> is greater than <paramref name="y" />.
            /// </returns>
            public override int Compare(object x, object y)
            {
                return x == null ? y == null ? 0 : -1 : y == null ? 1 : StringComparer.Compare(x.ToString(), y.ToString());
            }
        }
    }
}
