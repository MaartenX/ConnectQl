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

namespace ConnectQl.AsyncEnumerables
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The OrderedAsyncEnumerable interface.
    /// </summary>
    /// <typeparam name="TElement">
    /// The type of the elements in the enumerable.
    /// </typeparam>
    public interface IOrderedAsyncEnumerable<out TElement> : IAsyncEnumerable<TElement>
    {
        /// <summary>
        /// Performs a subsequent ordering on the elements of an <see cref="IOrderedAsyncEnumerable{T}"/> according to a key.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key produced by <paramref name="keySelector"/>.
        /// </typeparam>
        /// <param name="keySelector">
        /// The <see cref="Func{T, TResult}"/> used to extract the key for each element.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> used to compare keys for placement in the returned sequence.
        /// </param>
        /// <param name="descending">
        /// <c>true</c> to sort the elements in descending order; <c>false</c> to sort the elements in ascending order.
        /// </param>
        /// <returns>
        /// An <see cref="IOrderedAsyncEnumerable{TElement}"/> whose elements are sorted according to a key.
        /// </returns>
        IOrderedAsyncEnumerable<TElement> CreateOrderedAsyncEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
    }
}