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

namespace ConnectQl.Internal
{
    using System.Collections;
    using System.Collections.Generic;

    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The file formats implementation.
    /// </summary>
    internal class FileFormatsImplementation : IFileFormats, IEnumerable<IFileAccess>
    {
        /// <summary>
        /// The formats.
        /// </summary>
        private readonly HashSet<IFileAccess> formats = new HashSet<IFileAccess>();

        /// <summary>
        /// Adds a file access method to the file formats.
        /// </summary>
        /// <param name="access">
        /// The file access to add.
        /// </param>
        /// <returns>
        /// The <see cref="IFileFormats"/>.
        /// </returns>
        [NotNull]
        public IFileFormats AddFileAccess(IFileAccess access)
        {
            this.formats.Add(access);

            return this;
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        [NotNull]
        public IEnumerator<IFileAccess> GetEnumerator()
        {
            return this.formats.GetEnumerator();
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}