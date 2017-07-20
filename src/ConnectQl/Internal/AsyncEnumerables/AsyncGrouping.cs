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

namespace ConnectQl.Internal.AsyncEnumerables
{
    using System;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Internal.AsyncEnumerables.Enumerators;

    /// <summary>
    /// The async grouping.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the elements.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key.
    /// </typeparam>
    internal class AsyncGrouping<TSource, TKey> : Batch<TSource>, IAsyncGrouping<TSource, TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncGrouping{TSource,TKey}"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="items">
        /// The inner.
        /// </param>
        /// <param name="start">
        /// The offset.
        /// </param>
        /// <param name="count">
        /// The number of inner.
        /// </param>
        public AsyncGrouping(TKey key, IAsyncReadOnlyCollection<SourceKey<TSource, TKey>> items, long start, long count)
            : base(new SourceKeyAdapter(items), start, count)
        {
            this.Key = key;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Disposes the grouping.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        /// <summary>
        /// The source key adapter.
        /// </summary>
        private class SourceKeyAdapter : IAsyncReadOnlyCollection<TSource>
        {
            /// <summary>
            /// The inner collection.
            /// </summary>
            private readonly IAsyncReadOnlyCollection<SourceKey<TSource, TKey>> inner;

            /// <summary>
            /// Initializes a new instance of the <see cref="SourceKeyAdapter"/> class.
            /// </summary>
            /// <param name="inner">
            /// The inner collection.
            /// </param>
            public SourceKeyAdapter(IAsyncReadOnlyCollection<SourceKey<TSource, TKey>> inner)
            {
                this.inner = inner;
            }

            /// <summary>
            /// Gets the number of elements in the enumerable.
            /// </summary>
            public long Count => this.inner.Count;

            /// <summary>
            /// Gets the materialization policy.
            /// </summary>
            public IMaterializationPolicy Policy => this.inner.Policy;

            /// <summary>
            /// Gets an enumerator that returns batches of elements.
            /// </summary>
            /// <returns>
            /// The enumerator.
            /// </returns>
            public IAsyncEnumerator<TSource> GetAsyncEnumerator()
            {
                return new EnumeratorWrapper(this.inner.GetAsyncEnumerator());
            }

            /// <summary>
            /// Gets an enumerator that returns batches of elements and starts at the offset.
            /// </summary>
            /// <param name="offset">
            /// The offset.
            /// </param>
            /// <returns>
            /// The enumerator.
            /// </returns>
            public IAsyncEnumerator<TSource> GetAsyncEnumerator(long offset)
            {
                return new EnumeratorWrapper(this.inner.GetAsyncEnumerator(offset));
            }

            /// <summary>
            /// Disposes the collection.
            /// </summary>
            void IDisposable.Dispose()
            {
                this.inner?.Dispose();
            }

            /// <summary>
            /// The enumerator wrapper.
            /// </summary>
            private class EnumeratorWrapper : IAsyncEnumerator<TSource>
            {
                /// <summary>
                /// The asynchronous enumerator.
                /// </summary>
                private readonly IAsyncEnumerator<SourceKey<TSource, TKey>> asyncEnumerator;

                /// <summary>
                /// Initializes a new instance of the <see cref="EnumeratorWrapper"/> class.
                /// </summary>
                /// <param name="asyncEnumerator">
                /// The async enumerator.
                /// </param>
                public EnumeratorWrapper(IAsyncEnumerator<SourceKey<TSource, TKey>> asyncEnumerator)
                {
                    this.asyncEnumerator = asyncEnumerator;
                }

                /// <summary>
                /// Gets the element in the collection at the current position of the enumerator.
                /// </summary>
                /// <returns>
                /// The element in the collection at the current position of the enumerator.
                /// </returns>
                public TSource Current => this.asyncEnumerator.Current.Source;

                /// <summary>
                /// Gets a value indicating whether the enumerator is synchronous.
                ///     When <c>false</c>, <see cref="NextBatchAsync"/> must be called when <see cref="MoveNext"/> returns <c>false</c>.
                /// </summary>
                public bool IsSynchronous => this.asyncEnumerator.IsSynchronous;

                /// <summary>
                /// Disposes the enumerator.
                /// </summary>
                public void Dispose()
                {
                    this.asyncEnumerator.Dispose();
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
                public bool MoveNext() => this.asyncEnumerator.MoveNext();

                /// <summary>
                /// Advances the enumerator to the next batch of elements of the collection.
                /// </summary>
                /// <returns>
                /// True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the
                ///     end of the collection.
                /// </returns>
                /// <exception cref="T:System.InvalidOperationException">
                /// The collection was modified after the enumerator was created.
                /// </exception>
                public Task<bool> NextBatchAsync() => this.asyncEnumerator.NextBatchAsync();
            }
        }
    }
}