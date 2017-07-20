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
    using System.IO;

    using ConnectQl.Results;

    /// <summary>
    /// The FileWriter interface.
    /// </summary>
    public interface IFileWriter : IFileAccess
    {
        /// <summary>
        /// Gets a value indicating whether the collection of rows should be materialized when
        /// calling this writer.
        /// When all columns are needed in the header (e.g. for CSV or Excel), you should return <c>true</c>
        /// here. Other formats that use the columns per object (like JSON) can return <c>false</c>.
        /// </summary>
        bool ShouldMaterialize { get; }

        /// <summary>
        /// Checks if the file writer can write this file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// <c>true</c> if this reader can write the file, <c>false</c> otherwise.
        /// </returns>
        bool CanWriteThisFile(string fileName);

        /// <summary>
        /// Writes the header to the file.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="writer">
        /// The stream.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        void WriteHeader(IFileFormatExecutionContext context, StreamWriter writer, IEnumerable<string> fields);

        /// <summary>
        /// Writes rows to the file.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="writer">
        /// The stream.
        /// </param>
        /// <param name="rows">
        /// The rows to write.
        /// </param>
        /// <param name="upsert">
        /// True to upsert, false to insert.
        /// </param>
        /// <returns>
        /// The number of rows that were written.
        /// </returns>
        long WriteRows(IFileFormatExecutionContext context, StreamWriter writer, IEnumerable<Row> rows, bool upsert);

        /// <summary>
        /// Writes the footer to the file.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="writer">
        /// The stream.
        /// </param>
        void WriteFooter(IFileFormatExecutionContext context, StreamWriter writer);
    }
}