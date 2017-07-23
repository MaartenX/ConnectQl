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

namespace ConnectQl.Azure.Sources
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ConnectQl.DataSources;
    using ConnectQl.Interfaces;
    using Microsoft.WindowsAzure.Storage;

    /// <summary>
    /// The blob query source.
    /// </summary>
    internal class BlobDataSource : FileDataSource
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobDataSource"/> class.
        /// </summary>
        /// <param name="uri">
        /// The uri for the blob.
        /// </param>
        [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global", Justification = "Cannot use optional parameters in expression lambda's.")]
        public BlobDataSource(string uri)
            : this(uri, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobDataSource"/> class.
        /// </summary>
        /// <param name="uri">
        /// The uri for the bol.
        /// </param>
        /// <param name="connectionString">
        /// The connection string to use.
        /// </param>
        public BlobDataSource(string uri, string connectionString)
            : base(uri)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Opens the stream.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <param name="fileUri">
        /// The file uri.
        /// </param>
        /// <returns>
        /// The <see cref="System.Threading.Tasks.Task"/>.
        /// </returns>
        protected override async Task<Stream> OpenStreamAsync(IExecutionContext context, UriResolveMode mode, string fileUri)
        {
            var blobClient = CloudStorageAccount.Parse(this.connectionString ?? context.GetDefault("CONNECTIONSTRING", this).ToString()).CreateCloudBlobClient();
            var reference = blobClient.GetContainerReference(this.Uri.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).First());
            var parts = this.Uri.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            var block = reference.GetBlockBlobReference(string.Join("/", parts));

            return mode == UriResolveMode.Read ? await block.OpenReadAsync() : await block.OpenWriteAsync();
        }
    }
}