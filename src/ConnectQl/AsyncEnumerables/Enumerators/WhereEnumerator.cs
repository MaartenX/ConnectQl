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
    /// Enumerator used by the <see cref="AsyncEnumerableExtensions.Skip{T}"/> method.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source elements.
    /// </typeparam>
    internal class WhereEnumerator<TSource> : AsyncEnumeratorBase<TSource>
    {
        /// <summary>
        /// The enumerator.
        /// </summary>
        private IAsyncEnumerator<TSource> asyncEnumerator;

        /// <summary>
        /// The transform.
        /// </summary>
        private Func<TSource, bool> filter;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhereEnumerator{TSource}"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public WhereEnumerator([NotNull] IAsyncEnumerable<TSource> source, Func<TSource, bool> filter)
        {
            this.asyncEnumerator = source.GetAsyncEnumerator();
            this.filter = filter;
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
            this.filter = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        protected override IEnumerator<TSource> InitialBatch()
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
        protected override async Task<IEnumerator<TSource>> OnNextBatchAsync()
        {
            switch (this.state)
            {
                case 0:
                    if (!await this.asyncEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 1;
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
                if (this.filter(this.asyncEnumerator.Current))
                {
                    yield return this.asyncEnumerator.Current;
                }
            }

            if (this.asyncEnumerator.IsSynchronous)
            {
                this.state = 1;
            }
        }
    }
}