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

namespace ConnectQl.Internal.AsyncEnumerables.Enumerators
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;

    using JetBrains.Annotations;

    /// <summary>
    /// Enumerator used by the <see cref="AsyncEnumerableExtensions.OrderBy{T}"/> method.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source elements.
    /// </typeparam>
    internal class OrderedEnumerator<TSource> : AsyncEnumeratorBase<TSource>
    {
        /// <summary>
        /// The enumerator.
        /// </summary>
        private IAsyncEnumerator<TSource> asyncEnumerator;

        /// <summary>
        /// The comparison.
        /// </summary>
        private Comparison<TSource> comparison;

        /// <summary>
        /// The source.
        /// </summary>
        private IAsyncEnumerable<TSource> source;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedEnumerator{TSource}"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        public OrderedEnumerator(IAsyncEnumerable<TSource> source, Comparison<TSource> comparison)
        {
            this.source = source;
            this.comparison = comparison;
        }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
        /// </summary>
        public override bool IsSynchronous => false;

        /// <summary>
        /// Resets all fields to null, and sets the state to 'disposed'.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.state = 3;

            this.asyncEnumerator?.Dispose();

            this.asyncEnumerator = null;
            this.source = null;
            this.comparison = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        [CanBeNull]
        protected override IEnumerator<TSource> InitialBatch()
        {
            return null;
        }

        /// <summary>
        /// Gets called when the next batch is needed.
        /// </summary>
        /// <returns>
        /// A task returning an <see cref="IEnumerator{T}"/> containing the next batch, of <c>null</c> when all data is
        ///     enumerated.
        /// </returns>
        protected override async Task<IEnumerator<TSource>> OnNextBatchAsync()
        {
            switch (this.state)
            {
                case 0:
                    this.asyncEnumerator = (await this.source.Policy.SortAsync(this.source, this.comparison).ConfigureAwait(false)).GetAsyncEnumerator();
                    this.state = 1;

                    return this.EnumerateItems();
                case 1:
                    if (!await this.asyncEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 2;
                        return null;
                    }

                    return this.EnumerateItems();
                case 2:
                    return null;
                case 3:
                    throw new ObjectDisposedException(this.GetType().ToString());
                default:
                    throw new InvalidOperationException($"Invalid state: {this.state}.");
            }
        }

        /// <summary>
        /// Enumerates the items.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator{TSource}"/>.
        /// </returns>
        private IEnumerator<TSource> EnumerateItems()
        {
            while (this.asyncEnumerator.MoveNext())
            {
                yield return this.asyncEnumerator.Current;
            }

            if (this.asyncEnumerator.IsSynchronous)
            {
                this.state = 2;
            }
        }
    }
}