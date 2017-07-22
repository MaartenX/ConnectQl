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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.AsyncEnumerables;
    using ConnectQl.Internal.AsyncEnumerables.Enumerators;
    using ConnectQl.Internal.Comparers;
    using ConnectQl.Internal.Extensions;

    /// <summary>
    /// The async enumerable extensions.
    /// </summary>
    public static class AsyncEnumerableExtensions
    {
        /// <summary>
        /// The <see cref="ConvertInternal{TSource,TTarget}"/> method.
        /// </summary>s
        private static readonly MethodInfo ConvertInternalMethod = typeof(AsyncEnumerableExtensions).GetGenericMethod(nameof(ConvertInternal), typeof(IAsyncEnumerable<>));

        /// <summary>
        /// Performs an action before the <see cref="IAsyncEnumerable{T}"/> is enumerated.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="callback">
        /// The action to perform.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<T> AfterElement<T>(this IAsyncEnumerable<T> source, long index, Action<T> callback)
        {
            return source.Policy.CreateAsyncEnumerable(() => new CallbackEnumerator<T>(source, null, null, index, callback));
        }

        /// <summary>
        /// Gets the element type for the <see cref="IAsyncEnumerable"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public static Type GetElementType(this IAsyncEnumerable source)
        {
            return source.GetType().GetInterface(typeof(IAsyncEnumerable<>)).GenericTypeArguments[0];
        }

        /// <summary>
        /// Converts the <see cref="IAsyncEnumerable"/> to a typed <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The enumerable to convert.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items to convert to.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable"/>.
        /// </returns>
        public static IAsyncEnumerable<T> Convert<T>(this IAsyncEnumerable source)
        {
            var elementType = source.GetType().GetInterface(typeof(IAsyncEnumerable<>)).GenericTypeArguments[0];
            var parameter =
                new object[]
                    {
                        source,
                    };

            if (elementType == typeof(T))
            {
                return (IAsyncEnumerable<T>)source;
            }

            return (IAsyncEnumerable<T>)ConvertInternalMethod.MakeGenericMethod(elementType, typeof(T))
                .Invoke(null, parameter);
        }

        /// <summary>
        /// Performs an action when the <see cref="IAsyncEnumerable{T}"/> is enumerated.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="callback">
        /// The action to perform. The parameter is the number of items in the enumerable.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<T> AfterLastElement<T>(this IAsyncEnumerable<T> source, Action<long> callback)
        {
            return source.Policy.CreateAsyncEnumerable(() => new CallbackEnumerator<T>(source, null, callback, -1, null));
        }

        /// <summary>
        /// Aggregates an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the enumerable.
        /// </typeparam>
        /// <param name="source">
        /// The source <see cref="IAsyncEnumerable{T}"/>.
        /// </param>
        /// <param name="func">
        /// The aggregator.
        /// </param>
        /// <returns>
        /// The result.
        /// </returns>
        public static async Task<TSource> AggregateAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, TSource, Task<TSource>> func)
        {
            var enumerator = source.GetAsyncEnumerator();
            var result = default(TSource);
            var first = true;

            do
            {
                while (enumerator.MoveNext())
                {
                    if (first)
                    {
                        result = enumerator.Current;
                        first = false;
                    }
                    else
                    {
                        result = await func(result, enumerator.Current);
                    }
                }
            }
            while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));

            return result;
        }

        /// <summary>
        /// Aggregates an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the enumerable.
        /// </typeparam>
        /// <param name="source">
        /// The source <see cref="IAsyncEnumerable{T}"/>.
        /// </param>
        /// <param name="func">
        /// The aggregator.
        /// </param>
        /// <returns>
        /// The result.
        /// </returns>
        public static async Task<TSource> AggregateAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            var enumerator = source.GetAsyncEnumerator();
            var result = default(TSource);
            var first = true;

            do
            {
                while (enumerator.MoveNext())
                {
                    if (first)
                    {
                        result = enumerator.Current;
                        first = false;
                    }
                    else
                    {
                        result = func(result, enumerator.Current);
                    }
                }
            }
            while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));

            return result;
        }

        /// <summary>
        /// Aggregates an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the enumerable.
        /// </typeparam>
        /// <typeparam name="TAccumulate">
        /// The result type.
        /// </typeparam>
        /// <param name="source">
        /// The source <see cref="IAsyncEnumerable{T}"/>.
        /// </param>
        /// <param name="seed">
        /// The initial item.
        /// </param>
        /// <param name="func">
        /// The aggregator.
        /// </param>
        /// <returns>
        /// The result.
        /// </returns>
        public static async Task<TAccumulate> AggregateAsync<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            var enumerator = source.GetAsyncEnumerator();

            do
            {
                while (enumerator.MoveNext())
                {
                    seed = func(seed, enumerator.Current);
                }
            }
            while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));

            return seed;
        }

        /// <summary>
        /// Aggregates an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the source enumerable.
        /// </typeparam>
        /// <typeparam name="TAccumulate">
        /// The result type.
        /// </typeparam>
        /// <param name="source">
        /// The source <see cref="IAsyncEnumerable{T}"/>.
        /// </param>
        /// <param name="seed">
        /// The initial item.
        /// </param>
        /// <param name="func">
        /// The aggregator.
        /// </param>
        /// <returns>
        /// The result.
        /// </returns>
        public static async Task<TAccumulate> AggregateAsync<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, Task<TAccumulate>> func)
        {
            var enumerator = source.GetAsyncEnumerator();

            do
            {
                while (enumerator.MoveNext())
                {
                    seed = await func(seed, enumerator.Current);
                }
            }
            while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));

            return seed;
        }

        /// <summary>
        /// The apply enumerable function.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="func">
        /// The enumerable function.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TResult> ApplyEnumerableFunction<TItem, TResult>(this IAsyncEnumerable<TItem> source, Func<IEnumerable<TItem>, TResult> func)
        {
            return Task.Run(() => func(source.ConvertToIEnumerable()));
        }

        /// <summary>
        /// The average async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<float> AverageAsync(this IAsyncEnumerable<float> source)
        {
            var result = await source.AggregateAsync(new Tuple<float, int>(0, 0), (sum, item) => Tuple.Create(sum.Item1 + item, sum.Item2 + 1));

            return result.Item2 == 0 ? 0 : result.Item1 / result.Item2;
        }

        /// <summary>
        /// The average async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<double> AverageAsync(this IAsyncEnumerable<double> source)
        {
            var result = await source.AggregateAsync(new Tuple<double, int>(0, 0), (sum, item) => Tuple.Create(sum.Item1 + item, sum.Item2 + 1));

            return result.Item2 == 0 ? 0 : result.Item1 / result.Item2;
        }

        /// <summary>
        /// The average async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<int> AverageAsync(this IAsyncEnumerable<int> source)
        {
            var result = await source.AggregateAsync(new Tuple<int, int>(0, 0), (sum, item) => Tuple.Create(sum.Item1 + item, sum.Item2 + 1));

            return result.Item2 == 0 ? 0 : result.Item1 / result.Item2;
        }

        /// <summary>
        /// The average async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<long> AverageAsync(this IAsyncEnumerable<long> source)
        {
            var result = await source.AggregateAsync(new Tuple<long, int>(0, 0), (sum, item) => Tuple.Create(sum.Item1 + item, sum.Item2 + 1));

            return result.Item2 == 0 ? 0 : result.Item1 / result.Item2;
        }

        /// <summary>
        /// The average async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<float?> AverageAsync(this IAsyncEnumerable<float?> source)
        {
            var result = await source.AggregateAsync(new Tuple<float?, int>(0, 0), (sum, item) => Tuple.Create(sum.Item1 + (item ?? 0), sum.Item2 + (item == null ? 0 : 1)));

            return result.Item2 == 0 ? null : result.Item1 / result.Item2;
        }

        /// <summary>
        /// The average async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<double?> AverageAsync(this IAsyncEnumerable<double?> source)
        {
            var result = await source.AggregateAsync(new Tuple<double?, int>(0, 0), (sum, item) => Tuple.Create(sum.Item1 + (item ?? 0), sum.Item2 + (item == null ? 0 : 1)));

            return result.Item2 == 0 ? null : result.Item1 / result.Item2;
        }

        /// <summary>
        /// The average async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<int?> AverageAsync(this IAsyncEnumerable<int?> source)
        {
            var result = await source.AggregateAsync(new Tuple<int?, int>(0, 0), (sum, item) => Tuple.Create(sum.Item1 + (item ?? 0), sum.Item2 + (item == null ? 0 : 1)));

            return result.Item2 == 0 ? null : result.Item1 / result.Item2;
        }

        /// <summary>
        /// The average async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<long?> AverageAsync(this IAsyncEnumerable<long?> source)
        {
            var result = await source.AggregateAsync(new Tuple<long?, int>(0, 0), (sum, item) => Tuple.Create(sum.Item1 + (item ?? 0), sum.Item2 + (item == null ? 0 : 1)));

            return result.Item2 == 0 ? null : result.Item1 / result.Item2;
        }

        /// <summary>
        /// Splits the asynchronous enumerable into batches of <paramref name="batchSize"/> elements.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="batchSize">
        /// The batch size.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements.
        /// </typeparam>
        /// <returns>
        /// The groupings.
        /// </returns>
        public static IAsyncEnumerable<IAsyncEnumerable<TSource>> Batch<TSource>(this IAsyncEnumerable<TSource> source, long batchSize)
        {
            return source.Policy.CreateAsyncEnumerable(() => new BatchesEnumerator<TSource>(source, batchSize));
        }

        /// <summary>
        /// Splits the asynchronous enumerable into batches of at most <paramref name="batchSize"/> elements that have the same
        ///     value.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="batchSize">
        /// The batch size.
        /// </param>
        /// <param name="valueSelector">
        /// A function that returns the value that should be equal over a batch.
        /// </param>
        /// <param name="comparer">
        /// Compares values of batches.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the values that must be identical.
        /// </typeparam>
        /// <returns>
        /// The groupings.
        /// </returns>
        public static IAsyncEnumerable<IAsyncEnumerable<TSource>> Batch<TSource, TValue>(this IAsyncEnumerable<TSource> source, int batchSize, Func<TSource, TValue> valueSelector, IComparer<TValue> comparer = null)
        {
            return source.Policy.CreateAsyncEnumerable(() => new ValueBatchesEnumerator<TSource, TValue>(source, source.Policy, batchSize, valueSelector, comparer ?? DefaultComparer.Create<TValue>()));
        }

        /// <summary>
        /// Performs an action before the <see cref="IAsyncEnumerable{T}"/> is enumerated.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="callback">
        /// The action to perform.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<T> BeforeFirstElement<T>(this IAsyncEnumerable<T> source, Action callback)
        {
            return source.Policy.CreateAsyncEnumerable(() => new CallbackEnumerator<T>(source, callback, null, -1, null));
        }

        /// <summary>
        /// The count async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<long> CountAsync<TSource>(this IAsyncEnumerable<TSource> source) => source.CountAsync(null);

        /// <summary>
        /// Counts the number of items in the <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the.
        /// </typeparam>
        /// <param name="source">
        /// The enumerable to count.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The number of elements in the enumerable.
        /// </returns>
        public static async Task<long> CountAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> condition)
        {
            return (source as IAsyncReadOnlyCollection<TSource>)?.Count ??
                   (condition == null
                        ? await source.AggregateAsync(0L, (count, item) => count + 1).ConfigureAwait(false)
                        : await source.AggregateAsync(0L, (count, item) => count + (condition(item) ? 1 : 0)).ConfigureAwait(false));
        }

        /// <summary>
        /// Applies a function to all elements and combines the results.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left items.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right items.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> CrossApply<TLeft, TRight, TResult>(this IAsyncEnumerable<TLeft> source, Func<TLeft, IAsyncEnumerable<TRight>> function, Func<TLeft, TRight, TResult> resultSelector)
        {
            return source.Policy.CreateAsyncEnumerable(() => new ApplyEnumerator<TLeft, TRight, TResult>(false, source, function, resultSelector));
        }

        /// <summary>
        /// Performs a cross join between the two <see cref="IAsyncEnumerable{T}"/>s.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left enumerable.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right enumerable.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> CrossJoin<TLeft, TRight, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<TLeft, TRight, TResult> resultSelector)
        {
            return left.Policy.CreateAsyncEnumerable(() => new CrossJoinEnumerator<TLeft, TRight, TResult>(left, right, resultSelector));
        }

        /// <summary>
        /// The distinct.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TSource> Distinct<TSource>(this IAsyncEnumerable<TSource> source) => source.Distinct(null);

        /// <summary>
        /// Groups the asynchronous enumerable.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements.
        /// </typeparam>
        /// <returns>
        /// The groupings.
        /// </returns>
        public static IAsyncEnumerable<TSource> Distinct<TSource>(this IAsyncEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            return source.Policy.CreateAsyncEnumerable(() => new DistinctEnumerator<TSource>(source, comparer ?? DefaultComparer.Create<TSource>()));
        }

        /// <summary>
        /// Gets the first element.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> FirstAsync<TItem>(this IAsyncEnumerable<TItem> source)
            => FirstInternalAsync(
                source,
                null,
                () => { throw new InvalidOperationException("Sequence contains no matching elements."); });

        /// <summary>
        /// Returns the first item.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> FirstAsync<TItem>(this IAsyncEnumerable<TItem> source, Func<TItem, bool> condition)
            => FirstInternalAsync(
                source,
                condition,
                () => { throw new InvalidOperationException("Sequence contains no matching elements."); });

        /// <summary>
        /// The first or default async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> FirstOrDefaultAsync<TItem>(this IAsyncEnumerable<TItem> source)
            => FirstInternalAsync(source, null, () => default(TItem));

        /// <summary>
        /// Returns the first item for which the <paramref name="condition"/> is <c>true</c>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="condition">
        /// The condition that has to be <c>true</c> for the items.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> FirstOrDefaultAsync<TItem>(this IAsyncEnumerable<TItem> source, Func<TItem, bool> condition) => FirstInternalAsync(source, condition, () => default(TItem));

        /// <summary>
        /// Executes an action for all items in the <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task ForEachAsync<TItem>(this IAsyncEnumerable<TItem> source, Action<TItem> action)
        {
            using (var enumerator = source.GetAsyncEnumerator())
            {
                do
                {
                    while (enumerator.MoveNext())
                    {
                        action(enumerator.Current);
                    }
                }
                while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Executes an action for all items in the <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="asyncAction">
        /// The async action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task ForEachAsync<TItem>(this IAsyncEnumerable<TItem> source, Func<TItem, Task> asyncAction)
        {
            using (var enumerator = source.GetAsyncEnumerator())
            {
                do
                {
                    while (enumerator.MoveNext())
                    {
                        await asyncAction(enumerator.Current).ConfigureAwait(false);
                    }
                }
                while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Executes an action for all items in the <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task ForEachBatchAsync<TItem>(this IAsyncEnumerable<TItem> source, Action<IEnumerable<TItem>> action)
        {
            using (var enumerator = source.GetAsyncEnumerator())
            {
                do
                {
                    action(enumerator.CurrentBatchToEnumerable());
                }
                while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Executes an action for all items in the <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task ForEachBatchAsync<TItem>(this IAsyncEnumerable<TItem> source, Func<IEnumerable<TItem>, Task> action)
        {
            using (var enumerator = source.GetAsyncEnumerator())
            {
                do
                {
                    await action(enumerator.CurrentBatchToEnumerable());
                }
                while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Groups the asynchronous enumerable.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key.
        /// </typeparam>
        /// <returns>
        /// The groupings.
        /// </returns>
        public static IAsyncEnumerable<IAsyncGrouping<TSource, TKey>> GroupBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
        {
            return source.Policy.CreateAsyncEnumerable(() => new GroupByEnumerator<TSource, TKey>(source, keySelector, comparer ?? DefaultComparer.Create<TKey>()));
        }

        /// <summary>
        /// Joins the two <see cref="IAsyncEnumerable{T}"/>s on a key. When no item is found that matches an item in
        ///     <paramref name="left"/>,
        ///     <paramref name="resultSelector"/> is called with the left item and the default for <typeparamref name="TRight"/>.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="leftKeySelector">
        /// The left key selector.
        /// </param>
        /// <param name="rightKeySelector">
        /// The right key selector.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <param name="comparer">
        /// The key comparer.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left item.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right item.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key to join on.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> Join<TLeft, TRight, TKey, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, TResult> resultSelector, IComparer<TKey> comparer = null)
        {
            return left.Policy.CreateAsyncEnumerable(() => new JoinEnumerator<TLeft, TRight, TKey, TResult>(true, false, left, right, leftKeySelector, ExpressionType.Equal, rightKeySelector, null, resultSelector, comparer ?? DefaultComparer.Create<TKey>()));
        }

        /// <summary>
        /// Joins the two <see cref="IAsyncEnumerable{T}"/>s on a key. When no item is found that matches an item in
        ///     <paramref name="left"/>,
        ///     <paramref name="resultSelector"/> is called with the left item and the default for <typeparamref name="TRight"/>.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="leftKeySelector">
        /// The left key selector.
        /// </param>
        /// <param name="joinOperator">
        /// The join operator.
        /// </param>
        /// <param name="rightKeySelector">
        /// The right key selector.
        /// </param>
        /// <param name="resultFilter">
        /// The result filter.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <param name="comparer">
        /// The key comparer.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left item.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right item.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key to join on.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> Join<TLeft, TRight, TKey, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<TLeft, TKey> leftKeySelector, ExpressionType joinOperator, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, bool> resultFilter, Func<TLeft, TRight, TResult> resultSelector, IComparer<TKey> comparer = null)
        {
            if (joinOperator != ExpressionType.Equal && joinOperator != ExpressionType.LessThan && joinOperator != ExpressionType.LessThanOrEqual && joinOperator != ExpressionType.GreaterThan && joinOperator != ExpressionType.GreaterThanOrEqual && joinOperator != ExpressionType.NotEqual)
            {
                throw new ArgumentOutOfRangeException(nameof(joinOperator), "Invalid join operator");
            }

            return left.Policy.CreateAsyncEnumerable(() => new JoinEnumerator<TLeft, TRight, TKey, TResult>(false, false, left, right, leftKeySelector, joinOperator, rightKeySelector, resultFilter, resultSelector, comparer ?? DefaultComparer.Create<TKey>()));
        }

        /// <summary>
        /// The last async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> LastAsync<TItem>(this IAsyncEnumerable<TItem> source)
            => LastInternalAsync(
                source,
                null,
                () =>
                    {
                        throw new InvalidOperationException("Sequence contains no matching elements.");
                    });

        /// <summary>
        /// The last async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> LastAsync<TItem>(this IAsyncEnumerable<TItem> source, Func<TItem, bool> condition)
            => LastInternalAsync(
                source,
                condition,
                () => { throw new InvalidOperationException("Sequence contains no matching elements."); });

        /// <summary>
        /// The last or default async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> LastOrDefaultAsync<TItem>(this IAsyncEnumerable<TItem> source)
            => LastInternalAsync(source, null, () => default(TItem));

        /// <summary>
        /// The last or default async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> LastOrDefaultAsync<TItem>(this IAsyncEnumerable<TItem> source, Func<TItem, bool> condition)
            => LastInternalAsync(source, condition, () => default(TItem));

        /// <summary>
        /// Joins the two <see cref="IAsyncEnumerable{T}"/>s on a key. When no item is found that matches an item in
        ///     <paramref name="left"/>,
        ///     <paramref name="resultSelector"/> is called with the left item and the default for <typeparamref name="TRight"/>.
        ///     When no element can be matched with an element of <paramref name="left"/>,
        ///     the <paramref name="resultSelector"/> is called with the left element and
        ///     <c>default(<typeparamref name="TRight"/>)</c>.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="leftKeySelector">
        /// The left key selector.
        /// </param>
        /// <param name="joinOperator">
        /// The join operator.
        /// </param>
        /// <param name="rightKeySelector">
        /// The right key selector.
        /// </param>
        /// <param name="resultFilter">
        /// The result filter.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <param name="comparer">
        /// The key comparer.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left item.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right item.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key to join on.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<TLeft, TKey> leftKeySelector, ExpressionType joinOperator, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, bool> resultFilter, Func<TLeft, TRight, TResult> resultSelector, IComparer<TKey> comparer = null)
        {
            if (joinOperator != ExpressionType.Equal && joinOperator != ExpressionType.LessThan && joinOperator != ExpressionType.LessThanOrEqual && joinOperator != ExpressionType.GreaterThan && joinOperator != ExpressionType.GreaterThanOrEqual && joinOperator != ExpressionType.NotEqual)
            {
                throw new ArgumentOutOfRangeException(nameof(joinOperator), "Invalid join operator");
            }

            return left.Policy.CreateAsyncEnumerable(() => new JoinEnumerator<TLeft, TRight, TKey, TResult>(true, false, left, right, leftKeySelector, joinOperator, rightKeySelector, resultFilter, resultSelector, comparer ?? DefaultComparer.Create<TKey>()));
        }

        /// <summary>
        /// Retrieves all elements from the <see cref="IAsyncEnumerable{T}"/> from the source and stores them in a persistent
        ///     <see cref="IAsyncEnumerable{T}"/>.
        ///     This means that it can be enumerated multiple times without having to access the source over and over again.
        /// </summary>
        /// <param name="source">
        /// A sequence of values to materialize.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{T}"/> with the same elements as <paramref name="source"/> that can be enumerated
        ///     multiple times.
        /// </returns>
        public static Task<IAsyncReadOnlyCollection<TSource>> MaterializeAsync<TSource>(this IAsyncEnumerable<TSource> source)
        {
            return source.Policy.MaterializeAsync(source);
        }

        /// <summary>
        /// The max async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> MaxAsync<TItem>(this IAsyncEnumerable<TItem> source)
            => source.MaxAsync(null);

        /// <summary>
        /// The max async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> MaxAsync<TItem>(this IAsyncEnumerable<TItem> source, IComparer<TItem> comparer)
        {
            comparer = comparer ?? DefaultComparer.Create<TItem>();

            return source.AggregateAsync(default(TItem), (maxItem, item) => comparer.Compare(item, maxItem) > 0 ? item : maxItem);
        }

        /// <summary>
        /// The min async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> MinAsync<TItem>(this IAsyncEnumerable<TItem> source)
            => source.MinAsync(null);

        /// <summary>
        /// The min async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<TItem> MinAsync<TItem>(this IAsyncEnumerable<TItem> source, IComparer<TItem> comparer)
        {
            comparer = comparer ?? DefaultComparer.Create<TItem>();

            return source.AggregateAsync(default(TItem), (maxItem, item) => comparer.Compare(item, maxItem) < 0 ? item : maxItem);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <param name="source">
        /// A sequence of values to order.
        /// </param>
        /// <param name="keySelector">
        /// A function to extract a key from an element.
        /// </param>
        /// <param name="comparer">
        /// An <see cref="IComparer{T}"/> to compare keys.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key returned by <paramref name="keySelector"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IOrderedAsyncEnumerable{TSource}"/> whose elements are sorted according to a key..
        /// </returns>
        public static IOrderedAsyncEnumerable<TSource> OrderBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
        {
            Comparison<TSource> compareLambda = (first, second) => (comparer ?? DefaultComparer.Create<TKey>()).Compare(keySelector(first), keySelector(second));

            return new OrderedAsyncEnumerable<TSource>(source, compareLambda);
        }

        /// <summary>
        /// Orders the <see cref="IAsyncEnumerable{T}"/> by the <paramref name="orderByExpressions"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="orderByExpressions">
        /// The order by expressions.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<T> OrderBy<T>(this IAsyncEnumerable<T> source, IEnumerable<IOrderByExpression> orderByExpressions)
        {
            Comparison<T> comparison = null;

            foreach (var orderByExpression in orderByExpressions)
            {
                var property = orderByExpression.Expression.GetRowExpression<T>();
                var currentComparison = comparison;
                var compare = orderByExpression.Ascending
                                  ? (Comparison<T>)((first, second) => Comparer<object>.Default.Compare(property(first), property(second)))
                                  : (first, second) => Comparer<object>.Default.Compare(property(second), property(first));

                comparison
                    = comparison == null
                          ? compare
                          : (first, second) =>
                              {
                                  var result = currentComparison(first, second);
                                  return result == 0 ? compare(first, second) : result;
                              };
            }

            return
                comparison == null
                    ? source
                    : source.Policy.CreateAsyncEnumerable(() => new OrderedEnumerator<T>(source, comparison));
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <param name="source">
        /// A sequence of values to order.
        /// </param>
        /// <param name="keySelector">
        /// A function to extract a key from an element.
        /// </param>
        /// <param name="comparer">
        /// An <see cref="IComparer{T}"/> to compare keys.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key returned by <paramref name="keySelector"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IOrderedAsyncEnumerable{TSource}"/> whose elements are sorted according to a key..
        /// </returns>
        public static IOrderedAsyncEnumerable<TSource> OrderByDescending<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
        {
            Comparison<TSource> compareLambda = (first, second) => (comparer ?? DefaultComparer.Create<TKey>()).Compare(keySelector(second), keySelector(first));

            return new OrderedAsyncEnumerable<TSource>(source, compareLambda);
        }

        /// <summary>
        /// Applies a function to all elements and combines the results, when no results are returned, the result selector is
        ///     called with the default value for <typeparamref name="TRight"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left items.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right items.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> OuterApply<TLeft, TRight, TResult>(this IAsyncEnumerable<TLeft> source, Func<TLeft, IAsyncEnumerable<TRight>> function, Func<TLeft, TRight, TResult> resultSelector)
        {
            return source.Policy.CreateAsyncEnumerable(() => new ApplyEnumerator<TLeft, TRight, TResult>(true, source, function, resultSelector));
        }

        /// <summary>
        /// Joins the two <see cref="IAsyncEnumerable{T}"/>s on a key. When no item is found that matches an item in
        ///     <paramref name="left"/>,
        ///     <paramref name="resultSelector"/> is called with the left item and the default for <typeparamref name="TRight"/>.
        ///     Both <see cref="IAsyncEnumerable{T}"/>s
        ///     must be sorted by the keys before calling this method.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="leftKeySelector">
        /// The left key selector.
        /// </param>
        /// <param name="joinOperator">
        /// The join operator.
        /// </param>
        /// <param name="rightKeySelector">
        /// The right key selector.
        /// </param>
        /// <param name="resultFilter">
        /// The result filter.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <param name="comparer">
        /// The key comparer.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left item.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right item.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key to join on.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> PreSortedJoin<TLeft, TRight, TKey, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncReadOnlyCollection<TRight> right, Func<TLeft, TKey> leftKeySelector, ExpressionType joinOperator, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, bool> resultFilter, Func<TLeft, TRight, TResult> resultSelector, IComparer<TKey> comparer = null)
        {
            if (joinOperator != ExpressionType.Equal && joinOperator != ExpressionType.LessThan && joinOperator != ExpressionType.LessThanOrEqual && joinOperator != ExpressionType.GreaterThan && joinOperator != ExpressionType.GreaterThanOrEqual && joinOperator != ExpressionType.NotEqual)
            {
                throw new ArgumentOutOfRangeException(nameof(joinOperator), "Invalid join operator");
            }

            return left.Policy.CreateAsyncEnumerable(() => new JoinEnumerator<TLeft, TRight, TKey, TResult>(false, true, left, right, leftKeySelector, joinOperator, rightKeySelector, resultFilter, resultSelector, comparer ?? DefaultComparer.Create<TKey>()));
        }

        /// <summary>
        /// Joins the two <see cref="IAsyncEnumerable{T}"/>s on a key. When no item is found that matches an item in
        ///     <paramref name="left"/>,
        ///     <paramref name="resultSelector"/> is called with the left item and the default for <typeparamref name="TRight"/>.
        ///     Both <see cref="IAsyncEnumerable{T}"/>s
        ///     must be sorted by the keys before calling this method.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="leftKeySelector">
        /// The left key selector.
        /// </param>
        /// <param name="joinOperator">
        /// The join operator.
        /// </param>
        /// <param name="rightKeySelector">
        /// The right key selector.
        /// </param>
        /// <param name="resultFilter">
        /// The result filter.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <param name="comparer">
        /// The key comparer.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left item.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right item.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key to join on.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> PreSortedLeftJoin<TLeft, TRight, TKey, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncReadOnlyCollection<TRight> right, Func<TLeft, TKey> leftKeySelector, ExpressionType joinOperator, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, bool> resultFilter, Func<TLeft, TRight, TResult> resultSelector, IComparer<TKey> comparer = null)
        {
            if (joinOperator != ExpressionType.Equal && joinOperator != ExpressionType.LessThan && joinOperator != ExpressionType.LessThanOrEqual && joinOperator != ExpressionType.GreaterThan && joinOperator != ExpressionType.GreaterThanOrEqual && joinOperator != ExpressionType.NotEqual)
            {
                throw new ArgumentOutOfRangeException(nameof(joinOperator), "Invalid join operator");
            }

            return left.Policy.CreateAsyncEnumerable(() => new JoinEnumerator<TLeft, TRight, TKey, TResult>(true, true, left, right, leftKeySelector, joinOperator, rightKeySelector, resultFilter, resultSelector, comparer ?? DefaultComparer.Create<TKey>()));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">
        /// A sequence of values to invoke a transform function on.
        /// </param>
        /// <param name="selector">
        /// A transform function to apply to each element.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the value returned by <paramref name="selector"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{T}"/> whose elements are the result of invoking the transform function on each
        ///     element of <paramref name="source"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            return source.Policy.CreateAsyncEnumerable(() => new SelectAsyncEnumerator<TSource, TResult>(source, selector));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">
        /// A sequence of values to invoke a transform function on.
        /// </param>
        /// <param name="selector">
        /// A transform function to apply to each element.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the value returned by <paramref name="selector"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{T}"/> whose elements are the result of invoking the transform function on each
        ///     element of <paramref name="source"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Policy.CreateAsyncEnumerable(() => new SelectEnumerator<TSource, TResult>(source, selector));
        }

        /// <summary>
        /// The skip.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<T> Skip<T>(this IAsyncEnumerable<T> source, long count)
        {
            return source.Policy.CreateAsyncEnumerable(() => new SkipEnumerator<T>(source, count));
        }

        /// <summary>
        /// Sorts the <see cref="IAsyncEnumerable{T}"/> by using the <paramref name="comparison"/>.
        /// </summary>
        /// <param name="source">
        /// A sequence of values to sort.
        /// </param>
        /// <param name="comparison">
        /// The comparison to use when sorting.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <returns>
        /// The sorted <see cref="IAsyncReadOnlyCollection{T}"/>.
        /// </returns>
        public static Task<IAsyncReadOnlyCollection<TSource>> SortAsync<TSource>(this IAsyncEnumerable<TSource> source, Comparison<TSource> comparison)
        {
            return source.Policy.SortAsync(source, comparison);
        }

        /// <summary>
        /// The sum async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<float> SumAsync(this IAsyncEnumerable<float> source) => source.AggregateAsync((sum, item) => sum + item);

        /// <summary>
        /// The sum async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<double> SumAsync(this IAsyncEnumerable<double> source) => source.AggregateAsync((sum, item) => sum + item);

        /// <summary>
        /// The sum async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<int> SumAsync(this IAsyncEnumerable<int> source) => source.AggregateAsync((sum, item) => sum + item);

        /// <summary>
        /// The sum async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<long> SumAsync(this IAsyncEnumerable<long> source) => source.AggregateAsync((sum, item) => sum + item);

        /// <summary>
        /// The sum async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<float?> SumAsync(this IAsyncEnumerable<float?> source) => source.AggregateAsync((sum, item) => sum + item);

        /// <summary>
        /// The sum async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<double?> SumAsync(this IAsyncEnumerable<double?> source) => source.AggregateAsync((sum, item) => sum + item);

        /// <summary>
        /// The sum async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<int?> SumAsync(this IAsyncEnumerable<int?> source) => source.AggregateAsync((sum, item) => sum + item);

        /// <summary>
        /// The sum async.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<long?> SumAsync(this IAsyncEnumerable<long?> source) => source.AggregateAsync((sum, item) => sum + item);

        /// <summary>
        /// The take.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<T> Take<T>(this IAsyncEnumerable<T> source, long count)
        {
            return source.Policy.CreateAsyncEnumerable(() => new TakeEnumerator<T>(source, count));
        }

        /// <summary>
        /// Takes the number of items of the <see cref="IAsyncEnumerable{T}"/>. When <paramref name="count"/> is <c>null</c>,
        ///     all items are returned.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="count">
        /// The number of items to retrieve.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<T> Take<T>(this IAsyncEnumerable<T> source, long? count)
        {
            return count == null ? source : source.Take(count.Value);
        }

        /// <summary>
        /// Converts the enumerable to an array.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The array.
        /// </returns>
        public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> source)
        {
            T[] result = null;

            using (var enumerator = source.GetAsyncEnumerator())
            {
                do
                {
                    result = result == null ? enumerator.CurrentBatchToEnumerable().ToArray() : result.Concat(enumerator.CurrentBatchToEnumerable()).ToArray();
                }
                while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
            }

            return result ?? new T[0];
        }

        /// <summary>
        /// Produces the set union of two sequences by using a specified <see cref="IComparer{T}"/>.
        ///     The order of the rows is not preserved. Uses the materialization policy of the first sequence.
        /// </summary>
        /// <param name="first">
        /// An <see cref="IAsyncReadOnlyCollection{T}"/> whose distinct elements form the first set for the union.
        /// </param>
        /// <param name="second">
        /// An <see cref="IAsyncReadOnlyCollection{T}"/> whose distinct elements form the second set for the union.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> to compare values.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements of the input sequences.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TSource> Union<TSource>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second, IComparer<TSource> comparer = null)
        {
            return first.Policy.CreateAsyncEnumerable(() => new UnionEnumerator<TSource>(first, second, comparer ?? DefaultComparer.Create<TSource>()));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="source">
        /// An <see cref="IAsyncEnumerable{T}"/> to filter.
        /// </param>
        /// <param name="predicate">
        /// A function to test each element for a condition. When this is <c>null</c>, <paramref name="source"/> is returned.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{T}"/> that contains elements from the input sequence that satisfy the condition.
        /// </returns>
        public static IAsyncEnumerable<TSource> Where<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return predicate == null ? source : source.Policy.CreateAsyncEnumerable(() => new WhereEnumerator<TSource>(source, predicate));
        }

        /// <summary>
        /// Enumerates the two <see cref="IAsyncEnumerable{T}"/>s and calls a function on each pair.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left enumerable.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right enumerable.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> Zip<TLeft, TRight, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<TLeft, TRight, TResult> resultSelector)
        {
            return left.Policy.CreateAsyncEnumerable(() => new ZipEnumerator<TLeft, TRight, TResult>(left, right, resultSelector, false));
        }

        /// <summary>
        /// Enumerates the two <see cref="IAsyncEnumerable{T}"/>s and calls a function on each pair.
        ///     When <paramref name="right"/> has less elements than <paramref name="left"/>, they are padded with the default
        ///     value of <typeparamref name="TRight"/>.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="resultSelector">
        /// The result selector.
        /// </param>
        /// <typeparam name="TLeft">
        /// The type of the left enumerable.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The type of the right enumerable.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        public static IAsyncEnumerable<TResult> ZipAll<TLeft, TRight, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<TLeft, TRight, TResult> resultSelector)
        {
            return left.Policy.CreateAsyncEnumerable(() => new ZipEnumerator<TLeft, TRight, TResult>(left, right, resultSelector, true));
        }

        /// <summary>
        /// Converts an async enumerable into an enumerable. This can lead to deadlocks, so use from
        ///     <see cref="Task.Run(System.Action)"/>!.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <param name="source">
        /// The <see cref="IAsyncEnumerable{T}"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        private static IEnumerable<T> ConvertToIEnumerable<T>(this IAsyncEnumerable<T> source)
        {
            using (var enumerator = source.GetAsyncEnumerator())
            {
                do
                {
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
                while (!enumerator.IsSynchronous && enumerator.NextBatchAsync().ConfigureAwait(false).GetAwaiter().GetResult());
            }
        }

        /// <summary>
        /// Returns the first item that applies to the condition.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="actionWhenNotFound">
        /// The action When Not Found.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task<TItem> FirstInternalAsync<TItem>(this IAsyncEnumerable<TItem> source, Func<TItem, bool> condition, Func<TItem> actionWhenNotFound)
        {
            var enumerator = source.GetAsyncEnumerator();
            if (condition != null)
            {
                do
                {
                    while (enumerator.MoveNext())
                    {
                        if (condition(enumerator.Current))
                        {
                            return enumerator.Current;
                        }
                    }
                }
                while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
            }
            else
            {
                do
                {
                    while (enumerator.MoveNext())
                    {
                        return enumerator.Current;
                    }
                }
                while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
            }

            return actionWhenNotFound();
        }

        /// <summary>
        /// Returns the first item that applies to the condition.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="actionWhenNotFound">
        /// The action When Not Found.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task<TItem> LastInternalAsync<TItem>(this IAsyncEnumerable<TItem> source, Func<TItem, bool> condition, Func<TItem> actionWhenNotFound)
        {
            if (source is IAsyncReadOnlyCollection<TItem> materialized)
            {
                if (materialized.Count == 0)
                {
                    return actionWhenNotFound();
                }

                using (var materializedEnumerator = materialized.GetAsyncEnumerator(materialized.Count - 1))
                {
                    await materializedEnumerator.NextBatchAsync();
                    materializedEnumerator.MoveNext();
                    return materializedEnumerator.Current;
                }
            }

            var last = default(TItem);
            var found = false;

            using (var enumerator = source.GetAsyncEnumerator())
            {
                if (condition != null)
                {
                    do
                    {
                        while (enumerator.MoveNext() && condition(enumerator.Current))
                        {
                            last = enumerator.Current;
                            found = true;
                        }
                    }
                    while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
                }
                else
                {
                    do
                    {
                        while (enumerator.MoveNext())
                        {
                            last = enumerator.Current;
                            found = true;
                        }
                    }
                    while (!enumerator.IsSynchronous && await enumerator.NextBatchAsync().ConfigureAwait(false));
                }
            }

            return found ? last : actionWhenNotFound();
        }

        /// <summary>
        /// Converts the <see cref="IAsyncEnumerable"/> to a typed <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The source enumerable.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the source items.
        /// </typeparam>
        /// <typeparam name="TTarget">
        /// The type of the target items.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerable"/>.
        /// </returns>
        private static IAsyncEnumerable<TTarget> ConvertInternal<TSource, TTarget>(IAsyncEnumerable<TSource> source)
        {
            if (typeof(TTarget).IsConstructedGenericType && typeof(TTarget).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return source.Select(item => item == null ? default(TTarget) : (TTarget)System.Convert.ChangeType(item, Nullable.GetUnderlyingType(typeof(TTarget))));
            }

            return source.Select(item => item == null ? default(TTarget) : (TTarget)System.Convert.ChangeType(item, typeof(TTarget)));
        }
    }
}