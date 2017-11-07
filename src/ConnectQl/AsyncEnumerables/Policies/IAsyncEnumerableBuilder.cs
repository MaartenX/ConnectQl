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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;

    /// <summary>
    /// The AsyncEnumerableBuilder interface.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items of the <see cref="IAsyncEnumerable{T}"/>.
    /// </typeparam>
    public interface IAsyncEnumerableBuilder<T>
    {
        /// <summary>
        /// Adds items to the <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="items">
        /// The items to add.
        /// </param>
        /// <returns>
        /// This builder.
        /// </returns>
        Task<IAsyncEnumerableBuilder<T>> AddAsync(IEnumerable<T> items);

        /// <summary>
        /// Builds the <see cref="IAsyncEnumerable{T}"/> from the added items.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IAsyncReadOnlyCollection<T>> BuildAsync();
    }
}