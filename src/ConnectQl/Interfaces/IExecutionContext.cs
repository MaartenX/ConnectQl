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
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables.Policies;
    using ConnectQl.DataSources;

    /// <summary>
    /// The ExecuteContext interface.
    /// </summary>
    public interface IExecutionContext : IMaterializationPolicy
    {
        /// <summary>
        /// Gets the maximum rows to scan when determining the columns in a source.
        /// </summary>
        int MaxRowsToScan { get; }

        /// <summary>
        /// Gets the write progress interval.
        /// </summary>
        long WriteProgressInterval { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the available file formats.
        /// </summary>
        IEnumerable<IFileAccess> FileFormats { get; }

        /// <summary>
        /// Gets the value for the specified variable.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the variable.
        /// </typeparam>
        /// <param name="variable">
        /// The name of the variable, including the '@'.
        /// </param>
        /// <returns>
        /// The value of the variable.
        /// </returns>
        T GetVariable<T>(string variable);

        /// <summary>
        /// Opens a file.
        /// </summary>
        /// <param name="uri">
        /// The uri of the file.
        /// </param>
        /// <param name="uriResolveMode">
        /// The file mode.
        /// </param>
        /// <returns>
        /// The stream containing the data of the file.
        /// </returns>
        Task<Stream> OpenStreamAsync(string uri, UriResolveMode uriResolveMode);

        /// <summary>
        /// Gets the default setting for a data source. A 'USE DEFAULT' statement can be used to set a default value for a
        ///     function.
        /// </summary>
        /// <param name="setting">
        /// The default setting get the value for.
        /// </param>
        /// <param name="source">
        /// The data source to get the value for.
        /// </param>
        /// <param name="throwOnError">
        /// <c>true</c>to throw an exception when an error occurs.
        /// </param>
        /// <returns>
        /// The value for the function for the specified source.
        /// </returns>
        object GetDefault(string setting, IDataAccess source, bool throwOnError = true);
    }
}