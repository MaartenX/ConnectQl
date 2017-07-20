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

    /// <summary>
    /// Enumerator used by the <see cref="AsyncEnumerableExtensions.Skip{T}"/> method.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source elements.
    /// </typeparam>
    internal class CallbackEnumerator<TSource> : AsyncEnumeratorBase<TSource>
    {
        /// <summary>
        /// The after index.
        /// </summary>
        private readonly Action<TSource> afterIndex;

        /// <summary>
        /// The index.
        /// </summary>
        private readonly long index;

        /// <summary>
        /// The action to call after enumeration.
        /// </summary>
        private Action<long> after;

        /// <summary>
        /// The enumerator.
        /// </summary>
        private IAsyncEnumerator<TSource> asyncEnumerator;

        /// <summary>
        /// The action to call before enumerating.
        /// </summary>
        private Action before;

        /// <summary>
        /// The count.
        /// </summary>
        private long count;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackEnumerator{TSource}"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="before">
        /// Called before the enumeration starts.
        /// </param>
        /// <param name="after">
        /// Called when enumeration is complete with the number of items enumerated.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="afterIndex">
        /// The after Index.
        /// </param>
        public CallbackEnumerator(IAsyncEnumerable<TSource> source, Action before, Action<long> after, long index, Action<TSource> afterIndex)
        {
            this.asyncEnumerator = source.GetAsyncEnumerator();
            this.before = before;
            this.after = after;
            this.index = index;
            this.afterIndex = afterIndex;
        }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
        /// </summary>
        public override bool IsSynchronous => this.asyncEnumerator.IsSynchronous;

        /// <summary>
        /// Disposes the <see cref="IAsyncEnumerator{T}"/>.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.state = 2;

            this.asyncEnumerator?.Dispose();
            this.asyncEnumerator = null;
            this.before = null;
            this.after = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        protected override IEnumerator<TSource> InitialBatch()
        {
            this.before?.Invoke();

            return this.EnumerateItems();
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
                    if (!await this.asyncEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 1;
                        this.after?.Invoke(this.count);

                        return null;
                    }

                    return this.EnumerateItems();
                case 1:
                    return null;
                case 2:
                    throw new ObjectDisposedException(this.GetType().ToString());
                default:
                    throw new InvalidOperationException($"Invalid state: {this.state}.");
            }
        }

        /// <summary>
        /// Enumerates the items for the current batch.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator{TResult}"/>.
        /// </returns>
        private IEnumerator<TSource> EnumerateItems()
        {
            while (this.asyncEnumerator.MoveNext())
            {
                yield return this.asyncEnumerator.Current;

                if (this.count == this.index)
                {
                    this.afterIndex?.Invoke(this.asyncEnumerator.Current);
                }

                this.count++;
            }

            if (this.asyncEnumerator.IsSynchronous)
            {
                this.state = 1;
                this.after?.Invoke(this.count);
            }
        }
    }
}