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
    /// <summary>
    /// The file formats extensions.
    /// </summary>
    public static class FileFormatsExtensions
    {
        /// <summary>
        /// Adds a file access to the file format collections.
        /// </summary>
        /// <param name="formats">
        /// The formats.
        /// </param>
        /// <param name="format">
        /// The access to add.
        /// </param>
        /// <returns>
        /// The <see cref="IFileFormats"/>.
        /// </returns>
        public static IFileFormats Add(this IFileFormats formats, IFileFormat format)
        {
            return formats.AddFileAccess(format);
        }

        /// <summary>
        /// Adds a file reader to the file format collections.
        /// </summary>
        /// <param name="formats">
        /// The formats.
        /// </param>
        /// <param name="reader">
        /// The reader to add.
        /// </param>
        /// <returns>
        /// The <see cref="IFileFormats"/>.
        /// </returns>
        public static IFileFormats Add(this IFileFormats formats, IFileReader reader)
        {
            return formats.AddFileAccess(reader);
        }

        /// <summary>
        /// Adds a file writer to the file format collections.
        /// </summary>
        /// <param name="formats">
        /// The formats.
        /// </param>
        /// <param name="writer">
        /// The writer to add.
        /// </param>
        /// <returns>
        /// The <see cref="IFileFormats"/>.
        /// </returns>
        public static IFileFormats Add(this IFileFormats formats, IFileWriter writer)
        {
            return formats.AddFileAccess(writer);
        }
    }
}