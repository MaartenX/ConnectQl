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
    /// Enumerator that creates items using a generator function.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source elements.
    /// </typeparam>
    /// <typeparam name="TState">
    /// The type of the state.
    /// </typeparam>
    internal class GeneratorEnumerable<TSource, TState> : AsyncEnumeratorBase<TSource>
    {
        /// <summary>
        /// The dispose.
        /// </summary>
        private Action<TState> dispose;

        /// <summary>
        /// Stores the context.
        /// </summary>
        private TState enumeratorState;

        /// <summary>
        /// The get items.
        /// </summary>
        private Func<TState, Task<IEnumerable<TSource>>> generateItems;

        /// <summary>
        /// The initialize.
        /// </summary>
        private Func<Task<TState>> initialize;

        /// <summary>
        /// The state.
        /// </summary>
        private byte state;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorEnumerable{TSource,TState}"/> class.
        /// </summary>
        /// <param name="initialize">
        /// The initialize.
        /// </param>
        /// <param name="generateItems">
        /// The get Items.
        /// </param>
        /// <param name="dispose">
        /// The dispose.
        /// </param>
        public GeneratorEnumerable(Func<Task<TState>> initialize, Func<TState, Task<IEnumerable<TSource>>> generateItems, Action<TState> dispose)
        {
            this.initialize = initialize;
            this.generateItems = generateItems;
            this.dispose = dispose;
        }

        /// <summary>
        /// Gets a value indicating whether the enumerator is synchronous.
        ///     When <c>false</c>, <see cref="ConnectQl.AsyncEnumerables.IAsyncEnumerator{T}.NextBatchAsync"/> must be called when
        ///     <see cref="ConnectQl.AsyncEnumerables.IAsyncEnumerator{T}.MoveNext"/> returns <c>false</c>.
        /// </summary>
        public override bool IsSynchronous => false;

        /// <summary>
        /// Resets all fields to null, and sets the state to 'disposed'.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.state = 3;

            this.dispose?.Invoke(this.enumeratorState);

            this.initialize = null;
            this.generateItems = null;
            this.dispose = null;
            this.enumeratorState = default(TState);
        }

        /// <summary>
        /// Gets the initial batch.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        [CanBeNull]
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
        [ItemCanBeNull]
        protected override async Task<IEnumerator<TSource>> OnNextBatchAsync()
        {
            switch (this.state)
            {
                case 0:
                    if (this.initialize != null)
                    {
                        this.enumeratorState = await this.initialize().ConfigureAwait(false);
                    }

                    this.state = 1;

                    goto case 1;
                case 1:

                    var batch = await this.generateItems(this.enumeratorState).ConfigureAwait(false);

                    if (batch == null)
                    {
                        this.state = 2;
                    }

                    return batch?.GetEnumerator();
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