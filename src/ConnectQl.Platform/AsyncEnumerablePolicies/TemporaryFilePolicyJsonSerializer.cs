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

namespace ConnectQl.Platform.AsyncEnumerablePolicies
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables.Policies;

    using Newtonsoft.Json;

    /// <summary>
    /// The JSON row serializer.
    /// </summary>
    internal class TemporaryFilePolicyJsonSerializer : ISerializer
    {
        /// <summary>
        /// The readers.
        /// </summary>
        private readonly Dictionary<Stream, JsonTextReader> readers = new Dictionary<Stream, JsonTextReader>();

        /// <summary>
        /// The serializer.
        /// </summary>
        private readonly JsonSerializer serializer = new JsonSerializer
                                                         {
                                                             TypeNameHandling = TypeNameHandling.Auto
                                                         };

        /// <summary>
        /// The writers.
        /// </summary>
        private readonly Dictionary<Stream, JsonTextWriter> writers = new Dictionary<Stream, JsonTextWriter>();

        /// <summary>
        /// Reads items from the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="count">
        /// The number of items to read.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The items.
        /// </returns>
        public Task<IEnumerable<T>> ReadAsync<T>(Stream stream, long count)
        {
            return Task.FromResult(this.ReadAsyncInternal<T>(stream, count));
        }

        /// <summary>
        /// Writes the values to the stream.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <param name="stream">
        /// The stream to write to.
        /// </param>
        /// <param name="value">
        /// The items to write.
        /// </param>
        /// <returns>
        /// The number of items written.
        /// </returns>
        public async Task<long> WriteAsync<T>(Stream stream, IEnumerable<T> value)
        {
            if (!this.writers.TryGetValue(stream, out var writer))
            {
                this.writers[stream] = writer = new JsonTextWriter(new StreamWriter(stream));
            }

            var count = 0L;

            foreach (var obj in value)
            {
                this.serializer.Serialize(writer, obj);
                count++;
            }

            await writer.FlushAsync();

            return count;
        }

        /// <summary>
        /// Reads the items from the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="count">
        /// The number of items to read.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items.
        /// </typeparam>
        /// <returns>
        /// The items.
        /// </returns>
        private IEnumerable<T> ReadAsyncInternal<T>(Stream stream, long count)
        {
            if (!this.readers.TryGetValue(stream, out var reader))
            {
                this.readers[stream] = reader = new JsonTextReader(new StreamReader(stream))
                {
                    SupportMultipleContent = true
                };
            }

            while (count-- > 0 && reader.Read() && reader.TokenType == JsonToken.StartObject)
            {
                yield return this.serializer.Deserialize<T>(reader);
            }
        }
    }
}