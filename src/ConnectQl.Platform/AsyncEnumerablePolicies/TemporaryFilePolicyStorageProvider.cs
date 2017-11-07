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

    /// <summary>
    /// The temporary file policy storage provider.
    /// </summary>
    internal class TemporaryFilePolicyStorageProvider : IStorageProvider
    {
        /// <summary>
        /// The files.
        /// </summary>
        private readonly Dictionary<int, string> files = new Dictionary<int, string>();

        /// <summary>
        /// Deletes a file by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="System.Threading.Tasks.Task"/>.
        /// </returns>
        public Task DeleteFileAsync(int id)
        {
            if (this.files.TryGetValue(id, out var path))
            {
                File.Delete(path);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets the file by its id. Returns an opened stream.
        /// </summary>
        /// <param name="id">
        /// The id of the file. When this is a new id, the file will be created.
        /// </param>
        /// <param name="access">
        /// The file access. Can be read or write.
        /// </param>
        /// <returns>
        /// The stream.
        /// </returns>
        public Task<Stream> GetFileAsync(int id, FileAccessType access)
        {
            if (!this.files.TryGetValue(id, out var path))
            {
                this.files[id] = path = Path.GetTempFileName();
            }

            return Task.FromResult<Stream>(
                access == FileAccessType.Read
                    ? File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                    : File.Open(path, FileMode.Truncate, FileAccess.Write, FileShare.Read));
        }
    }
}