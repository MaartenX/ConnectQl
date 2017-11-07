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

namespace ConnectQl.AsyncEnumerables.Enumerators
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Enumerator used by the <see cref="AsyncEnumerableExtensions.GroupBy{TSource,TKey}"/> method.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source elements.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the keys.
    /// </typeparam>
    internal class GroupByEnumerator<TSource, TKey> : AsyncEnumeratorBase<IAsyncGrouping<TSource, TKey>>
    {
        /// <summary>
        /// The comparer.
        /// </summary>
        private IComparer<TKey> comparer;

        /// <summary>
        /// The enumerator.
        /// </summary>
        private IAsyncEnumerator<SourceKey<TSource, TKey>> enumerator;

        /// <summary>
        /// The key selector.
        /// </summary>
        private Func<TSource, TKey> keySelector;

        /// <summary>
        /// The last key.
        /// </summary>
        private TKey lastKey;

        /// <summary>
        /// The last offset.
        /// </summary>
        private long lastOffset;

        /// <summary>
        /// The offset.
        /// </summary>
        private long offset;

        /// <summary>
        /// The sorted.
        /// </summary>
        private IAsyncReadOnlyCollection<SourceKey<TSource, TKey>> sorted;

        /// <summary>
        /// The source.
        /// </summary>
        private IAsyncEnumerable<SourceKey<TSource, TKey>> source;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupByEnumerator{TSource,TKey}"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public GroupByEnumerator(IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            this.source = source.Select(s => new SourceKey<TSource, TKey>(s, keySelector(s)));
            this.keySelector = keySelector;
            this.comparer = comparer;
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

            this.enumerator?.Dispose();

            this.source = null;
            this.keySelector = null;
            this.comparer = null;
            this.sorted = null;
            this.enumerator = null;
            this.lastKey = default(TKey);
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        [CanBeNull]
        protected override IEnumerator<IAsyncGrouping<TSource, TKey>> InitialBatch()
        {
            return null;
        }

        /// <summary>
        /// Moves to the next batch. Implemented as a state machine.
        /// </summary>
        /// <returns>
        /// <c>true</c> if another batch is available, <c>false</c> otherwise.
        /// </returns>
        protected override async Task<IEnumerator<IAsyncGrouping<TSource, TKey>>> OnNextBatchAsync()
        {
            switch (this.state)
            {
                case 0: // Initial, sort the source, and return the first groupings.
                    Comparison<SourceKey<TSource, TKey>> comparison = (first, second) => this.comparer.Compare(first.Key, second.Key);
                    this.sorted = await this.source.Policy.SortAsync(this.source, comparison).ConfigureAwait(false);
                    this.enumerator = this.sorted.GetAsyncEnumerator();
                    this.state = 1;

                    return this.EnumerateGroupings();
                case 1: // Continue, return the next groupings.
                    if (!await this.enumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 2;

                        return this.offset == this.lastOffset
                                   ? null
                                   : GroupByEnumerator<TSource, TKey>.EnumerateItem(new AsyncGrouping<TSource, TKey>(this.lastKey, this.sorted, this.lastOffset, this.offset - this.lastOffset));
                    }

                    return this.EnumerateGroupings();
                case 2: // Done, no more groupings available.
                    return null;
                case 3: // Disposed, throw error.
                    throw new ObjectDisposedException(this.GetType().ToString());
                default: // Shouldn't happen.
                    throw new InvalidOperationException($"Invalid state: {this.state}.");
            }
        }

        /// <summary>
        /// Enumerates the item.
        /// </summary>
        /// <param name="asyncGrouping">
        /// The async grouping.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerator{T}"/>.
        /// </returns>
        private static IEnumerator<IAsyncGrouping<TSource, TKey>> EnumerateItem(IAsyncGrouping<TSource, TKey> asyncGrouping)
        {
            yield return asyncGrouping;
        }

        /// <summary>
        /// Enumerates the groupings in a batch.
        /// </summary>
        /// <returns>
        /// The groupings.
        /// </returns>
        private IEnumerator<AsyncGrouping<TSource, TKey>> EnumerateGroupings()
        {
            while (this.enumerator.MoveNext())
            {
                var key = this.enumerator.Current.Key;

                if (this.comparer.Compare(key, this.lastKey) != 0)
                {
                    if (this.offset != 0)
                    {
                        yield return new AsyncGrouping<TSource, TKey>(key, this.sorted, this.lastOffset, this.offset - this.lastOffset);

                        this.lastOffset = this.offset;
                    }

                    this.lastKey = key;
                }

                this.offset++;
            }

            if (!this.enumerator.IsSynchronous)
            {
                yield break;
            }

            if (this.offset != this.lastOffset)
            {
                yield return new AsyncGrouping<TSource, TKey>(this.lastKey, this.sorted, this.lastOffset, this.offset - this.lastOffset);
            }

            this.state = 2;
        }
    }
}