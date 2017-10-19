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

namespace ConnectQl.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="IAsyncEnumerable{T}"/>s.
    /// </summary>
    [Trait("Category", "IAsyncEnumerable<T>")]
    public class AsyncEnumerableTests
    {
        /// <summary>
        /// Creates <see cref="IAsyncEnumerable{T}"/>s from an enumerable.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="items">
        /// The elements in the <see cref="IAsyncEnumerable{T}"/>.
        /// </param>
        /// <param name="extraArguments">
        /// The extra arguments.
        /// </param>
        /// <returns>
        /// An array of arrays of enumerables.
        /// </returns>
        public static object[][] CreateEnumerables<T>(IEnumerable<T> items, params object[] extraArguments)
        {
            Func<IAsyncEnumerable<T>> enumerable = () => new InMemoryPolicy().CreateAsyncEnumerable(items);

            var results = new[]
            {
                enumerable().Select(i => i),
                enumerable().Where(i => true),
                enumerable().Skip(0),
                enumerable().Take(5),
                enumerable().Zip(enumerable(), (a, b) => a),
                enumerable().GroupBy(e => e).Select(e => e.FirstAsync()),
                enumerable().Batch(1).Select(b => b.FirstAsync()),
                enumerable().Distinct()
            };

            return results.Select(e => new[] { e }.Concat(extraArguments).ToArray()).ToArray();
        }

        /// <summary>
        /// IAsyncEnumerable{T}.Current should be default(T).
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="enumerable">
        /// The enumerable to test.
        /// </param>
        [Theory(DisplayName = "IAsyncEnumerable<T>.Current should be default(T).")]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new int[] { 1, 2, 3, 4, 5 } })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new object[] { 1, "2", null, 4.0f, 5d } })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new string[] { "1", "2", "3", "4", "5" } })]
        public void CurrentShouldBeDefault<T>(IAsyncEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetAsyncEnumerator();

            Assert.Equal(default(T), enumerator.Current);
        }

        /// <summary>
        /// IAsyncEnumerable{T}.CountAsync() should be 5.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="enumerable">
        /// The enumerable to test.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(DisplayName = "IAsyncEnumerable<T>.CountAsync() should be 5.")]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new int[] { 1, 2, 3, 4, 5 } })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new object[] { 1, "2", null, 4.0f, 5d } })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new string[] { "1", "2", "3", "4", "5" } })]
        public async Task CountShouldBeFive<T>(IAsyncEnumerable<T> enumerable)
        {
            var count = await enumerable.CountAsync();

            Assert.Equal(5, count);
        }

        /// <summary>
        /// IAsyncEnumerable{T}.MaxAsync() should be <paramref name="max"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="enumerable">
        /// The enumerable to test.
        /// </param>
        /// <param name="max">
        /// The maximum value.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(DisplayName = "IAsyncEnumerable<T>.MaxAsync() should be specified max.")]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new int[] { 1, 2, 3, 4, 5 }, 5 })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new object[] { 1, "2", null, 4.0f, 5d }, 5d })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new string[] { "1", "2", "3", "4", "5" }, "5" })]
        public async Task MaxShouldBeSpecifiedMax<T>(IAsyncEnumerable<T> enumerable, T max)
        {
            var count = await enumerable.MaxAsync();

            Assert.Equal(max, count);
        }

        /// <summary>
        /// IAsyncEnumerable{T}.AverageAsync() should be 3.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable to test.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(DisplayName = "IAsyncEnumerable<int>.AverageAsync() should be 3.")]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new int[] { 1, 2, 3, 4, 5 } })]
        public async Task IntAverageShouldBe3(IAsyncEnumerable<int> enumerable)
        {
            var average = await enumerable.AverageAsync();

            Assert.Equal(3, average);
        }

        /// <summary>
        /// IAsyncEnumerable{T}.AverageAsync() should be 3.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable to test.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(DisplayName = "IAsyncEnumerable<float>.AverageAsync() should be 3.")]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new float[] { 1, 2, 3, 4, 5 } })]
        public async Task FloatAverageShouldBe3(IAsyncEnumerable<float> enumerable)
        {
            var average = await enumerable.AverageAsync();

            using (var enumerator = enumerable.GetAsyncEnumerator())
            {
                Assert.Equal(3, average);
            }
        }

        /// <summary>
        /// IAsyncEnumerable{T}.MoveNext() should keep returning false after the first time.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements.
        /// </typeparam>
        /// <param name="enumerable">
        /// The enumerable to test.
        /// </param>
        [Theory(DisplayName = "IAsyncEnumerable<T>.MoveNext() should keep returning false.")]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new int[] { 1, 2, 3, 4, 5 } })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new object[] { 1, "2", null, 4.0f, 5d } })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new string[] { "1", "2", "3", "4", "5" } })]
        public void MoveNextShouldKeepReturningFalse<T>(IAsyncEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetAsyncEnumerator();

            while (enumerator.MoveNext())
            {
                // Do nothing.
            }

            for (var i = 0; i < 100; i++)
            {
                Assert.False(enumerator.MoveNext(), "MoveNext was true again.");
            }
        }

        /// <summary>
        /// An IAsyncEnumerable{T} with IsSynchronous <c>true</c> should not have a next batch.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the <see cref="IAsyncEnumerable{T}"/>
        /// </typeparam>
        /// <param name="enumerable">
        /// The enumerable to test.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(DisplayName = "Synchronous IAsyncEnumerable<T> should not return a next batch.")]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new int[] { 1, 2, 3, 4, 5 } })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new object[] { 1, "2", null, 4.0f, 5d } })]
        [GenericMemberData(nameof(CreateEnumerables), new object[] { new string[] { "1", "2", "3", "4", "5" } })]
        public async Task SynchronousShouldNotReturnNextBatch<T>(IAsyncEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetAsyncEnumerator();

            while (enumerator.MoveNext())
            {
                // Do nothing.
            }

            if (enumerator.IsSynchronous)
            {
                Assert.False(await enumerator.NextBatchAsync());
            }
        }
    }
}
