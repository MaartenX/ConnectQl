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

namespace ConnectQl.Ftp
{
    using System.Text;

    using ConnectQl.Ftp.Sources;
    using ConnectQl.Interfaces;

    /// <summary>
    ///     The FTP plugin.
    /// </summary>
    public class Plugin : IConnectQlPlugin
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name => "Ftp";

        /// <summary>
        ///     Registers the plugin on the context.
        /// </summary>
        /// <param name="context">
        ///     The context to register the plugin on.
        /// </param>
        public void RegisterPlugin(IPluginContext context)
        {
            context.Functions
                .AddWithoutSideEffects("ftp", (string uri) => new FtpDataSource(uri))
                .SetDescription("Creates a connection to an FTP file using the default connection string.", "The server relative path of the file.")
                .AddWithoutSideEffects("ftp", (string uri, string encoding) => new FtpDataSource(uri, Encoding.GetEncoding(encoding)))
                .SetDescription("Creates a connection to an FTP file using the specified encoding and the default connection string.", "The server relative path of the file.", "The encoding of the file.")
                .AddWithoutSideEffects("ftp", (string uri, string encoding, string connectionString) => new FtpDataSource(uri, Encoding.GetEncoding(encoding), connectionString))
                .SetDescription("Creates a connection to an FTP file using the specified encoding and connection string.", "The server relative path of the file.", "The encoding of the file.", "The connection string to use");
        }
    }
}