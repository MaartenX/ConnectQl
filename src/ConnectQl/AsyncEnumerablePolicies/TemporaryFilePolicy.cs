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
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;

    using JetBrains.Annotations;

    /// <summary>
    ///     The temporary file policy.
    /// </summary>
    public class TemporaryFilePolicy : IMaterializationPolicy
    {
        /// <summary>
        ///     The memory policy.
        /// </summary>
        private readonly InMemoryPolicy memPolicy = new InMemoryPolicy();

        /// <summary>
        /// The serializer.
        /// </summary>
        private readonly ISerializer serializer;

        /// <summary>
        ///     Gets or sets the storage provider.
        /// </summary>
        private readonly IStorageProvider storageProvider;

        /// <summary>
        /// The registered transforms.
        /// </summary>
        private readonly Dictionary<Type, object> registeredTransforms = new Dictionary<Type, object>();

        /// <summary>
        /// The current id.
        /// </summary>
        private int currentId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFilePolicy"/> class.
        /// </summary>
        /// <param name="storageProvider">
        /// The storage provider.
        /// </param>
        /// <param name="serializer">
        /// The serializer.
        /// </param>
        protected TemporaryFilePolicy(IStorageProvider storageProvider, ISerializer serializer)
        {
            this.storageProvider = storageProvider;
            this.serializer = serializer;
        }

        /// <summary>
        ///     Gets the maximum chunk size.
        /// </summary>
        public long MaximumChunkSize { get; } = 1024;

        /// <summary>
        ///     Creates a builder that can be used to create an <see cref="IAsyncEnumerable{T}" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IAsyncEnumerableBuilder{T}" />.
        /// </returns>
        [NotNull]
        public IAsyncEnumerableBuilder<T> CreateBuilder<T>()
        {
            return new Builder<T>(this);
        }

        /// <summary>
        /// Registers a transform that turns items into serializable objects.
        /// </summary>
        /// <param name="transform">
        /// The transform.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="TemporaryFilePolicy"/>.
        /// </returns>
        [NotNull]
        public TemporaryFilePolicy RegisterTransform<T>(ITransform<T> transform)
        {
            if (!this.registeredTransforms.ContainsKey(typeof(T)))
            {
                this.registeredTransforms.Add(typeof(T), transform);
            }

            return this;
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
            var chunks = new List<IAsyncReadOnlyCollection<T>>();

            if (source.Policy != this)
            {
                source = await this.MaterializeAsync(source).ConfigureAwait(false);
            }

            await source.Batch(this.MaximumChunkSize).ForEachAsync(
                async batch =>
                    {
                        var builder = await this.CreateBuilder<T>().AddAsync(await this.memPolicy.SortAsync(batch, comparison).ConfigureAwait(false));

                        chunks.Add(await builder.BuildAsync().ConfigureAwait(false));
                    });

            var result = this.CreateBuilder<T>();

            var enumerators = chunks.Select(c => c.GetAsyncEnumerator()).ToList();

            foreach (var enumerator in enumerators.ToArray())
            {
                if (!enumerator.MoveNext() && !(await enumerator.NextBatchAsync().ConfigureAwait(false) && enumerator.MoveNext()))
                {
                    enumerator.Dispose();
                    enumerators.Remove(enumerator);
                }
            }

            enumerators.Sort((a, b) => comparison(a.Current, b.Current));

            while (true)
            {
                await result.AddAsync(TemporaryFilePolicy.TakeItems(enumerators, comparison)).ConfigureAwait(false);

                if (!(await enumerators[0].NextBatchAsync().ConfigureAwait(false) && enumerators[0].MoveNext()))
                {
                    enumerators[0].Dispose();
                    enumerators.RemoveAt(0);

                    if (enumerators.Count == 0)
                    {
                        break;
                    }
                }
                else
                {
                    TemporaryFilePolicy.SortEnumerators(enumerators, comparison);
                }
            }

            foreach (var chunk in chunks)
            {
                chunk.Dispose();
            }

            return await result.BuildAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Takes sorted items from the enumerators while it can be done synchronously.
        /// </summary>
        /// <param name="enumerators">
        /// The enumerators.
        /// </param>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The items that were taken synchronously.
        /// </returns>
        private static IEnumerable<T> TakeItems<T>([NotNull] List<IAsyncEnumerator<T>> enumerators, Comparison<T> comparison)
        {
            while (true)
            {
                yield return enumerators[0].Current;

                if (!enumerators[0].MoveNext())
                {
                    yield break;
                }

                TemporaryFilePolicy.SortEnumerators(enumerators, comparison);
            }
        }

        /// <summary>
        /// Sorts the list of enumerators. The list should be already sorted, except for the first element.
        /// </summary>
        /// <param name="enumerators">
        /// The enumerators.
        /// </param>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        private static void SortEnumerators<T>([NotNull] List<IAsyncEnumerator<T>> enumerators, Comparison<T> comparison)
        {
            for (var i = 1; i < enumerators.Count; i++)
            {
                if (comparison(enumerators[i - 1].Current, enumerators[i].Current) <= 0)
                {
                    break;
                }

                var tmp = enumerators[i - 1];
                enumerators[i - 1] = enumerators[i];
                enumerators[i] = tmp;
            }
        }

        /// <summary>
        /// The <see cref="IAsyncReadOnlyCollection{T}"/> implementation.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        private class Collection<T> : IAsyncReadOnlyCollection<T>
        {
            /// <summary>
            ///     The <see cref="ReadAndTransformItemsAsync{TItem}" /> method.
            /// </summary>
            private static readonly MethodInfo ReadAndTransformItemsAsyncMethod = typeof(Collection<T>).GetRuntimeMethods().FirstOrDefault(m => m.Name == nameof(Collection<T>.ReadAndTransformItemsAsync));

            /// <summary>
            /// The transform context.
            /// </summary>
            private readonly IDisposable context;

            /// <summary>
            /// The file id.
            /// </summary>
            private readonly int fileId;

            /// <summary>
            /// The policy that owns this collection.
            /// </summary>
            private readonly TemporaryFilePolicy policy;

            /// <summary>
            /// The transform to use.
            /// </summary>
            private readonly ITransform<T> transform;

            /// <summary>
            /// Generates the items.
            /// </summary>
            private Func<Stream, Task<IEnumerable<T>>> generateItems;

            /// <summary>
            /// Initializes a new instance of the <see cref="Collection{T}"/> class.
            /// </summary>
            /// <param name="policy">
            /// The policy.
            /// </param>
            /// <param name="fileId">
            /// The file id.
            /// </param>
            /// <param name="count">
            /// The count.
            /// </param>
            /// <param name="transform">
            /// The transform.
            /// </param>
            /// <param name="context">
            /// The context.
            /// </param>
            public Collection(TemporaryFilePolicy policy, int fileId, long count, ITransform<T> transform, IDisposable context)
            {
                this.policy = policy;
                this.fileId = fileId;
                this.Count = count;
                this.transform = transform;
                this.context = context;
            }

            /// <summary>
            ///     Gets the number of elements in the enumerable.
            /// </summary>
            public long Count { get; }

            /// <summary>
            ///     Gets the materialization policy.
            /// </summary>
            IMaterializationPolicy IAsyncEnumerable.Policy => this.policy;

            /// <summary>
            ///     Disposes the collection and deletes the file.
            /// </summary>
            async void IDisposable.Dispose()
            {
                this.context?.Dispose();
                await this.policy.storageProvider.DeleteFileAsync(this.fileId);
            }

            /// <summary>
            ///     Gets an enumerator that returns batches of elements.
            /// </summary>
            /// <returns>
            ///     The enumerator.
            /// </returns>
            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator()
            {
                return this.GetAsyncEnumerable().GetAsyncEnumerator();
            }

            /// <summary>
            ///     Gets an enumerator that returns batches of elements and starts at the offset.
            /// </summary>
            /// <param name="offset">
            ///     The offset.
            /// </param>
            /// <returns>
            ///     The enumerator.
            /// </returns>
            IAsyncEnumerator<T> IAsyncReadOnlyCollection<T>.GetAsyncEnumerator(long offset)
            {
                return this.GetAsyncEnumerable().Skip(offset).GetAsyncEnumerator();
            }

            /// <summary>
            ///     Gets the enumerable that enumerates all items in the collection..
            /// </summary>
            /// <returns>
            ///     The <see cref="IAsyncEnumerable{T}" />.
            /// </returns>
            private IAsyncEnumerable<T> GetAsyncEnumerable()
            {
                if (this.generateItems == null)
                {
                    this.CreateGenerator();
                }

                return this.policy.CreateAsyncEnumerable(
                        async () => await this.policy.storageProvider.GetFileAsync(this.fileId, FileAccessType.Read),
                        this.generateItems,
                        stream => stream.Dispose())
                    .Take(this.Count);
            }

            /// <summary>
            /// Creates the generator.
            /// </summary>
            private void CreateGenerator()
            {
                if (this.transform != null)
                {
                    var arg = Expression.Parameter(typeof(Stream));

                    this.generateItems = Expression.Lambda<Func<Stream, Task<IEnumerable<T>>>>(
                        Expression.Call(
                            Expression.Constant(this),
                            Collection<T>.ReadAndTransformItemsAsyncMethod.MakeGenericMethod(this.transform.TargetType),
                            arg),
                        arg).Compile();
                }
                else
                {
                    this.generateItems = stream => this.policy.serializer.ReadAsync<T>(stream, this.policy.MaximumChunkSize);
                }
            }

            /// <summary>
            ///     Reads and transform items.
            /// </summary>
            /// <param name="stream">
            ///     The stream.
            /// </param>
            /// <typeparam name="TItem">
            ///     The type of the items to read.
            /// </typeparam>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [ItemNotNull]
            private async Task<IEnumerable<T>> ReadAndTransformItemsAsync<TItem>(Stream stream)
            {
                return (await this.policy.serializer.ReadAsync<TItem>(stream, this.policy.MaximumChunkSize)).Select(item => this.transform.Deserialize(this.context, item));
            }
        }

        /// <summary>
        ///     The <see cref="IAsyncEnumerableBuilder{T}"/> implementation.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items.
        /// </typeparam>
        private class Builder<T> : IAsyncEnumerableBuilder<T>
        {
            /// <summary>
            ///     The <see cref="WriteItemsAsync{TItem}" /> method.
            /// </summary>
            private static readonly MethodInfo WriteItemsAsyncMethod = typeof(Builder<T>).GetRuntimeMethods().FirstOrDefault(m => m.Name == nameof(Builder<T>.WriteItemsAsync));

            /// <summary>
            ///     The policy.
            /// </summary>
            private TemporaryFilePolicy policy;

            /// <summary>
            ///     The context.
            /// </summary>
            private IDisposable context;

            /// <summary>
            /// Lock for adding items.
            /// </summary>
            private object addLock = new object();

            /// <summary>
            ///     The count.
            /// </summary>
            private long count;

            /// <summary>
            ///     The file.
            /// </summary>
            private Stream file;

            /// <summary>
            ///     The file id.
            /// </summary>
            private int fileId;

            /// <summary>
            ///     The transform.
            /// </summary>
            private ITransform<T> transform;

            /// <summary>
            /// Writes the items to the file.
            /// </summary>
            private Func<IEnumerable<T>, Task<long>> writeItemsAsync;

            /// <summary>
            /// The task that adds items to the builder.
            /// </summary>
            private Task addTask;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Builder{T}" /> class.
            /// </summary>
            /// <param name="policy">
            ///     The policy.
            /// </param>
            public Builder(TemporaryFilePolicy policy)
            {
                this.policy = policy;
            }

            /// <summary>
            /// Adds items to the <see cref="IAsyncEnumerable{T}"/>.
            /// </summary>
            /// <param name="items">
            /// The items to add.
            /// </param>
            /// <returns>
            /// This builder.
            /// </returns>
            [NotNull]
            public IAsyncEnumerableBuilder<T> Add(IEnumerable<T> items)
            {
                if (this.policy == null)
                {
                    throw new InvalidOperationException("A builder can only be used once; collection was already built.");
                }

                lock (this.addLock)
                {
                    var currentAddTask = this.addTask;

                    var add = currentAddTask == null
                        ? (Func<Task>)(() => this.AddAsync(items))
                        : async () =>
                          {
                              await currentAddTask.ConfigureAwait(false);

                              await this.AddAsync(items).ConfigureAwait(false);
                          };

                    this.addTask = add();
                }

                return this;
            }

            /// <summary>
            ///     Builds the <see cref="IAsyncEnumerable{T}" /> from the added items.
            /// </summary>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [ItemNotNull]
            public async Task<IAsyncReadOnlyCollection<T>> BuildAsync()
            {
                if (this.policy == null)
                {
                    throw new InvalidOperationException("A builder can only be used once; collection was already built.");
                }

                if (this.addTask != null)
                {
                    await this.addTask;
                }

                await this.file.FlushAsync();

                this.file.Dispose();
                this.file = null;

                var result = new Collection<T>(this.policy, this.fileId, this.count, this.transform, this.context);

                this.policy = null;

                return result;
            }

            /// <summary>
            ///     Adds items to the <see cref="IAsyncEnumerable{T}" />.
            /// </summary>
            /// <param name="items">
            ///     The items to add.
            /// </param>
            /// <returns>
            ///     The <see cref="Task" />.
            /// </returns>
            [ItemNotNull]
            public async Task<IAsyncEnumerableBuilder<T>> AddAsync(IEnumerable<T> items)
            {
                if (this.file == null)
                {
                    await this.CreateFileAsync();
                }

                this.count += await this.writeItemsAsync(items);

                return this;
            }

            /// <summary>
            /// Creates the file.
            /// </summary>
            /// <returns>
            /// The <see cref="Task"/>.
            /// </returns>
            private async Task CreateFileAsync()
            {
                this.file = await this.policy.storageProvider.GetFileAsync(this.fileId = this.policy.currentId++, FileAccessType.Write).ConfigureAwait(false);

                this.policy.registeredTransforms.TryGetValue(typeof(T), out var transformObject);
                this.transform = transformObject as ITransform<T>;
                this.context = this.transform?.CreateContext();

                if (this.transform != null)
                {
                    var arg = Expression.Parameter(typeof(IEnumerable<T>));

                    this.writeItemsAsync = Expression.Lambda<Func<IEnumerable<T>, Task<long>>>(Expression.Call(Expression.Constant(this), Builder<T>.WriteItemsAsyncMethod.MakeGenericMethod(this.transform.TargetType), arg), arg).Compile();
                }
                else
                {
                    this.writeItemsAsync = itemsToWrite => this.policy.serializer.WriteAsync(this.file, itemsToWrite);
                }
            }

            /// <summary>
            /// Writes the items to the serializer.
            /// </summary>
            /// <typeparam name="TItem">
            /// The type of the items.
            /// </typeparam>
            /// <param name="items">
            /// The items to write.
            /// </param>
            /// <returns>
            /// The number of items that were written.
            /// </returns>
            private async Task<long> WriteItemsAsync<TItem>([NotNull] IEnumerable<T> items)
            {
                return await this.policy.serializer.WriteAsync(this.file, items.Select(i => this.transform.Serialize(this.context, i)).Cast<TItem>()).ConfigureAwait(false);
            }
        }
    }
}