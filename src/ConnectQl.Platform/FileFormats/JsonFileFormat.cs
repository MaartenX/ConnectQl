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

namespace ConnectQl.FileFormats
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;
    using Newtonsoft.Json;

    /// <summary>
    /// The JSON file reader.
    /// </summary>
    public class JsonFileFormat : IFileFormat, IDescriptableFileFormat
    {
        /// <summary>
        /// The new streams.
        /// </summary>
        private readonly HashSet<StreamWriter> newStreams = new HashSet<StreamWriter>();

        /// <summary>
        /// Gets a value indicating whether the collection of rows should be materialized when
        /// calling this writer.
        /// When all columns are needed in the header (e.g. for CSV or Excel), you should return <c>true</c>
        /// here. Other formats that use the columns per object (like JSON) can return <c>false</c>.
        /// </summary>
        public bool ShouldMaterialize { get; } = false;

        /// <summary>
        /// Checks if the file reader can read this file.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="firstBytes">
        /// The first bytes of the file.
        /// </param>
        /// <returns>
        /// <c>true</c> if this reader can read the file, <c>false</c> otherwise.
        /// </returns>
        public bool CanReadThisFile(IFileFormatExecutionContext context, string fileName, byte[] firstBytes)
        {
            return fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || firstBytes.Length > 0 && (firstBytes[0] == '{' || firstBytes[0] == '[');
        }

        /// <summary>
        /// Checks if the file writer can write this file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// <c>true</c> if this reader can write the file, <c>false</c> otherwise.
        /// </returns>
        public bool CanWriteThisFile(string fileName)
        {
            return fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the descriptor for this data source.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="context">
        ///     The execution context.
        /// </param>
        /// <param name="reader">
        ///     The reader.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(string alias, IFileFormatExecutionContext context, StreamReader reader)
        {
            var maxRowsToScan = context.MaxRowsToScan;

            using (var jsonReader = new JsonTextReader(reader))
            {
                var fields = new HashSet<string>();
                var serializer = new JsonSerializer();
                var types = new Dictionary<string, Type>();
                var lines = 0;

                jsonReader.SupportMultipleContent = true;
                jsonReader.Read();

                if (jsonReader.TokenType == JsonToken.StartArray)
                {
                    while (jsonReader.Read() && jsonReader.TokenType == JsonToken.StartObject)
                    {
                        if (++lines > maxRowsToScan)
                        {
                            break;
                        }

                        foreach (var kv in serializer.Deserialize<Dictionary<string, object>>(jsonReader))
                        {
                            var fieldType = kv.Value?.GetType() ?? typeof(object);
                            if (fields.Add(kv.Key))
                            {
                                types[kv.Key] = fieldType;
                            }
                            else
                            {
                                var type = types[kv.Key];
                                if (type != fieldType && type != typeof(object))
                                {
                                    types[kv.Key] = typeof(object);
                                }
                            }
                        }
                    }
                }
                else
                {
                    do
                    {
                        if (lines++ > maxRowsToScan)
                        {
                            break;
                        }

                        foreach (var kv in serializer.Deserialize<Dictionary<string, object>>(jsonReader))
                        {
                            var fieldType = kv.Value?.GetType() ?? typeof(object);
                            if (fields.Add(kv.Key))
                            {
                                types[kv.Key] = fieldType;
                            }
                            else
                            {
                                var type = types[kv.Key];
                                if (type != fieldType && type != typeof(object))
                                {
                                    types[kv.Key] = typeof(object);
                                }
                            }
                        }
                    }
                    while (jsonReader.Read() && jsonReader.TokenType == JsonToken.StartObject);
                }

                return Task.FromResult(Descriptor.ForDataSource(alias, fields.Select(f => Descriptor.ForColumn(f, types[f]))));
            }
        }

        /// <summary>
        /// Reads JSON objects from the stream.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <param name="reader">
        /// The stream.
        /// </param>
        /// <param name="fields">
        /// The fields, or <c>null</c> to retrieve all fields.
        /// </param>
        /// <returns>
        /// The rows.
        /// </returns>
        public IEnumerable<Row> Read(IFileFormatExecutionContext context, IRowBuilder rowBuilder, StreamReader reader, HashSet<string> fields)
        {
            var idx = 0L;
            var serializer = JsonSerializer.Create();
            var filterFields = fields == null
                                   ? (Func<KeyValuePair<string, object>, bool>)(kv => true)
                                   : kv => fields.Contains(kv.Key);

            using (var jsonTextReader = new JsonTextReader(reader))
            {
                jsonTextReader.SupportMultipleContent = true;
                jsonTextReader.Read();

                if (jsonTextReader.TokenType == JsonToken.StartArray)
                {
                    while (jsonTextReader.Read() && jsonTextReader.TokenType == JsonToken.StartObject)
                    {
                        yield return rowBuilder.CreateRow(idx++, serializer.Deserialize<Dictionary<string, object>>(jsonTextReader).Where(filterFields));
                    }
                }
                else
                {
                    do
                    {
                        yield return rowBuilder.CreateRow(idx++, serializer.Deserialize<Dictionary<string, object>>(jsonTextReader).Where(filterFields));
                    }
                    while (jsonTextReader.Read() && jsonTextReader.TokenType == JsonToken.StartObject);
                }
            }
        }

        /// <summary>
        /// Writes the footer to the file.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="writer">
        ///     The stream.
        /// </param>
        public void WriteFooter(IFileFormatExecutionContext context, StreamWriter writer)
        {
            //// If we have no rows, clean up the stream.
            this.newStreams.Remove(writer);

            writer.Write("]");
        }

        /// <summary>
        /// Writes the header to the file.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="writer">
        ///     The stream.
        /// </param>
        /// <param name="fields">
        ///     The fields.
        /// </param>
        public void WriteHeader(IFileFormatExecutionContext context, StreamWriter writer, IEnumerable<string> fields)
        {
            writer.Write("[");

            this.newStreams.Add(writer);
        }

        /// <summary>
        /// Writes rows to the file.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="writer">
        ///     The stream.
        /// </param>
        /// <param name="rows">
        ///     The rows to write.
        /// </param>
        /// <param name="upsert">
        ///     True to upsert, false to insert.
        /// </param>
        /// <returns>
        /// The number of rows that were written.
        /// </returns>
        public long WriteRows(IFileFormatExecutionContext context, StreamWriter writer, IEnumerable<Row> rows, bool upsert)
        {
            var count = 0L;
            var serialize = JsonSerializer.Create();
            var first = this.newStreams.Remove(writer);

            foreach (var row in rows)
            {
                if (!first)
                {
                    writer.Write(',');
                }
                else
                {
                    first = false;
                }

                serialize.Serialize(writer, row.ToDictionary());
                count++;
            }

            return count;
        }
    }
}