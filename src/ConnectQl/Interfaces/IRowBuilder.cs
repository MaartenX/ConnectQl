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

namespace ConnectQl.Interfaces
{
    using System.Collections.Generic;

    using ConnectQl.Results;

    /// <summary>
    /// Contains a method to build rows for a data set.
    /// </summary>
    public interface IRowBuilder
    {
        /// <summary>
        /// Creates a row.
        /// </summary>
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
        Row CreateRow<T>(T uniqueId, IEnumerable<KeyValuePair<string, object>> fields);

        /// <summary>
        /// Combines two rows.
        /// </summary>
        /// <param name="first">
        /// The first row.
        /// </param>
        /// <param name="second">
        /// The second row.
        /// </param>
        /// <returns>
        /// The combined rows or <c>null</c> if both rows were <c>null</c>.
        /// </returns>
        Row CombineRows(Row first, Row second);

        /// <summary>
        /// Attaches the row to the current builder.
        /// </summary>
        /// <param name="row">
        /// The row to attach.
        /// </param>
        /// <returns>
        /// The <paramref name="row"/> if it was already attached to the builder, or a copy otherwise.
        /// </returns>
        Row Attach(Row row);
    }
}