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

namespace ConnectQl.AsyncEnumerables
{
    using System;
    using System.Diagnostics;

    using ConnectQl.AsyncEnumerables.Enumerators;
    using ConnectQl.AsyncEnumerables.Policies;
    using ConnectQl.AsyncEnumerables.Visualizers;

    using JetBrains.Annotations;

    /// <summary>
    /// The batch.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items.
    /// </typeparam>
    [DebuggerTypeProxy(typeof(MaterializedAsyncEnumerableVisualizer<>))]
    internal class Batch<T> : IAsyncReadOnlyCollection<T>
    {
        /// <summary>
        /// Stores the materialized enumerable this enumerable uses.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IAsyncReadOnlyCollection<T> materialized;

        /// <summary>
        /// Stores the start position of the batch.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly long start;

        /// <summary>
        /// Initializes a new instance of the <see cref="Batch{T}"/> class.
        /// </summary>
        /// <param name="materialized">
        /// The materialized.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        public Batch(IAsyncReadOnlyCollection<T> materialized, long start, long count)
        {
            this.materialized = materialized;
            this.start = start;
            this.Count = count;
        }

        /// <summary>
        /// Gets the number of elements in the enumerable.
        /// </summary>
        public long Count { get; }

        /// <summary>
        /// Gets the materialization policy.
        /// </summary>
        public IMaterializationPolicy Policy => this.materialized.Policy;

        /// <summary>
        /// Gets an enumerator that returns batches of elements.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        [NotNull]
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator()
        {
            return new TakeEnumerator<T>(this.materialized.GetAsyncEnumerator(this.start), this.Count);
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
        [NotNull]
        IAsyncEnumerator<T> IAsyncReadOnlyCollection<T>.GetAsyncEnumerator(long offset)
        {
            return offset >= this.Count
                       ? (IAsyncEnumerator<T>)new EmptyEnumerator<T>()
                       : new TakeEnumerator<T>(this.materialized.GetAsyncEnumerator(this.start + offset), this.Count - offset);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        void IDisposable.Dispose()
        {
        }
    }
}