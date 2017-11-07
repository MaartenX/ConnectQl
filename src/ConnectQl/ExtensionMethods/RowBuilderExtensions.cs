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

// ReSharper disable once CheckNamespace, Extension methods in namespace of extended classes.
namespace ConnectQl.Interfaces
{
    using System.Collections.Generic;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The row builder extensions.
    /// </summary>
    public static class RowBuilderExtensions
    {
        /// <summary>
        /// Creates a row.
        /// </summary>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <param name="uniqueId">
        /// The unique id of the row.
        /// </param>
        /// <param name="fields">
        /// The fields in the row.
        /// </param>
        /// <typeparam name="T">
        /// The type of the unique id of the row.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Row"/>.
        /// </returns>
        public static Row CreateRow<T>([NotNull] this IRowBuilder rowBuilder, T uniqueId, params KeyValuePair<string, object>[] fields)
        {
            return rowBuilder.CreateRow(uniqueId, fields);
        }
    }
}