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

namespace ConnectQl.AsyncEnumerablePolicies
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;

    /// <summary>
    ///     The in memory policy.
    /// </summary>
    public class InMemoryPolicy : IMaterializationPolicy
    {
        /// <summary>
        /// Gets the maximum chunk size.
        /// </summary>
        public long MaximumChunkSize { get; } = long.MaxValue;

        /// <summary>
        ///     Creates a builder that can be used to create an <see cref="IAsyncEnumerable{T}" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the <see cref="IAsyncEnumerable{T}" />.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IAsyncEnumerableBuilder{T}" />.
        /// </returns>
        public IAsyncEnumerableBuilder<T> CreateBuilder<T>()
        {
            return new InMemoryAsyncEnumerableBuilder<T>(this);
        }

        /// <summary>
        ///     Creates a new <see cref="IAsyncReadOnlyCollection{T}" /> that contains the sorted elements of the
        ///     <see cref="IAsyncEnumerable{T}" />.
        /// </summary>
        /// <param name="source">
        ///     The <see cref="IAsyncEnumerable{T}" /> to sort.
        /// </param>
        /// <param name="comparison">
        ///     The comparison to use while sorting.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the items.
        /// </typeparam>
        /// <returns>
        ///     The sorted <see cref="IAsyncReadOnlyCollection{T}" />.
        /// </returns>
        public async Task<IAsyncReadOnlyCollection<T>> SortAsync<T>(IAsyncEnumerable<T> source, Comparison<T> comparison)
        {
            T[] items;
            var materialized = source as InMemoryAsyncReadOnlyCollection<T>;

            if (materialized == null || materialized.Policy != this)
            {
                items = ((InMemoryAsyncReadOnlyCollection<T>)(await this.MaterializeAsync(source).ConfigureAwait(false))).Items;
            }
            else
            {
                items = (T[])materialized.Items.Clone();
            }

            Array.Sort(items, comparison);

            return new InMemoryAsyncReadOnlyCollection<T>(this, items);
        }

        /// <summary>
        ///     The in-memory asynchronous enumerable builder.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items.
        /// </typeparam>
        private class InMemoryAsyncEnumerableBuilder<T> : ISynchronousAsyncEnumerableBuilder<T>
        {
            /// <summary>
            ///     The minimum buffer size.
            /// </summary>
            private const int MinimumBufferSize = 1024;

            /// <summary>
            ///     The policy.
            /// </summary>
            private readonly IMaterializationPolicy policy;

            /// <summary>
            ///     The array.
            /// </summary>
            private T[] array = new T[InMemoryAsyncEnumerableBuilder<T>.MinimumBufferSize];

            /// <summary>
            ///     The number of items in the builder.
            /// </summary>
            private int numItems;

            /// <summary>
            ///     Initializes a new instance of the <see cref="InMemoryAsyncEnumerableBuilder{T}" /> class.
            /// </summary>
            /// <param name="policy">
            ///     The policy.
            /// </param>
            public InMemoryAsyncEnumerableBuilder(IMaterializationPolicy policy)
            {
                this.policy = policy;
            }

            /// <summary>
            /// Adds the specified items.
            /// </summary>
            /// <param name="items">The items.</param>
            /// <returns>
            /// This builder.
            /// </returns>
            ISynchronousAsyncEnumerableBuilder<T> ISynchronousAsyncEnumerableBuilder<T>.Add(IEnumerable<T> items)
            {
                if (this.numItems == -1)
                {
                    throw new InvalidOperationException("A builder can only be used once; collection was already built.");
                }

                foreach (var item in items)
                {
                    if (this.numItems == this.array.Length)
                    {
                        Array.Resize(ref this.array, this.array.Length * 2);
                    }

                    this.array[this.numItems++] = item;
                }

                return this;
            }

            /// <summary>
            ///     Adds items to the <see cref="IAsyncEnumerable{T}" />.
            /// </summary>
            /// <param name="items">
            ///     The items to add.
            /// </param>
            /// <returns>
            /// This builder.
            /// </returns>
            public Task<IAsyncEnumerableBuilder<T>> AddAsync(IEnumerable<T> items)
            {
                return Task.FromResult<IAsyncEnumerableBuilder<T>>(((ISynchronousAsyncEnumerableBuilder<T>)this).Add(items));
            }

            /// <summary>
            ///     Builds the <see cref="IAsyncEnumerable{T}" /> from the added items.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            public Task<IAsyncReadOnlyCollection<T>> BuildAsync()
            {
                if (this.numItems == -1)
                {
                    throw new InvalidOperationException("A builder can only be used once; collection was already built.");
                }

                if (this.array != null && this.array.Length != this.numItems)
                {
                    Array.Resize(ref this.array, this.numItems);
                }

                var result = Task.FromResult<IAsyncReadOnlyCollection<T>>(new InMemoryAsyncReadOnlyCollection<T>(this.policy, this.array ?? new T[0]));

                this.array = new T[InMemoryAsyncEnumerableBuilder<T>.MinimumBufferSize];
                this.numItems = -1;

                return result;
            }

            /// <summary>
            /// Disposes the builder.
            /// </summary>
            public void Dispose()
            {
                this.array = null;
            }
        }

        /// <summary>
        ///     An <see cref="IAsyncEnumerable{T}" /> that is stored in memory.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements.
        /// </typeparam>
        [StructLayout(LayoutKind.Sequential)]
        private class InMemoryAsyncReadOnlyCollection<T> : IAsyncReadOnlyCollection<T>
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="InMemoryAsyncReadOnlyCollection{T}" /> class.
            /// </summary>
            /// <param name="policy">
            ///     The policy.
            /// </param>
            /// <param name="items">
            ///     The items.
            /// </param>
            public InMemoryAsyncReadOnlyCollection(IMaterializationPolicy policy, T[] items)
            {
                this.Policy = policy;
                this.Items = items;
            }

            /// <summary>
            ///     Gets the number of elements in the enumerable.
            /// </summary>
            public long Count => this.Items.Length;

            /// <summary>
            ///     Gets the policy.
            /// </summary>
            public IMaterializationPolicy Policy { get; }

            /// <summary>
            ///     Gets the items for this enumerable.
            /// </summary>
            /// <returns>
            ///     The items.
            /// </returns>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public T[] Items { get; private set; }

            /// <summary>
            ///     Gets an enumerator that returns batches of elements.
            /// </summary>
            /// <returns>
            ///     The enumerator.
            /// </returns>
            public IAsyncEnumerator<T> GetAsyncEnumerator()
            {
                if (this.Items == null)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                return new Enumerator(this.Items);
            }

            /// <summary>
            ///     Gets the policy.
            /// </summary>
            /// <param name="offset">
            ///     The offset.
            /// </param>
            /// <returns>
            ///     The <see cref="IAsyncEnumerator{T}" />.
            /// </returns>
            public IAsyncEnumerator<T> GetAsyncEnumerator(long offset)
            {
                if (this.Items == null)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                return new Enumerator(this.Items, offset);
            }

            /// <summary>
            ///     Disposes the collection.
            /// </summary>
            void IDisposable.Dispose()
            {
                this.Items = null;
            }

            /// <summary>
            ///     The enumerator.
            /// </summary>
            private class Enumerator : IAsyncEnumerator<T>
            {
                /// <summary>
                ///     Stores the enumerator.
                /// </summary>
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private IEnumerator<T> enumerator;

                /// <summary>
                ///     The is enumerating.
                /// </summary>
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private bool isEnumerating;

                /// <summary>
                ///     Initializes a new instance of the <see cref="Enumerator" /> class.
                /// </summary>
                /// <param name="items">
                ///     The items.
                /// </param>
                /// <param name="offset">
                ///     The offset to start enumerating from.
                /// </param>
                public Enumerator(IEnumerable<T> items, long offset = 0)
                {
                    this.isEnumerating = true;
                    this.enumerator = (offset == 0 ? items : items.Skip((int)offset)).GetEnumerator();
                }

                /// <summary>
                ///     Gets the element in the collection at the current position of the enumerator.
                /// </summary>
                /// <returns>
                ///     The element in the collection at the current position of the enumerator.
                /// </returns>
                public T Current { get; private set; }

                /// <summary>
                ///     Gets a value indicating whether the enumerator is synchronous.
                ///     When <c>false</c>, <see cref="NextBatchAsync" /> must be called when <see cref="MoveNext" /> returns <c>false</c>.
                /// </summary>
                public bool IsSynchronous => true;

                /// <summary>
                ///     Disposes the enumerator.
                /// </summary>
                public void Dispose()
                {
                    this.enumerator?.Dispose();
                    this.enumerator = null;
                }

                /// <summary>
                ///     Advances the enumerator to the next element of the collection.
                /// </summary>
                /// <returns>
                ///     True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the
                ///     end of the collection.
                /// </returns>
                /// <exception cref="T:System.InvalidOperationException">
                ///     The collection was modified after the enumerator was created.
                /// </exception>
                public bool MoveNext()
                {
                    if (!this.isEnumerating)
                    {
                        return false;
                    }

                    if (this.enumerator.MoveNext())
                    {
                        this.Current = this.enumerator.Current;
                        return true;
                    }

                    this.enumerator.Dispose();
                    this.enumerator = null;
                    this.isEnumerating = false;

                    return false;
                }

                /// <summary>
                ///     Moves to the next batch.
                /// </summary>
                /// <returns>
                ///     <c>true</c> if another batch is available, <c>false</c> otherwise.
                /// </returns>
                public Task<bool> NextBatchAsync()
                {
                    if (this.isEnumerating)
                    {
                        throw new InvalidOperationException("Cannot move to the next batch until all items are enumerated.");
                    }

                    return Task.FromResult(this.enumerator != null);
                }
            }
        }
    }
}