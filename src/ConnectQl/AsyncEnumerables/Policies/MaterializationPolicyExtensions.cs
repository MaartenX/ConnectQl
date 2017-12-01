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

namespace ConnectQl.AsyncEnumerables.Policies
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.AsyncEnumerables.Enumerators;
    using ConnectQl.AsyncEnumerables.Visualizers;

    using JetBrains.Annotations;

    /// <summary>
    /// The materialization policy extensions.
    /// </summary>
    [PublicAPI]
    public static class MaterializationPolicyExtensions
    {
        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> from a generator.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="initialize">
        /// Initializes the enumerable, and returns the context.
        /// </param>
        /// <param name="generateItems">
        /// Returns a batch of items or <c>null</c> when no more batches are available.
        /// </param>
        /// <param name="dispose">
        /// Function to call when disposing the generator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <typeparam name="TState">
        /// The type of the enumerator state.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerable<T, TState>(this IMaterializationPolicy policy, Func<TState> initialize, Func<TState, Task<IEnumerable<T>>> generateItems, [CanBeNull] Action<TState> dispose = null)
            => policy.CreateAsyncEnumerable(() => Task.Run(initialize), generateItems, dispose);

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> from a generator.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="factory">
        /// The expr.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerable<T>(this IMaterializationPolicy policy, Func<Task<IAsyncEnumerable<T>>> factory)
            => policy.CreateAsyncEnumerable(() => new FactoryEnumerator<T>(factory));

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> from a generator.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="generateItems">
        /// Returns a batch of items or <c>null</c> when no more batches are available.
        /// </param>
        /// <param name="dispose">
        /// Function to call when disposing the generator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <typeparam name="TState">
        /// The type of the enumerator state.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerable<T, TState>(this IMaterializationPolicy policy, Func<TState, Task<IEnumerable<T>>> generateItems, [CanBeNull] Action<TState> dispose = null)
            where TState : new()
            => policy.CreateAsyncEnumerable(() => Task.FromResult(new TState()), generateItems, dispose);

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> from a generator.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="initialize">
        /// Initializes the enumerable, and returns the context.
        /// </param>
        /// <param name="generateItem">
        /// Returns an item or <c>null</c> when no more items are available.
        /// </param>
        /// <param name="dispose">
        /// Function to call when disposing the generator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <typeparam name="TState">
        /// The type of the enumerator state.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerable<T, TState>(this IMaterializationPolicy policy, Func<Task<TState>> initialize, Func<TState, Task<T>> generateItem, [CanBeNull] Action<TState> dispose = null)
            where T : class
            => policy.CreateAsyncEnumerable(initialize, context => MaterializationPolicyExtensions.ToEnumerableAsync(generateItem(context)), dispose);

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> from a generator.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="initialize">
        /// Initializes the enumerable, and returns the context.
        /// </param>
        /// <param name="generateItem">
        /// Returns an item or <c>null</c> when no more items are available.
        /// </param>
        /// <param name="dispose">
        /// Function to call when disposing the generator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <typeparam name="TState">
        /// The type of the enumerator state.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerable<T, TState>(this IMaterializationPolicy policy, Func<TState> initialize, Func<TState, Task<T>> generateItem, [CanBeNull] Action<TState> dispose = null)
            where T : class
            => policy.CreateAsyncEnumerable(() => Task.FromResult(initialize()), context => MaterializationPolicyExtensions.ToEnumerableAsync(generateItem(context)), dispose);

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> from a generator.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="generateItem">
        /// The generate Item.
        /// </param>
        /// <param name="dispose">
        /// Function to call when disposing the generator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <typeparam name="TState">
        /// The type of the enumerator state.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerable<T, TState>(this IMaterializationPolicy policy, Func<TState, Task<T>> generateItem, [CanBeNull] Action<TState> dispose = null)
            where TState : new()
            where T : class
            => policy.CreateAsyncEnumerable(() => new GeneratorEnumerable<T, TState>(() => Task.FromResult(new TState()), context => MaterializationPolicyExtensions.ToEnumerableAsync(generateItem(context)), dispose));

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> from a generator.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="initialize">
        /// Initializes the enumerable, and returns the context.
        /// </param>
        /// <param name="generateItems">
        /// Returns a batch of items or <c>null</c> when no more batches are available.
        /// </param>
        /// <param name="dispose">
        /// Function to call when disposing the generator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <typeparam name="TState">
        /// The type of the enumerator state.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerable<T, TState>(this IMaterializationPolicy policy, Func<Task<TState>> initialize, Func<TState, Task<IEnumerable<T>>> generateItems, [CanBeNull] Action<TState> dispose = null)
            => policy.CreateAsyncEnumerable(() => new GeneratorEnumerable<T, TState>(initialize, generateItems, dispose));

        /// <summary>
        /// Creates an asynchronous enumerable from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerable<T>(this IMaterializationPolicy policy, IEnumerable<T> enumerable)
        {
            return policy.CreateAsyncEnumerable(() => new EnumerableEnumerator<T>(enumerable));
        }

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> by calling <paramref name="itemGenerator"/> once.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="itemGenerator">
        /// The item Generator.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerableAndRunOnce<T>(this IMaterializationPolicy policy, Func<Task<IEnumerable<T>>> itemGenerator)
            => policy.CreateAsyncEnumerable<T, SingleRunContext<object>>(src => src.NumberOfRuns++ == 0 ? itemGenerator() : null);

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> by calling <paramref name="itemGenerator"/> once.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="itemGenerator">
        /// Returns the items.
        /// </param>
        /// <param name="dispose">
        /// Called when the enumerator is disposed.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerableAndRunOnce<T>(this IMaterializationPolicy policy, Func<Task<IEnumerable<T>>> itemGenerator, Action dispose)
            where T : class
            => policy.CreateAsyncEnumerable<T, SingleRunContext<object>>(src => src.NumberOfRuns++ == 0 ? itemGenerator() : null, src => dispose());

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> by calling <paramref name="itemGenerator"/> once.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="initialize">
        /// The initialize.
        /// </param>
        /// <param name="itemGenerator">
        /// Returns the items.
        /// </param>
        /// <param name="dispose">
        /// Called when the enumerator is disposed.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <typeparam name="TState">
        /// The type of the enumerator state.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerableAndRunOnce<T, TState>(this IMaterializationPolicy policy, Func<Task<TState>> initialize, Func<TState, Task<IEnumerable<T>>> itemGenerator, Action<TState> dispose)
            where T : class
            => policy.CreateAsyncEnumerable(
                async () => new SingleRunContext<TState>(await initialize()),
                async src => src.NumberOfRuns++ == 0 ? await itemGenerator(src.Context) : null,
                src => dispose(src.Context));

        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> by calling <paramref name="itemGenerator"/> once.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="initialize">
        /// The initialize.
        /// </param>
        /// <param name="itemGenerator">
        /// Returns the items.
        /// </param>
        /// <param name="dispose">
        /// Called when the enumerator is disposed.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <typeparam name="TState">
        /// The type of the enumerator state.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateAsyncEnumerableAndRunOnce<T, TState>(this IMaterializationPolicy policy, Func<Task<TState>> initialize, Func<TState, IEnumerable<T>> itemGenerator, Action<TState> dispose)
            where T : class
            => policy.CreateAsyncEnumerable(
                async () => new SingleRunContext<TState>(await initialize()),
                src => Task.Run(() => src.NumberOfRuns++ == 0 ? itemGenerator(src.Context) : null),
                src => dispose(src == null ? default(TState) : src.Context));

        /// <summary>
        /// Creates an empty async enumerable.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> CreateEmptyAsyncEnumerable<T>(this IMaterializationPolicy policy)
        {
            return policy.CreateAsyncEnumerable(() => new EmptyEnumerator<T>());
        }

        /// <summary>
        /// Retrieves all elements from the <see cref="IAsyncEnumerable{T}"/> from the source and stores them in a persistent
        ///     <see cref="IAsyncReadOnlyCollection{T}"/>.
        ///     This means that it can be enumerated multiple times without having to access the source over and over again.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <param name="policy">
        /// The materialization policy.
        /// </param>
        /// <param name="source">
        /// The <see cref="IAsyncEnumerable{T}"/> to materialize.
        /// </param>
        /// <returns>
        /// If <paramref name="source"/> was already a <see cref="IAsyncReadOnlyCollection{T}"/>, <paramref name="source"/>,
        ///     otherwise a new
        ///     <see cref="IAsyncReadOnlyCollection{T}"/> containing the elements in the sequence.
        /// </returns>
        public static async Task<IAsyncReadOnlyCollection<T>> MaterializeAsync<T>(this IMaterializationPolicy policy, [NotNull] IAsyncEnumerable<T> source)
        {
            if (source.Policy == policy && source is IAsyncReadOnlyCollection<T>)
            {
                return (IAsyncReadOnlyCollection<T>)source;
            }

            var builder = policy.CreateBuilder<T>();

            using (var enumerator = source.GetAsyncEnumerator())
            {
                if (builder is ISynchronousAsyncEnumerableBuilder<T> syncBuilder)
                {
                    do
                    {
                        syncBuilder.Add(enumerator.CurrentBatchToEnumerable());
                    }
                    while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
                }
                else
                {
                    do
                    {
                        await builder.AddAsync(enumerator.CurrentBatchToEnumerable());
                    }
                    while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
                }
            }

            return await builder.BuildAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Converts an enumerable to an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IMaterializationPolicy policy, IEnumerable<T> enumerable)
        {
            return policy.CreateAsyncEnumerableAndRunOnce(() => Task.FromResult(enumerable));
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="factory">
        /// The expr.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        [NotNull]
        internal static IAsyncEnumerable<T> CreateAsyncEnumerable<T>(this IMaterializationPolicy policy, [NotNull] Expression<Func<IAsyncEnumerator<T>>> factory)
        {
            return new FactoryEnumerable<T>(policy, factory.Compile(), factory.Body.Type.Name);
        }

        /// <summary>
        /// Converts an item to an enumerable.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <typeparam name="T">
        /// The type of the item.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        private static IEnumerable<T> ToEnumerable<T>(T item)
        {
            yield return item;
        }

        /// <summary>
        /// Converts a task to a task of enumerable.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <typeparam name="T">
        /// The type of the item.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/> or <c>null</c> if the item returns <c>null</c>.
        /// </returns>
        [ItemCanBeNull]
        private static async Task<IEnumerable<T>> ToEnumerableAsync<T>([NotNull] Task<T> item)
            where T : class
        {
            var result = await item.ConfigureAwait(false);

            return result == null ? null : MaterializationPolicyExtensions.ToEnumerable(result);
        }

        /// <summary>
        /// The expr enumerable.
        /// </summary>
        /// <typeparam name="TElement">
        /// The type of the elements.
        /// </typeparam>
        [DebuggerTypeProxy(typeof(AsyncEnumerableVisualizer<>))]
        private class FactoryEnumerable<TElement> : IAsyncEnumerable<TElement>
        {
            /// <summary>
            /// The expr.
            /// </summary>
            private readonly Func<IAsyncEnumerator<TElement>> factory;

            /// <summary>
            /// The name of the enumerable.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// Initializes a new instance of the <see cref="FactoryEnumerable{TElement}"/> class.
            /// </summary>
            /// <param name="policy">
            /// The policy.
            /// </param>
            /// <param name="factory">
            /// The expr.
            /// </param>
            /// <param name="name">
            /// The name of the enumerator Used for debugging.
            /// </param>
            public FactoryEnumerable(IMaterializationPolicy policy, Func<IAsyncEnumerator<TElement>> factory, string name)
            {
                this.Policy = policy;
                this.factory = factory;
                this.name = name;
            }

            /// <summary>
            /// Gets the materialization policy.
            /// </summary>
            public IMaterializationPolicy Policy { get; }

            /// <summary>
            /// Gets an enumerator that returns batches of elements.
            /// </summary>
            /// <returns>
            /// The enumerator.
            /// </returns>
            public IAsyncEnumerator<TElement> GetAsyncEnumerator() => this.factory();

            /// <summary>
            /// Converts the enuemrable to a string.
            /// </summary>
            /// <returns>
            /// The string representation of the enumerable.
            /// </returns>
            public override string ToString()
            {
                return Regex.Replace(this.name.Replace("Enumerator", "Enumerable"), "`[0-9]+$", string.Empty);
            }
        }

        /// <summary>
        /// Used to make sure a generator only runs once.
        /// </summary>
        /// <typeparam name="TState">
        /// The type of the inner context.
        /// </typeparam>
        private class SingleRunContext<TState>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SingleRunContext{TState}"/> class.
            /// </summary>
            public SingleRunContext()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SingleRunContext{TState}"/> class.
            /// </summary>
            /// <param name="context">
            /// The context.
            /// </param>
            public SingleRunContext(TState context)
            {
                this.Context = context;
            }

            /// <summary>
            /// Gets the context.
            /// </summary>
            public TState Context { get; }

            /// <summary>
            /// Gets or sets the number of runs.
            /// </summary>
            public int NumberOfRuns { get; set; }
        }
    }
}