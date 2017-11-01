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

namespace ConnectQl.Intellisense
{
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// The ConnectQlContext interface.
    /// </summary>
    internal interface IConnectQlContext
    {
        /// <summary>
        ///     Executes the queries in the stream.
        /// </summary>
        /// <param name="filename">
        ///     The filename.
        /// </param>
        /// <param name="stream">
        ///     The stream. If this is <c>null</c>, the file is loaded using the <see cref="ConnectQlContext.UriResolver"/>.
        /// </param>
        /// <returns>
        ///     The execute result.
        /// </returns>
        [NotNull]
        [PublicAPI]
        Task<byte[]> ExecuteToByteArrayAsync(string filename, Stream stream);
    }
}