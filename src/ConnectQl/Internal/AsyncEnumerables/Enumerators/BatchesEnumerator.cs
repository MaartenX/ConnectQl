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

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;

    /// <summary>
    /// Enumerator used by the <see cref="AsyncEnumerableExtensions.Batch{TSource}"/> method.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source elements.
    /// </typeparam>
    internal class BatchesEnumerator<TSource> : AsyncEnumeratorBase<IAsyncEnumerable<TSource>>
    {
        /// <summary>
        /// The batch size.
        /// </summary>
        private readonly long batchSize;

        /// <summary>
        /// The enumerator.
        /// </summary>
        private IAsyncEnumerator<TSource> enumerator;

        /// <summary>
        /// The sorted.
        /// </summary>
        private IAsyncReadOnlyCollection<TSource> materialized;

        /// <summary>
        /// The offset.
        /// </summary>
        private long offset;

        /// <summary>
        /// The source.
        /// </summary>
        private IAsyncEnumerable<TSource> source;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchesEnumerator{TSource}"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="batchSize">
        /// The batch size.
        /// </param>
        public BatchesEnumerator(IAsyncEnumerable<TSource> source, long batchSize)
        {
            this.source = source;
            this.batchSize = batchSize;
            this.materialized = source as IAsyncReadOnlyCollection<TSource>;

            if (this.materialized != null)
            {
                this.enumerator = this.materialized.GetAsyncEnumerator();
                this.IsSynchronous = this.enumerator.IsSynchronous;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
        /// </summary>
        public override bool IsSynchronous { get; }

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
            this.materialized = null;
            this.enumerator = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        protected override IEnumerator<IAsyncEnumerable<TSource>> InitialBatch()
        {
            return this.enumerator == null ? null : this.EnumerateBatches();
        }

        /// <summary>
        /// Moves to the next batch. Implemented as a state machine.
        /// </summary>
        /// <returns>
        /// <c>true</c> if another batch is available, <c>false</c> otherwise.
        /// </returns>
        protected override async Task<IEnumerator<IAsyncEnumerable<TSource>>> OnNextBatchAsync()
        {
            switch (this.state)
            {
                case 0: // Initial, materialize the source, and return the first groupings.
                    this.materialized = await this.source.Policy.MaterializeAsync(this.source).ConfigureAwait(false);
                    this.enumerator = this.materialized.GetAsyncEnumerator();

                    return this.EnumerateBatches();
                case 1: // Continue, return the next groupings.
                    if (!await this.enumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 2;
                        var count = this.offset % this.batchSize;
                        return count == 0 ? null : EnumerateItem(new Batch<TSource>(this.materialized, this.offset - count, count));
                    }

                    return this.EnumerateBatches();
                case 2: // Done, no more batches available.
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
        /// <param name="batch">
        /// The batch.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerator{T}"/>.
        /// </returns>
        private static IEnumerator<IAsyncEnumerable<TSource>> EnumerateItem(Batch<TSource> batch)
        {
            yield return batch;
        }

        /// <summary>
        /// Enumerates the groupings in a batch.
        /// </summary>
        /// <returns>
        /// The groupings.
        /// </returns>
        private IEnumerator<IAsyncEnumerable<TSource>> EnumerateBatches()
        {
            while (this.enumerator.MoveNext())
            {
                if (this.offset % this.batchSize == 0 && this.offset != 0)
                {
                    yield return new Batch<TSource>(this.materialized, this.offset - this.batchSize, this.batchSize);
                }

                this.offset++;
            }

            if (this.enumerator.IsSynchronous)
            {
                var count = this.offset % this.batchSize;

                if (count != 0)
                {
                    yield return new Batch<TSource>(this.materialized, this.offset - count, count);
                }

                // We're done.
                this.state = 2;
            }
            else
            {
                // Next batch.
                this.state = 1;
            }
        }
    }
}