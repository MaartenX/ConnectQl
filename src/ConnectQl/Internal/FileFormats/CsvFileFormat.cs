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

namespace ConnectQl.Internal.FileFormats
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    /// <summary>
    ///     The CSV file reader.
    /// </summary>
    public class CsvFileFormat : IFileFormat, IDescriptableFileFormat
    {
        /// <summary>
        ///     The escaped string.
        /// </summary>
        private static readonly Regex EscapedString = new Regex("^\".*\"$");

        /// <summary>
        ///     Gets a value indicating whether the collection of rows should be materialized when
        ///     calling this writer.
        ///     When all columns are needed in the header (e.g. for CSV or Excel), you should return <c>true</c>
        ///     here. Other formats that use the columns per object (like JSON) can return <c>false</c>.
        /// </summary>
        public bool ShouldMaterialize { get; } = true;

        /// <summary>
        ///     Checks if the file reader can read this file.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="fileName">
        ///     The file name.
        /// </param>
        /// <param name="firstBytes">
        ///     The first bytes of the file.
        /// </param>
        /// <returns>
        ///     <c>true</c> if this reader can read the file, <c>false</c> otherwise.
        /// </returns>
        public bool CanReadThisFile(IFileFormatExecutionContext context, string fileName, byte[] firstBytes)
        {
            return fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Checks if the file writer can write this file.
        /// </summary>
        /// <param name="fileName">
        ///     The file name.
        /// </param>
        /// <returns>
        ///     <c>true</c> if this reader can write the file, <c>false</c> otherwise.
        /// </returns>
        public bool CanWriteThisFile(string fileName)
        {
            return fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Gets the descriptor for this data source.
        /// </summary>
        /// <param name="alias">
        ///     The alias.
        /// </param>
        /// <param name="context">
        ///     The execution context.
        /// </param>
        /// <param name="reader">
        ///     The reader.
        /// </param>
        /// <returns>
        ///     The <see cref="System.Threading.Tasks.Task" />.
        /// </returns>
        public Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(string alias, IFileFormatExecutionContext context, StreamReader reader)
        {
            var separator = context.GetDefault("SEPARATOR", false) as string ?? ",";
            return Task.FromResult(Descriptor.ForDataSource(alias, GetHeaders(GetSplitter(separator), reader, separator).Where(header => header.Length > 0).Select(column => Descriptor.ForColumn(column, typeof(string)))));
        }

        /// <summary>
        ///     Reads a reader as comma separated values.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="rowBuilder">
        ///     The data Set.
        /// </param>
        /// <param name="reader">
        ///     The stream.
        /// </param>
        /// <param name="fields">
        ///     The fields, or <c>null</c> to retrieve all fields.
        /// </param>
        /// <returns>
        ///     The rows.
        /// </returns>
        public IEnumerable<Row> Read(IFileFormatExecutionContext context, IRowBuilder rowBuilder, StreamReader reader, HashSet<string> fields)
        {
            var separator = context.GetDefault("SEPARATOR", false) as string ?? ",";
            var splitter = GetSplitter(separator);
            var headers = GetHeaders(splitter, reader, separator);

            if (headers.Length == 1 && string.IsNullOrEmpty(headers[0]))
            {
                yield break;
            }

            var idx = 0L;

            do
            {
                var line = splitter.Matches($"{reader.ReadLine()}{separator}")
                    .Cast<Match>()
                    .Select(match => match.Groups[1].Value)
                    .Select(value => value.Trim())
                    .Select(value => EscapedString.IsMatch(value) ? value.Substring(1, value.Length - 2).Replace("\"\"", "\"") : value)
                    .ToArray();

                if (line.Length == headers.Length)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    yield return rowBuilder.CreateRow(idx++, headers.Select((header, i) => new KeyValuePair<string, object>(header, line[i])));
                }
            }
            while (!reader.EndOfStream);
        }

        /// <summary>
        ///     Writes the footer to the file.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="writer">
        ///     The reader.
        /// </param>
        public void WriteFooter(IFileFormatExecutionContext context, StreamWriter writer)
        {
        }

        /// <summary>
        ///     Writes the header to the file.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="writer">
        ///     The stream.
        /// </param>
        /// <param name="fields">
        ///     The fields.
        /// </param>
        public void WriteHeader(IFileFormatExecutionContext context, StreamWriter writer, IEnumerable<string> fields)
        {
            writer.WriteLine(string.Join(",", fields.Select(c => $"\"{c}\"")));
        }

        /// <summary>
        ///     Writes rows to the file.
        /// </summary>
        /// <param name="context">
        ///     The context.
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
        ///     The number of rows that were written.
        /// </returns>
        public long WriteRows(IFileFormatExecutionContext context, StreamWriter writer, IEnumerable<Row> rows, bool upsert)
        {
            var count = 0L;

            foreach (var row in rows)
            {
                writer.WriteLine(string.Join(",", row.ColumnNames.Select(c => Escape(row[c]))));
                count++;
            }

            return count;
        }

        /// <summary>
        ///     Escapes a value.
        /// </summary>
        /// <param name="o">
        ///     The o.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private static string Escape(object o) =>
            o is Enum || o is string || (o?.GetType().GetTypeInfo().IsValueType ?? false)
                ? $"\"{o.ToString().Replace("\"", "\"\"")}\""
                : o?.ToString();

        /// <summary>
        /// Gets the headers from the reader.
        /// </summary>
        /// <param name="splitter">
        /// The splitter.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="separator">
        /// The separator.
        /// </param>
        /// <returns>
        /// The <see cref="T:string[]"/>.
        /// </returns>
        private static string[] GetHeaders(Regex splitter, TextReader reader, string separator)
        {
            var headers = splitter.Matches($"{reader.ReadLine()}{separator}")
                .Cast<Match>()
                .Select(match => match.Groups[1].Value)
                .Select(header => header.Trim())
                .Select(header => EscapedString.IsMatch(header) ? header.Substring(1, header.Length - 2).Replace("\"\"", "\"") : header)
                .ToArray();

            return headers;
        }

        /// <summary>
        ///     Gets the splitter based on the current default separator.
        /// </summary>
        /// <param name="separator">
        ///     The separator.
        /// </param>
        /// <returns>
        ///     The <see cref="Regex" />.
        /// </returns>
        private static Regex GetSplitter(string separator)
        {
            var splitter = new Regex($@"^?\s*(""(?:[^""]|"""")*""\s*|.*?){Regex.Escape(separator)}");
            return splitter;
        }
    }
}