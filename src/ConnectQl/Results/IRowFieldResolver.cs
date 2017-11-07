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

namespace ConnectQl.Results
{
    using System;
    using System.Collections.Generic;

    using ConnectQl.Interfaces;

    /// <summary>
    /// The RowFieldResolver interface.
    /// </summary>
    internal interface IRowFieldResolver : IRowBuilder
    {
        /// <summary>
        /// Gets the fields.
        /// </summary>
        IReadOnlyList<string> Fields { get; }

        /// <summary>
        /// Gets the index for the field in the row.
        /// </summary>
        /// <param name="field">
        /// The field to get the index for.
        /// </param>
        /// <returns>
        /// The index or <c>null</c> if the field wasn't found.
        /// </returns>
        int? GetIndex(string field);

        /// <summary>
        /// Gets the indices for the fields and their values.
        /// </summary>
        /// <param name="values">
        /// The values to get the indices for.
        /// </param>
        /// <returns>
        /// A list of tuples, containing the index and the object at that index, the largest index first.
        /// </returns>
        IList<Tuple<int, object>> GetIndices(IEnumerable<KeyValuePair<string, object>> values);

        /// <summary>
        /// Gets the index for the field with the internal name in the row.
        /// </summary>
        /// <param name="internalName">
        /// The internal name of the field to get the index for.
        /// </param>
        /// <returns>
        /// The index or <c>null</c> if the field wasn't found.
        /// </returns>
        int? GetInternalNameIndex(string internalName);

        /// <summary>
        /// Deserializes a row.
        /// </summary>
        /// <param name="id">The id of the row.</param>
        /// <param name="values">The values in the row.</param>
        /// <returns>The row.</returns>
        Row DeserializeRow(object id, IDictionary<string, object> values);

        /// <summary>
        /// Serializes a row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>The id and values of the row.</returns>
        Tuple<object, IDictionary<string, object>> SerializeRow(Row row);
    }
}