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
    /// Enumerator used by the <see cref="AsyncEnumerableExtensions.Skip{T}"/> method.
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
    internal class CrossJoinEnumerator<TLeft, TRight, TResult> : AsyncEnumeratorBase<TResult>
    {
        /// <summary>
        /// The left enumerator.
        /// </summary>
        private IAsyncEnumerator<TLeft> leftEnumerator;

        /// <summary>
        /// The materialized right collection.
        /// </summary>
        private IAsyncReadOnlyCollection<TRight> materializedRight;

        /// <summary>
        /// The result selector.
        /// </summary>
        private Func<TLeft, TRight, TResult> resultSelector;

        /// <summary>
        /// The right.
        /// </summary>
        private IAsyncEnumerable<TRight> right;

        /// <summary>
        /// The right enumerator.
        /// </summary>
        private IAsyncEnumerator<TRight> rightEnumerator;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// The still enumerating.
        /// </summary>
        private bool stillEnumerating;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossJoinEnumerator{TLeft,TRight,TResult}"/> class.
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
        public CrossJoinEnumerator([NotNull] IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<TLeft, TRight, TResult> resultSelector)
        {
            this.resultSelector = resultSelector;
            this.leftEnumerator = left.GetAsyncEnumerator();
            this.right = right;
            this.materializedRight = this.right as IAsyncReadOnlyCollection<TRight>;

            if (this.materializedRight != null)
            {
                this.rightEnumerator = this.materializedRight.GetAsyncEnumerator();
                this.IsSynchronous = this.leftEnumerator.IsSynchronous && this.rightEnumerator.IsSynchronous;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
        /// </summary>
        public override bool IsSynchronous { get; }

        /// <summary>
        /// Disposes the <see cref="IAsyncEnumerator{T}"/>.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.state = 3;

            this.leftEnumerator?.Dispose();
            this.rightEnumerator?.Dispose();

            this.resultSelector = null;
            this.leftEnumerator = null;
            this.rightEnumerator = null;
            this.materializedRight = null;
            this.right = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        [CanBeNull]
        protected override IEnumerator<TResult> InitialBatch()
        {
            return this.rightEnumerator == null ? null : this.EnumerateItems();
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
                case 0: // Right side was not materialized, materialize it and start enumeration.
                    this.materializedRight = await this.right.MaterializeAsync().ConfigureAwait(false);
                    this.rightEnumerator = this.materializedRight.GetAsyncEnumerator();

                    return this.EnumerateItems();
                case 1: // Next batch for the left side.
                    if (await this.leftEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        return this.EnumerateItems();
                    }

                    this.state = 3;
                    return null;
                case 2: // Next batch for the right side.
                    if (!await this.rightEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.rightEnumerator.Dispose();
                        this.rightEnumerator = this.materializedRight.GetAsyncEnumerator();
                        this.stillEnumerating = false;
                    }

                    return this.EnumerateItems();
                case 3: // Done.
                    return null;
                case 4: // Disposed.
                    throw new ObjectDisposedException(this.GetType().ToString());
                default:
                    throw new InvalidOperationException($"Invalid state: {this.state}.");
            }
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
                while (this.rightEnumerator.MoveNext())
                {
                    yield return this.resultSelector(this.leftEnumerator.Current, this.rightEnumerator.Current);
                }

                if (!this.rightEnumerator.IsSynchronous)
                {
                    this.state = 2;
                    this.stillEnumerating = true;

                    yield break;
                }

                this.rightEnumerator.Dispose();
                this.rightEnumerator = this.materializedRight.GetAsyncEnumerator();
                this.stillEnumerating = false;
            }

            this.state = this.leftEnumerator.IsSynchronous ? (byte)3 : (byte)1;
        }
    }
}