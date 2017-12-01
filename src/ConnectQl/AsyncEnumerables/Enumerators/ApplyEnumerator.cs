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
    /// Applies all elements in the left collection to the function in the right collection and returns the results.
    /// </summary>
    /// <typeparam name="TLeft">
    /// The type of the elements in the left collection.
    /// </typeparam>
    /// <typeparam name="TRight">
    /// The type of the elements in the right collection.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the resulting elements.
    /// </typeparam>
    internal class ApplyEnumerator<TLeft, TRight, TResult> : AsyncEnumeratorBase<TResult>
    {
        /// <summary>
        /// True if this is an is OUTER APPY, false for a CROSS APPLY.
        /// </summary>
        private readonly bool isOuterApply;

        /// <summary>
        /// The number of items returned for the current item.
        /// </summary>
        private long itemsReturned;

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
        /// The right expr.
        /// </summary>
        private Func<TLeft, IAsyncEnumerable<TRight>> rightFactory;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// True if we're still enumerating, false otherwise.
        /// </summary>
        private bool stillEnumerating;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyEnumerator{TLeft,TRight,TResult}"/> class.
        /// </summary>
        /// <param name="isOuterApply">
        /// The is outer apply.
        /// </param>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="rightFactory">
        /// The right expr.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        public ApplyEnumerator(bool isOuterApply, [NotNull] IAsyncEnumerable<TLeft> left, Func<TLeft, IAsyncEnumerable<TRight>> rightFactory, Func<TLeft, TRight, TResult> resultSelector)
        {
            this.isOuterApply = isOuterApply;
            this.rightFactory = rightFactory;
            this.resultSelector = resultSelector;
            this.leftEnumerator = left.GetAsyncEnumerator();
        }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
        ///     Since the <see cref="rightFactory"/> function can return anything, we can never be sure this is a synchronous
        ///     enumerator.
        /// </summary>
        public override bool IsSynchronous => false;

        /// <summary>
        /// Disposes the <see cref="IAsyncEnumerator{T}"/>.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.leftEnumerator?.Dispose();
            this.leftEnumerator = null;
            this.rightFactory = null;
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
            return this.EnumerateItems();
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
                case 0: // Next batch for the left side.
                    if (!await this.leftEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 2;

                        return null;
                    }

                    return this.EnumerateItems();
                case 1: // Next batch for the right side.
                    if (!await this.rightEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.stillEnumerating = false;

                        if (this.isOuterApply && this.itemsReturned == 0)
                        {
                            return ApplyEnumerator<TLeft, TRight, TResult>.EnumerateItem(this.resultSelector(this.leftEnumerator.Current, default(TRight)));
                        }
                    }

                    return this.EnumerateItems();
                case 2: // Done.
                    return null;
                case 3: // Disposed.
                    throw new ObjectDisposedException(this.GetType().ToString());
                default:
                    throw new InvalidOperationException($"Invalid state: {this.state}.");
            }
        }

        /// <summary>
        /// The enumerate item.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerator{T}"/>.
        /// </returns>
        private static IEnumerator<TResult> EnumerateItem(TResult result)
        {
            yield return result;
        }

        /// <summary>
        /// Enumerates the items.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator{TResult}"/>.
        /// </returns>
        private IEnumerator<TResult> EnumerateItems()
        {
            while (this.stillEnumerating || this.leftEnumerator.MoveNext())
            {
                if (!this.stillEnumerating)
                {
                    this.rightEnumerator?.Dispose();
                    this.rightEnumerator = this.rightFactory(this.leftEnumerator.Current).GetAsyncEnumerator();
                    this.itemsReturned = 0L;
                }

                while (this.rightEnumerator.MoveNext())
                {
                    this.itemsReturned++;
                    yield return this.resultSelector(this.leftEnumerator.Current, this.rightEnumerator.Current);
                }

                if (!this.rightEnumerator.IsSynchronous)
                {
                    this.state = 1;
                    this.stillEnumerating = true;

                    yield break;
                }

                this.stillEnumerating = false;

                if (this.isOuterApply && this.itemsReturned == 0)
                {
                    yield return this.resultSelector(this.leftEnumerator.Current, default(TRight));
                }
            }

            this.state = this.leftEnumerator.IsSynchronous ? (byte)2 : (byte)0;
        }
    }
}