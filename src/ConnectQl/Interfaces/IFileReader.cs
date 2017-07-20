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
    /// The FileReader interface.
    /// </summary>
    public interface IFileReader : IFileAccess
    {
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
        bool CanReadThisFile(IFileFormatExecutionContext context, string fileName, byte[] firstBytes);

        /// <summary>
        /// Reads objects from the stream.
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
        IEnumerable<Row> Read(IFileFormatExecutionContext context, IRowBuilder rowBuilder, StreamReader reader, HashSet<string> fields);
    }
}