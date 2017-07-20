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
    internal class UnionEnumerator<TSource> : AsyncEnumeratorBase<TSource>
    {
        /// <summary>
        /// The comparer.
        /// </summary>
        private IComparer<TSource> comparer;

        /// <summary>
        /// The last item.
        /// </summary>
        private TSource lastItem;

        /// <summary>
        /// Indicates whether the last item is invalid.
        /// </summary>
        private bool lastItemInvalid = true;

        /// <summary>
        /// The left.
        /// </summary>
        private IAsyncEnumerable<TSource> left;

        /// <summary>
        /// The left enumerator.
        /// </summary>
        private IAsyncEnumerator<TSource> leftEnumerator;

        /// <summary>
        /// The right.
        /// </summary>
        private IAsyncEnumerable<TSource> right;

        /// <summary>
        /// The right enumerator.
        /// </summary>
        private IAsyncEnumerator<TSource> rightEnumerator;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnionEnumerator{TSource}"/> class.
        /// </summary>
        /// <param name="left">
        /// The left part of the union.
        /// </param>
        /// <param name="right">
        /// The right part of the union.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public UnionEnumerator(IAsyncEnumerable<TSource> left, IAsyncEnumerable<TSource> right, IComparer<TSource> comparer)
        {
            this.left = left;
            this.right = right;
            this.comparer = comparer;
        }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
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

            this.state = 6;

            this.leftEnumerator?.Dispose();
            this.rightEnumerator?.Dispose();

            this.left = null;
            this.right = null;
            this.leftEnumerator = null;
            this.rightEnumerator = null;
            this.comparer = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
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
                case 0: // Initial, init left and right.
                    Func<Task<IAsyncEnumerator<TSource>>> setLeft = async () =>
                        this.leftEnumerator = (await this.left.Policy.SortAsync(this.left, this.comparer.Compare).ConfigureAwait(false)).GetAsyncEnumerator();
                    Func<Task<IAsyncEnumerator<TSource>>> setRight = async () =>
                        this.rightEnumerator = (await this.right.Policy.SortAsync(this.right, this.comparer.Compare).ConfigureAwait(false)).GetAsyncEnumerator();

                    await Task.WhenAll(setLeft(), setRight()).ConfigureAwait(false);

                    if (!this.leftEnumerator.MoveNext())
                    {
                        if (!this.rightEnumerator.MoveNext())
                        {
                            this.state = 5;

                            return null;
                        }

                        goto case 1;
                    }

                    if (!this.rightEnumerator.MoveNext())
                    {
                        goto case 2;
                    }

                    return this.EnumerateItems();
                case 1: // Left needs new batch, right is ok.
                    if (!await this.leftEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 4;

                        return this.Enumerate(this.rightEnumerator);
                    }

                    if (this.leftEnumerator.MoveNext())
                    {
                        goto case 1;
                    }

                    return this.EnumerateItems();
                case 2: // Right needs new batch, left is ok.
                    if (!await this.rightEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 3;

                        return this.Enumerate(this.leftEnumerator);
                    }

                    if (!this.rightEnumerator.MoveNext())
                    {
                        goto case 2;
                    }

                    return this.EnumerateItems();
                case 3: // Only left side has items.
                    if (await this.leftEnumerator.NextBatchAsync().ConfigureAwait(false) && this.leftEnumerator.MoveNext())
                    {
                        return this.Enumerate(this.leftEnumerator);
                    }

                    this.state = 5;

                    return null;
                case 4: // Only right side has items.
                    if (await this.rightEnumerator.NextBatchAsync().ConfigureAwait(false) && this.rightEnumerator.MoveNext())
                    {
                        return this.Enumerate(this.rightEnumerator);
                    }

                    this.state = 5;

                    return null;
                case 5: // Done.
                    return null;
                case 6: // Disposed.
                    throw new ObjectDisposedException(this.GetType().ToString());
                default:
                    throw new InvalidOperationException($"Invalid state: {this.state}.");
            }
        }

        /// <summary>
        /// Enumerates the enumerator, leaving out duplicate records.
        /// </summary>
        /// <param name="enumerator">
        /// The enumerator.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerator{TSource}"/>.
        /// </returns>
        private IEnumerator<TSource> Enumerate(IAsyncEnumerator<TSource> enumerator)
        {
            do
            {
                if (this.lastItemInvalid && this.comparer.Compare(enumerator.Current, this.lastItem) == 0)
                {
                    continue;
                }

                yield return this.lastItem = enumerator.Current;

                this.lastItemInvalid = false;
            }
            while (enumerator.MoveNext());
        }

        /// <summary>
        /// Enumerates the items for the current batch.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator{TResult}"/>.
        /// </returns>
        private IEnumerator<TSource> EnumerateItems()
        {
            // ReSharper disable InvertIf
            while (true)
            {
                var compared = this.comparer.Compare(this.leftEnumerator.Current, this.rightEnumerator.Current);
                if (compared <= 0 && (this.lastItemInvalid || this.comparer.Compare(this.leftEnumerator.Current, this.lastItem) != 0))
                {
                    yield return this.lastItem = this.leftEnumerator.Current;

                    this.lastItemInvalid = false;

                    if (!this.leftEnumerator.MoveNext())
                    {
                        this.state = 1;

                        yield break;
                    }

                    if (compared == 0 && !this.rightEnumerator.MoveNext())
                    {
                        this.state = 2;

                        yield break;
                    }
                }
                else if (compared > 0 && (this.lastItemInvalid || this.comparer.Compare(this.rightEnumerator.Current, this.lastItem) != 0))
                {
                    yield return this.lastItem = this.rightEnumerator.Current;

                    this.lastItemInvalid = false;

                    if (!this.rightEnumerator.MoveNext())
                    {
                        this.state = 2;

                        yield break;
                    }
                }
            }

            // ReSharper restore InvertIf
        }
    }
}