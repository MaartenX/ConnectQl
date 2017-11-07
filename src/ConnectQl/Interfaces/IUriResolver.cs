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
    using System.IO;
    using System.Threading.Tasks;

    using ConnectQl.DataSources;

    /// <summary>
    /// Used to resolve uri's.
    /// </summary>
    public interface IUriResolver
    {
        /// <summary>
        /// Resolves an uri to a stream.
        /// </summary>
        /// <param name="uri">The uri to resolve.</param>
        /// <param name="mode">The mode to use when resolving the uri.</param>
        /// <returns>A Task returning a stream.</returns>
        Task<Stream> ResolveToStream(string uri, UriResolveMode mode);

        /// <summary>
        /// Gets the full path of the uri.
        /// </summary>
        /// <param name="uri">The uri to get the full path for.</param>
        /// <returns>
        /// The full path of the uri.
        /// </returns>
        string GetFullPath(string uri);
    }
}