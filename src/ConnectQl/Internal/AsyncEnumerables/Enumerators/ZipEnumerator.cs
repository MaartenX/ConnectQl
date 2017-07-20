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
    /// Enumerator used by the <see cref="AsyncEnumerableExtensions.Zip{TLeft,TRight,TResult}"/> method.
    /// </summary>
    /// <typeparam name="TLeft">
    /// The type of the left source elements.
    /// </typeparam>
    /// <typeparam name="TRight">
    /// The type of the right source elements.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result elements.
    /// </typeparam>
    internal class ZipEnumerator<TLeft, TRight, TResult> : AsyncEnumeratorBase<TResult>
    {
        /// <summary>
        /// True if we need to return everything, not only elements that can be joined with the rights side.
        /// </summary>
        private readonly bool isZipAll;

        /// <summary>
        /// The left enumerator.
        /// </summary>
        private IAsyncEnumerator<TLeft> leftEnumerator;

        /// <summary>
        /// The result selector.
        /// </summary>
        private Func<TLeft, TRight, TResult> resultSelector;

        /// <summary>
        /// The right enumerator.
        /// </summary>
        private IAsyncEnumerator<TRight> rightEnumerator;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipEnumerator{TLeft,TRight,TResult}"/> class.
        /// </summary>
        /// <param name="left">
        /// The left part of the union.
        /// </param>
        /// <param name="right">
        /// The right part of the union.
        /// </param>
        /// <param name="resultSelector">
        /// The result Selector.
        /// </param>
        /// <param name="isZipAll">
        /// The is Zip All.
        /// </param>
        public ZipEnumerator(IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<TLeft, TRight, TResult> resultSelector, bool isZipAll)
        {
            this.leftEnumerator = left.GetAsyncEnumerator();
            this.rightEnumerator = right.GetAsyncEnumerator();
            this.resultSelector = resultSelector;
            this.isZipAll = isZipAll;
        }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
        /// </summary>
        public override bool IsSynchronous => this.leftEnumerator.IsSynchronous && this.rightEnumerator.IsSynchronous;

        /// <summary>
        /// Disposes the <see cref="IAsyncEnumerator{T}"/>.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.state = 4;

            this.leftEnumerator?.Dispose();
            this.rightEnumerator?.Dispose();
            this.leftEnumerator = null;
            this.rightEnumerator = null;
            this.resultSelector = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        protected override IEnumerator<TResult> InitialBatch()
        {
            return this.EnumerateItems(false);
        }

        /// <summary>
        /// Gets called when the next batch is needed.
        /// </summary>
        /// <returns>
        /// A task returning an <see cref="IEnumerator{T}"/> containing the next batch, of <c>null</c> when all data is
        ///     enumerated.
        /// </returns>
        protected override async Task<IEnumerator<TResult>> OnNextBatchAsync()
        {
            switch (this.state)
            {
                case 0: // Left side needs new batch.
                    if (this.leftEnumerator.IsSynchronous || !await this.leftEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 3;
                        return null;
                    }

                    return this.EnumerateItems(false);
                case 1: // Right side needs a new batch.
                    if (this.rightEnumerator.IsSynchronous || !await this.rightEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        if (this.isZipAll)
                        {
                            this.state = 2;
                            return this.EnumerateLeft();
                        }

                        this.state = 3;
                        return null;
                    }

                    return this.EnumerateItems(true);
                case 2: // Right is done, enumerate left side.
                    do
                    {
                        if (!this.leftEnumerator.IsSynchronous && await this.leftEnumerator.NextBatchAsync().ConfigureAwait(false))
                        {
                            continue;
                        }

                        this.state = 3;
                        return null;
                    }
                    while (!this.leftEnumerator.MoveNext());

                    return this.EnumerateLeft();
                case 3: // Done.
                    return null;
                case 4: // Disposed.
                    throw new ObjectDisposedException(this.GetType().ToString());
                default:
                    throw new InvalidOperationException($"Invalid state: {this.state}.");
            }
        }

        /// <summary>
        /// The enumerate items.
        /// </summary>
        /// <param name="rightNeededNewBatch">
        /// The right needed new batch.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerator{TResult}"/>.
        /// </returns>
        private IEnumerator<TResult> EnumerateItems(bool rightNeededNewBatch)
        {
            if (rightNeededNewBatch)
            {
                if (!this.rightEnumerator.MoveNext())
                {
                    this.state = 1;
                    yield break;
                }

                yield return this.resultSelector(this.leftEnumerator.Current, this.rightEnumerator.Current);
            }

            while (true)
            {
                if (!this.leftEnumerator.MoveNext())
                {
                    this.state = 0;
                    yield break;
                }

                if (!this.rightEnumerator.MoveNext())
                {
                    this.state = 1;
                    yield break;
                }

                yield return this.resultSelector(this.leftEnumerator.Current, this.rightEnumerator.Current);
            }
        }

        /// <summary>
        /// Enumerates the items in the right <see cref="IAsyncEnumerator{T}"/> and combines them with the left item.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator{T}"/>.
        /// </returns>
        private IEnumerator<TResult> EnumerateLeft()
        {
            do
            {
                yield return this.resultSelector(this.leftEnumerator.Current, default(TRight));
            }
            while (this.leftEnumerator.MoveNext());
        }
    }
}