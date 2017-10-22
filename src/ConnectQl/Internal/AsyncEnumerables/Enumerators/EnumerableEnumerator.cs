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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Enumerates an <see cref="IEnumerable{T}"/> as an <see cref="ConnectQl.AsyncEnumerables.IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items.
    /// </typeparam>
    internal class EnumerableEnumerator<T> : AsyncEnumeratorBase<T>
    {
        /// <summary>
        /// The enumerator.
        /// </summary>
        private IEnumerator<T> enumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableEnumerator{T}"/> class.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        public EnumerableEnumerator([NotNull] IEnumerable<T> enumerable)
        {
            this.enumerator = enumerable.GetEnumerator();
        }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="ConnectQl.AsyncEnumerables.IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="ConnectQl.AsyncEnumerables.IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
        /// </summary>
        public override bool IsSynchronous => true;

        /// <summary>
        /// Disposes the <see cref="ConnectQl.AsyncEnumerables.IAsyncEnumerator{T}"/>.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.enumerator?.Dispose();
            this.enumerator = null;
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        protected override IEnumerator<T> InitialBatch()
        {
            return this.enumerator;
        }

        /// <summary>
        /// Gets called when the next batch is needed.
        /// </summary>
        /// <returns>
        /// A task returning an <see cref="IEnumerator{T}"/> containing the next batch, of <c>null</c> when all data is
        ///     enumerated.
        /// </returns>
        protected override Task<IEnumerator<T>> OnNextBatchAsync()
        {
            return Task.FromResult<IEnumerator<T>>(null);
        }
    }
}