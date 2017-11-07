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
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;

    using JetBrains.Annotations;

    /// <summary>
    /// The async enumerable builder extensions.
    /// </summary>
    public static class AsyncEnumerableBuilderExtensions
    {
        /// <summary>
        /// Adds items to the <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the items to add.
        /// </typeparam>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="items">
        /// The items to add.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [ItemNotNull]
        public static async Task<IAsyncEnumerableBuilder<T>> AddAsync<T>([NotNull] this IAsyncEnumerableBuilder<T> builder, IAsyncEnumerable<T> items)
        {
            await items.ForEachBatchAsync(builder.AddAsync);

            return builder;
        }
    }
}