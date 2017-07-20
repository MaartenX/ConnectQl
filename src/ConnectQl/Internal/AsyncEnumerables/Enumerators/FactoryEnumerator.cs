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
    /// Enumerator that enumerates the items of an asynchronously created <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result elements.
    /// </typeparam>
    internal class FactoryEnumerator<T> : AsyncEnumeratorBase<T>
    {
        /// <summary>
        /// The enumerator.
        /// </summary>
        private IAsyncEnumerator<T> asyncEnumerator;

        /// <summary>
        /// The transform.
        /// </summary>
        private Func<Task<IAsyncEnumerable<T>>> factory;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryEnumerator{T}"/> class.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        public FactoryEnumerator(Func<Task<IAsyncEnumerable<T>>> factory)
        {
            this.factory = factory;
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

            this.state = 3;

            this.asyncEnumerator?.Dispose();
            this.asyncEnumerator = null;
            this.factory = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        protected override IEnumerator<T> InitialBatch()
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
        protected override async Task<IEnumerator<T>> OnNextBatchAsync()
        {
            switch (this.state)
            {
                case 0:
                    this.asyncEnumerator = (await this.factory().ConfigureAwait(false)).GetAsyncEnumerator();
                    this.state = 1;

                    return this.asyncEnumerator.CurrentBatchToEnumerator();
                case 1:
                    if (!await this.asyncEnumerator.NextBatchAsync().ConfigureAwait(false))
                    {
                        this.state = 2;
                        return null;
                    }

                    return this.asyncEnumerator.CurrentBatchToEnumerator();
                case 2:
                    return null;
                case 3:
                    throw new ObjectDisposedException(this.GetType().ToString());
                default:
                    throw new InvalidOperationException($"Invalid state: {this.state}.");
            }
        }
    }
}