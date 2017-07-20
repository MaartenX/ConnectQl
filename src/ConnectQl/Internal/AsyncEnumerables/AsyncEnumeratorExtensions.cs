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

namespace ConnectQl.Internal.AsyncEnumerables
{
    using System;
    using System.Collections.Generic;

    using ConnectQl.AsyncEnumerables;

    /// <summary>
    /// The async enumerator extensions.
    /// </summary>
    internal static class AsyncEnumeratorExtensions
    {
        /// <summary>
        /// Converts the current batch of the <see cref="IAsyncEnumerator{T}"/> to an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="asyncEnumerator">
        /// The async enumerator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <paramref name="asyncEnumerator"/> was moved to another batch.
        /// </exception>
        public static IEnumerable<T> CurrentBatchToEnumerable<T>(this IAsyncEnumerator<T> asyncEnumerator)
        {
            while (asyncEnumerator.MoveNext())
            {
                yield return asyncEnumerator.Current;
            }
        }

        /// <summary>
        /// Converts the current batch of the <see cref="IAsyncEnumerator{T}"/> to an array.
        /// </summary>
        /// <param name="asyncEnumerator">
        /// The async enumerator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The array.
        /// </returns>
        internal static T[] CurrentBatchToArray<T>(this IAsyncEnumerator<T> asyncEnumerator)
        {
            var result = new T[4];
            var index = 0;

            while (asyncEnumerator.MoveNext())
            {
                result[index++] = asyncEnumerator.Current;

                if (index != result.Length)
                {
                    continue;
                }

                var newArray = new T[result.Length * 2];

                Array.Copy(result, newArray, result.Length);

                result = newArray;
            }

            if (index != result.Length)
            {
                Array.Resize(ref result, index);
            }

            return result;
        }

        /// <summary>
        /// Converts the current batch of the <see cref="IAsyncEnumerator{T}"/> to an <see cref="IEnumerator{T}"/>.
        /// </summary>
        /// <param name="asyncEnumerator">
        /// The async enumerator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerator{T}"/>.
        /// </returns>
        internal static IEnumerator<T> CurrentBatchToEnumerator<T>(this IAsyncEnumerator<T> asyncEnumerator)
        {
            while (asyncEnumerator.MoveNext())
            {
                yield return asyncEnumerator.Current;
            }
        }
    }
}