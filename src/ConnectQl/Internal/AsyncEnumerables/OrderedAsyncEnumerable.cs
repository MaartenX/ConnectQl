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

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Internal.AsyncEnumerables.Enumerators;

    using JetBrains.Annotations;

    /// <summary>
    /// The ordered async enumerable.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements.
    /// </typeparam>
    internal class OrderedAsyncEnumerable<T> : IOrderedAsyncEnumerable<T>
    {
        /// <summary>
        /// The lambda used to compare two items.
        /// </summary>
        private readonly Comparison<T> comparison;

        /// <summary>
        /// The source enumerable.
        /// </summary>
        private readonly IAsyncEnumerable<T> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedAsyncEnumerable{T}"/> class.
        /// </summary>
        /// <param name="source">
        /// The async enumerable.
        /// </param>
        /// <param name="comparison">
        /// The compare lambda.
        /// </param>
        public OrderedAsyncEnumerable(IAsyncEnumerable<T> source, Comparison<T> comparison)
        {
            this.source = source;
            this.comparison = comparison;
        }

        /// <summary>
        /// Gets the materialization policy.
        /// </summary>
        public IMaterializationPolicy Policy => this.source.Policy;

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
        [NotNull]
        public IOrderedAsyncEnumerable<T> CreateOrderedAsyncEnumerable<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            Comparison<T> newComparer = (first, second) =>
                {
                    var result = this.comparison(first, second);

                    return result != 0
                               ? result
                               : (comparer ?? Comparer<TKey>.Default).Compare(keySelector(first), keySelector(second)) * (descending ? -1 : 1);
                };

            return new OrderedAsyncEnumerable<T>(this.source, newComparer);
        }

        /// <summary>
        /// Gets an enumerator that returns batches of elements.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        [NotNull]
        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new OrderedEnumerator<T>(this.source, this.comparison);
        }
    }
}