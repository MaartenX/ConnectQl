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

namespace ConnectQl.DataSources
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    /// <summary>
    /// The file data source.
    /// </summary>
    public class FileDataSource : IDataSource, IDataTarget, IDescriptableDataSource
    {
        /// <summary>
        /// Gets the encoding.
        /// </summary>
        private readonly Encoding encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataSource"/> class.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        // ReSharper disable once IntroduceOptionalParameters.Global, optional parameters cannot be used in expression lambda's.
        public FileDataSource(string uri)
            : this(uri, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataSource"/> class.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <param name="encoding">
        /// The encoding to use. Defaults to UTF8.
        /// </param>
        public FileDataSource(string uri, Encoding encoding)
        {
            this.encoding = encoding ?? Encoding.UTF8;
            this.Uri = uri;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        protected string Uri { get; }

        /// <summary>
        /// Gets the descriptor for this data source.
        /// </summary>
        /// <param name="sourceAlias">
        /// The source alias.
        /// </param>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        async Task<IDataSourceDescriptor> IDescriptableDataSource.GetDataSourceDescriptorAsync(string sourceAlias, IExecutionContext context)
        {
            var reader = this.GetFileReader(context);
            var descriptable = reader as IDescriptableFileFormat;

            if (descriptable == null)
            {
                return Descriptor.DynamicDataSource(sourceAlias);
            }

            using (var streamReader = await this.GetStreamReaderAsync(context, reader))
            {
                return await descriptable.GetDataSourceDescriptorAsync(sourceAlias, new FileFormatExecutionContext(context, this), streamReader).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the data from the file source.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncEnumerable{T}"/>.
        /// </returns>
        IAsyncEnumerable<Row> IDataSource.GetRows(IExecutionContext context, IRowBuilder rowBuilder, IQuery query)
        {
            var whereFilter = query.GetFilter(context);
            var sortOrders = query.GetSortOrders(context).ToArray();
            var fields = query.RetrieveAllFields ? null : new HashSet<string>(query.Fields);
            var fileReader = this.GetFileReader(context);

            if (fileReader == null)
            {
                throw new InvalidOperationException($"Unable to load file with extension '{Path.GetExtension(this.Uri)}', did you load the file format?");
            }

            var fileFormatContext = new FileFormatExecutionContext(context, this);

            return context.CreateAsyncEnumerableAndRunOnce(
                    () => this.GetStreamReaderAsync(context, fileReader),
                    stream => fileReader.Read(fileFormatContext, rowBuilder, stream, fields),
                    stream => stream?.Dispose())
                .Where(whereFilter?.GetRowFilter())
                .OrderBy(sortOrders);
        }

        /// <summary>
        /// Writes the rowsToWrite to the specified target.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rowsToWrite">
        /// The rowsToWrite.
        /// </param>
        /// <param name="upsert">
        /// True to also update records, false to insert.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        async Task<long> IDataTarget.WriteRowsAsync(IExecutionContext context, IAsyncEnumerable<Row> rowsToWrite, bool upsert)
        {
            var fileWriter = context.FileFormats.OfType<IFileWriter>().FirstOrDefault(reader => reader.CanWriteThisFile(this.Uri));

            if (fileWriter == null)
            {
                throw new InvalidOperationException($"Unable to load file with extension '{Path.GetExtension(this.Uri)}', did you load the file format?");
            }

            if (fileWriter.ShouldMaterialize)
            {
                rowsToWrite = await rowsToWrite.MaterializeAsync().ConfigureAwait(false);
            }

            var result = 0L;

            var fileFormatContext = new FileFormatExecutionContext(context, this);

            using (var stream = await this.OpenStreamAsync(context, UriResolveMode.Write, this.Uri))
            {
                using (var writer = new StreamWriter(stream, (fileWriter as IOverrideEncoding)?.Encoding ?? this.encoding))
                {
                    using (var enumerator = rowsToWrite.GetAsyncEnumerator())
                    {
                        var emptyEnumerable = false;

                        while (!enumerator.MoveNext())
                        {
                            if (!enumerator.IsSynchronous && await enumerator.NextBatchAsync())
                            {
                                continue;
                            }

                            emptyEnumerable = true;
                            break;
                        }

                        if (!emptyEnumerable)
                        {
                            var done = false;

                            fileWriter.WriteHeader(fileFormatContext, writer, enumerator.Current.ColumnNames);

                            while (!done)
                            {
                                result += fileWriter.WriteRows(fileFormatContext, writer, CurrentAndRest(enumerator), upsert);
                                done = true;

                                if (enumerator.IsSynchronous)
                                {
                                    continue;
                                }

                                while (await enumerator.NextBatchAsync())
                                {
                                    if (!enumerator.MoveNext())
                                    {
                                        continue;
                                    }

                                    done = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            fileWriter.WriteHeader(fileFormatContext, writer, Enumerable.Empty<string>());
                        }
                    }

                    fileWriter.WriteFooter(fileFormatContext, writer);
                }
            }

            return result;
        }

        /// <summary>
        /// Opens the stream.
        /// </summary>
        /// <param name="context">
        /// The execution context.
        /// </param>
        /// <param name="uriResolveMode">
        /// The file Mode.
        /// </param>
        /// <param name="fileUri">
        /// The uri.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected virtual Task<Stream> OpenStreamAsync(IExecutionContext context, UriResolveMode uriResolveMode, string fileUri)
        {
            return context.OpenStreamAsync(fileUri, uriResolveMode);
        }

        /// <summary>
        /// The current and rest.
        /// </summary>
        /// <param name="enumerator">
        /// The enumerator.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        private static IEnumerable<Row> CurrentAndRest(IAsyncEnumerator<Row> enumerator)
        {
            yield return enumerator.Current;

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        /// <summary>
        /// Gets the file reader.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="IFileReader"/> or <c>null</c> if none was found.
        /// </returns>
        private IFileReader GetFileReader(IExecutionContext context)
        {
            return context.FileFormats.OfType<IFileReader>().FirstOrDefault(reader => reader.CanReadThisFile(new FileFormatExecutionContext(context, this), this.Uri, new byte[0]));
        }

        /// <summary>
        /// Gets the stream reader.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="fileReader">
        /// The file reader.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<StreamReader> GetStreamReaderAsync(IExecutionContext context, IFileReader fileReader)
        {
            return new StreamReader(await this.OpenStreamAsync(context, UriResolveMode.Read, this.Uri), (fileReader as IOverrideEncoding)?.Encoding ?? this.encoding, true);
        }

        /// <summary>
        /// The file format execution context.
        /// </summary>
        private class FileFormatExecutionContext : IFileFormatExecutionContext
        {
            /// <summary>
            /// The data access.
            /// </summary>
            private readonly IDataAccess access;

            /// <summary>
            /// The context.
            /// </summary>
            private readonly IExecutionContext context;

            /// <summary>
            /// Initializes a new instance of the <see cref="FileFormatExecutionContext"/> class.
            /// </summary>
            /// <param name="context">
            /// The context.
            /// </param>
            /// <param name="access">
            /// The access.
            /// </param>
            public FileFormatExecutionContext(IExecutionContext context, IDataAccess access)
            {
                this.context = context;
                this.access = access;
            }

            /// <summary>
            ///     Gets the logger.
            /// </summary>
            public ILog Log => this.context.Log;

            /// <summary>
            /// Gets the maximum rows to scan when determining the columns in a source.
            /// </summary>
            public int MaxRowsToScan => this.context.MaxRowsToScan;

            /// <summary>
            /// Gets the default setting for a data source. A 'USE DEFAULT' statement can be used to set a default value for a
            ///     function.
            /// </summary>
            /// <param name="setting">
            /// The default setting get the value for.
            /// </param>
            /// <param name="throwOnError">
            /// <c>true</c> to throw an exception when an error occurs, false otherwise.
            /// </param>
            /// <returns>
            /// The value for the function for the specified source.
            /// </returns>
            public object GetDefault(string setting, bool throwOnError)
            {
                return this.context.GetDefault(setting, this.access, throwOnError);
            }
        }
    }
}