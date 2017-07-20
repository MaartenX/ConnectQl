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
    using System.Diagnostics;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;

    /// <summary>
    /// The async enumerator base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items.
    /// </typeparam>
    internal abstract class AsyncEnumeratorBase<T> : IAsyncEnumerator<T>
    {
        /// <summary>
        /// The current enumerator.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerator<T> currentEnumerator;

        /// <summary>
        /// Stores the current item.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private T currentItem;

        /// <summary>
        /// Stores a value indicating whether the current item is valid.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool currentItemValid;

        /// <summary>
        /// True if the enumerator was initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Stores a value indicating whether we are enumerating.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isEnumerating = true;

        /// <summary>
        /// Finalizes an instance of the <see cref="AsyncEnumeratorBase{T}"/> class.
        /// </summary>
        ~AsyncEnumeratorBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public T Current { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
        /// </summary>
        public abstract bool IsSynchronous { get; }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the
        ///     end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        [DebuggerHidden]
        public bool MoveNext()
        {
            if (!this.initialized)
            {
                this.initialized = true;
                this.GetInitialBatch();
            }

            if (!this.currentItemValid)
            {
                this.Current = default(T);
                this.isEnumerating = false;

                return false;
            }

            this.Current = this.currentItem;

            if (this.currentEnumerator != null && this.currentEnumerator.MoveNext())
            {
                this.currentItem = this.currentEnumerator.Current;
            }
            else
            {
                this.currentItemValid = false;
                this.currentEnumerator?.Dispose();
                this.currentEnumerator = null;
            }

            return true;
        }

        /// <summary>
        /// Advances the enumerator to the next batch of elements of the collection. Filters out empty batches.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of
        ///     the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        [DebuggerHidden]
        public async Task<bool> NextBatchAsync()
        {
            if (this.isEnumerating)
            {
                throw new InvalidOperationException("Cannot move to the next batch until all items are enumerated.");
            }

            this.currentEnumerator = await this.OnNextBatchAsync().ConfigureAwait(false);

            while (this.currentEnumerator != null && !this.currentEnumerator.MoveNext())
            {
                this.currentEnumerator.Dispose();
                this.currentEnumerator = await this.OnNextBatchAsync().ConfigureAwait(false);
            }

            if (this.currentEnumerator == null)
            {
                return false;
            }

            this.isEnumerating = true;
            this.currentItem = this.currentEnumerator.Current;
            this.currentItemValid = true;

            return true;
        }

        /// <summary>
        /// Disposes the <see cref="IAsyncEnumerator{T}"/>.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            this.currentEnumerator?.Dispose();
            this.currentEnumerator = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        protected abstract IEnumerator<T> InitialBatch();

        /// <summary>
        /// Gets called when the next batch is needed.
        /// </summary>
        /// <returns>
        /// A task returning an <see cref="IEnumerator{T}"/> containing the next batch, of <c>null</c> when all data is
        ///     enumerated.
        /// </returns>
        protected abstract Task<IEnumerator<T>> OnNextBatchAsync();

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        private void GetInitialBatch()
        {
            this.currentEnumerator = this.InitialBatch();

            if (this.currentEnumerator != null && this.currentEnumerator.MoveNext())
            {
                this.currentItem = this.currentEnumerator.Current;
                this.currentItemValid = true;
                this.isEnumerating = true;
            }
            else
            {
                this.currentItemValid = false;
                this.currentEnumerator?.Dispose();
                this.currentEnumerator = null;
            }
        }
    }
}