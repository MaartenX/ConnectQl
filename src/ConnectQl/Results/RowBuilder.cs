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

namespace ConnectQl.Internal.Results
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The row builder.
    /// </summary>
    internal class RowBuilder : IRowFieldResolver
    {
        /// <summary>
        /// Translates fields to their display names.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly FieldMapping fieldMapping;

        /// <summary>
        /// The field names.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> fieldNames = new List<string>();

        /// <summary>
        /// The fields.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, int> fields = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="RowBuilder"/> class.
        /// </summary>
        public RowBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowBuilder"/> class.
        /// </summary>
        /// <param name="fieldMapping">
        /// The field mapping.
        /// </param>
        public RowBuilder(FieldMapping fieldMapping)
        {
            this.fieldMapping = fieldMapping;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowBuilder"/> class.
        /// </summary>
        /// <param name="alias">
        /// The alias that will be prepended to all fields.
        /// </param>
        public RowBuilder(string alias)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        public IReadOnlyList<string> Fields => this.fieldMapping?.Fields ?? this.fieldNames;

        /// <summary>
        /// Attaches the row to the current builder.
        /// </summary>
        /// <param name="row">
        /// The row to attach.
        /// </param>
        /// <returns>
        /// The <paramref name="row"/> if it was already attached to the builder, or a copy otherwise.
        /// </returns>
        public Row Attach([NotNull] Row row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            return row.Resolver == this ? row : row.Clone(this);
        }

        /// <summary>
        /// Deserializes the row.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The row.
        /// </returns>
        Row IRowFieldResolver.DeserializeRow(object id, IDictionary<string, object> values)
        {
            return Row.Create(this, id, values);
        }

        /// <summary>
        /// Serializes a row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>The id and values of the row.</returns>
        Tuple<object, IDictionary<string, object>> IRowFieldResolver.SerializeRow([NotNull] Row row)
        {
            var values = row.ToDictionary();

            if (this.fieldMapping != null)
            {
                values = values.ToDictionary(kv => this.fieldMapping[kv.Key], kv => kv.Value);
            }

            return Tuple.Create(row.UniqueId, values);
        }

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
        public Row CombineRows([CanBeNull] Row first, [CanBeNull] Row second)
        {
            return first == null ? this.Attach(second) : (second == null ? this.Attach(first) : first.CombineWith(this, second));
        }

        /// <summary>
        /// Creates a row.
        /// </summary>
        /// <param name="uniqueId">
        /// The unique id of the row.
        /// </param>
        /// <param name="values">
        /// The key/values in the row.
        /// </param>
        /// <typeparam name="T">
        /// The type of the unique id of the row.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Row"/>.
        /// </returns>
        public Row CreateRow<T>(T uniqueId, IEnumerable<KeyValuePair<string, object>> values)
        {
            return Row.Create(this, uniqueId, this.Alias == null ? values : values.Select(kv => new KeyValuePair<string, object>($"{this.Alias}.{kv.Key}", kv.Value)));
        }

        /// <summary>
        /// Gets the index for the field in the row.
        /// </summary>
        /// <param name="field">
        /// The field to get the index for.
        /// </param>
        /// <returns>
        /// The index or <c>null</c> if the field wasn't found.
        /// </returns>
        public int? GetIndex(string field)
        {
            return this.fields.TryGetValue(this.fieldMapping == null ? field : this.fieldMapping[field], out var index) ? (int?)index : null;
        }

        /// <summary>
        /// Gets the indices for the fields and their values.
        /// </summary>
        /// <param name="values">
        /// The values to get the indices for.
        /// </param>
        /// <returns>
        /// A list of tuples, containing the index and the object at that index, the largest index first.
        /// </returns>
        [NotNull]
        public IList<Tuple<int, object>> GetIndices([NotNull] IEnumerable<KeyValuePair<string, object>> values)
        {
            var result = new List<Tuple<int, object>>();
            var missing = new List<Tuple<string, int>>();

            foreach (var keyValuePair in values)
            {
                var index = this.fields.TryGetValue(keyValuePair.Key, out var fieldIndex) ? (int?)fieldIndex : null;

                if (index == null)
                {
                    missing.Add(Tuple.Create(keyValuePair.Key, result.Count));
                }

                result.Add(Tuple.Create(index.GetValueOrDefault(), keyValuePair.Value));
            }

            if (missing.Count > 0)
            {
                foreach (var field in missing.Select(m => m.Item1))
                {
                    this.fields.Add(field, this.fields.Count);
                    this.fieldNames.Add(field);
                }

                this.fieldMapping?.AddRowFields(missing.Select(m => m.Item1));

                foreach (var tuple in missing)
                {
                    result[tuple.Item2] = Tuple.Create(this.fields[tuple.Item1], result[tuple.Item2].Item2);
                }
            }

            result.Sort((left, right) => Comparer<int>.Default.Compare(right.Item1, left.Item1));

            return result;
        }

        /// <summary>
        /// Gets the index for the field with the internal name in the row.
        /// </summary>
        /// <param name="internalName">
        /// The internal name of the field to get the index for.
        /// </param>
        /// <returns>
        /// The index or <c>null</c> if the field wasn't found.
        /// </returns>
        public int? GetInternalNameIndex(string internalName)
        {
            return this.fields.TryGetValue(internalName, out var index) ? (int?)index : null;
        }
    }
}