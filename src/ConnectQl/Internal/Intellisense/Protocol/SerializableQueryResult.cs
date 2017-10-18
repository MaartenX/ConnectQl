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

namespace ConnectQl.Internal.Intellisense.Protocol
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Internal.Results;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The serializable query result.
    /// </summary>
    internal class SerializableQueryResult : IQueryResult
    {
        /// <summary>
        /// The row builder.
        /// </summary>
        private readonly RowBuilder rowBuilder = new RowBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableQueryResult"/> class.
        /// </summary>
        [UsedImplicitly]
        public SerializableQueryResult()
        {
        }

        /// <summary>
        /// Gets or sets the number of affected records.
        /// </summary>
        public long AffectedRecords { get; [UsedImplicitly] set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        [UsedImplicitly]
        public List<NameValuePair>[] Rows { get; set; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        IAsyncEnumerable<Row> IQueryResult.Rows =>
            new InMemoryPolicy().CreateAsyncEnumerable(this.Rows.Select((r, i) => Row.Create(this.rowBuilder, i, r.Select(nv => new KeyValuePair<string, object>(nv.Name, nv.Value)))));

        /// <summary>
        /// Creates a query result from an existing query result asynchronously.
        /// </summary>
        /// <param name="result">
        /// The result to create it from.
        /// </param>
        /// <returns>
        /// A task returning a serializable result.
        /// </returns>
        [ItemNotNull]
        public static async Task<SerializableQueryResult> CreateAsync([NotNull] IQueryResult result)
        {
            return new SerializableQueryResult
                       {
                           AffectedRecords = result.AffectedRecords,
                           Rows = await SerializableQueryResult.FlattenRowsAsync(result.Rows)
                       };
        }

        /// <summary>
        /// Flattens the rows into a list of arrays of <see cref="NameValuePair"/>.
        /// </summary>
        /// <param name="resultRows">
        /// The rows to flatten.
        /// </param>
        /// <returns>A <see cref="Task{T}"/> that returns the list of arrays.</returns>
        [ItemNotNull]
        private static async Task<List<NameValuePair>[]> FlattenRowsAsync([NotNull] IAsyncEnumerable<Row> resultRows)
        {
            return (await resultRows.ToArrayAsync()).Select(r => r.ToDictionary().Select(kv => new NameValuePair(kv)).ToList()).ToArray();
        }

        /// <summary>
        /// Stores a name/value pair.
        /// </summary>
        internal class NameValuePair
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NameValuePair"/> class.
            /// </summary>
            [UsedImplicitly]
            public NameValuePair()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NameValuePair"/> class.
            /// </summary>
            /// <param name="keyValuePair">
            /// The key value pair.
            /// </param>
            public NameValuePair(KeyValuePair<string, object> keyValuePair)
            {
                this.Name = keyValuePair.Key;
                this.Value = keyValuePair.Value?.ToString();
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; [UsedImplicitly] set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public string Value { get; [UsedImplicitly] set; }
        }
    }
}