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

namespace ConnectQl.Ftp.Sources
{
    using System;
    using System.IO;
    using System.Net;
#if NET45
    using System.Runtime.Remoting;
#endif
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ConnectQl.DataSources;
    using ConnectQl.Ftp.ConnectionStrings;
    using ConnectQl.Interfaces;
    using Renci.SshNet;

    /// <summary>
    ///     The FTP data source.
    /// </summary>
    public class FtpDataSource : FileDataSource
    {
        /// <summary>
        ///     The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FtpDataSource" /> class.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        // ReSharper disable once IntroduceOptionalParameters.Global, optional parameters cannot be used in expression lambda's.
        public FtpDataSource(string uri)
            : base(uri)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FtpDataSource" /> class.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="encoding">
        ///     The encoding to use. Defaults to UTF8.
        /// </param>
        public FtpDataSource(string uri, Encoding encoding)
            : base(uri, encoding)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FtpDataSource" /> class.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="encoding">
        ///     The encoding to use. Defaults to UTF8.
        /// </param>
        /// <param name="connectionString">
        ///     The connection string to use.
        /// </param>
        public FtpDataSource(string uri, Encoding encoding, string connectionString)
            : base(uri, encoding)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        ///     Opens the stream.
        /// </summary>
        /// <param name="context">
        ///     The execution context.
        /// </param>
        /// <param name="uriResolveMode">
        ///     The file Mode.
        /// </param>
        /// <param name="fileUri">
        ///     The uri.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        protected override async Task<Stream> OpenStreamAsync(IExecutionContext context, UriResolveMode uriResolveMode, string fileUri)
        {
            var connection = FtpConnectionString.Parse(this.connectionString ?? context.GetDefault("CONNECTIONSTRING", this).ToString());
            var uri = Combine(connection.Uri, this.Uri);

            if (uri.Scheme.Equals("sftp", StringComparison.OrdinalIgnoreCase))
            {
                var client = new SftpClient(uri.Host, uri.Port == -1 ? 22 : uri.Port, connection.Username, connection.Password);

                client.Connect();

                Action dispose = () =>
                    {
                        client.Disconnect();
                        client.Dispose();
                    };

                return uriResolveMode == UriResolveMode.Read
                           ? new FtpStream(client.OpenRead(uri.AbsolutePath), dispose)
                           : new FtpStream(client.OpenWrite(uri.AbsolutePath), dispose);
            }

            bool useSsl;

            if (connection.UseSsl == null && (uri.Port == 22 || uri.Scheme.Equals("ftps", StringComparison.OrdinalIgnoreCase)))
            {
                uri = new UriBuilder(uri)
                                 {
                                     Scheme = "ftp",
                                     Port = uri.Port == -1 ? 22 : uri.Port
                                 }.Uri;
                useSsl = true;
            }
            else
            {
                useSsl = connection.UseSsl.GetValueOrDefault();
            }

            var ftpRequest = (FtpWebRequest)WebRequest.Create(uri);

            ftpRequest.EnableSsl = useSsl;

            ftpRequest.Method = uriResolveMode == UriResolveMode.Read
                                    ? WebRequestMethods.Ftp.DownloadFile
                                    : WebRequestMethods.Ftp.UploadFile;

            ftpRequest.Credentials = new NetworkCredential(connection.Username, connection.Password);

            ftpRequest.UseBinary = true;
            ftpRequest.KeepAlive = true;
            ftpRequest.UsePassive = true;

            return uriResolveMode == UriResolveMode.Read
                ? ((FtpWebResponse)await ftpRequest.GetResponseAsync()).GetResponseStream() 
                : new FtpStream(ftpRequest.GetRequestStream(), () => ftpRequest.GetResponse());
        }

        /// <summary>
        ///     Combines the uri and the path by concatenating the two.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="path">
        ///     The path to add.
        /// </param>
        /// <returns>
        ///     The combined <see cref="Uri" />.
        /// </returns>
        private static Uri Combine(Uri uri, string path)
        {
            var splitted = path.TrimStart('/').Split(
                new[]
                    {
                        '?'
                    },
                2);

            return new UriBuilder(uri)
                       {
                           Path = $"{uri.AbsolutePath.TrimEnd('/')}/{splitted[0]}",
                           Query = splitted.Length == 2 ? splitted[1] : string.Empty
                       }.Uri;
        }

        /// <summary>
        /// The ftp stream.
        /// </summary>
        private class FtpStream : Stream
        {
            /// <summary>
            /// The inner.
            /// </summary>
            private readonly Stream inner;

            /// <summary>
            /// Called when the stream closes.
            /// </summary>
            private readonly Action callWhenClosed;

            /// <summary>
            /// Initializes a new instance of the <see cref="FtpStream"/> class.
            /// </summary>
            /// <param name="inner">
            /// The inner.
            /// </param>
            /// <param name="callWhenClosed">
            /// Called when the stream closes.
            /// </param>
            public FtpStream(Stream inner, Action callWhenClosed)
            {
                this.inner = inner;
                this.callWhenClosed = callWhenClosed;
            }

            /// <summary>Gets a value indicating whether the current stream supports reading.</summary>
            /// <returns>true if the stream supports reading; otherwise, false.</returns>
            public override bool CanRead => this.inner.CanRead;

            /// <summary>Gets a value indicating whether the current stream supports seeking.</summary>
            /// <returns>true if the stream supports seeking; otherwise, false.</returns>
            public override bool CanSeek => this.inner.CanSeek;

            /// <summary>Gets a value indicating whether the current stream can time out.</summary>
            /// <returns>A value that determines whether the current stream can time out.</returns>
            public override bool CanTimeout => this.inner.CanTimeout;

            /// <summary>Gets a value indicating whether the current stream supports writing.</summary>
            /// <returns>true if the stream supports writing; otherwise, false.</returns>
            public override bool CanWrite => this.inner.CanWrite;

            /// <summary>Gets the length in bytes of the stream.</summary>
            /// <returns>A long value representing the length of the stream in bytes.</returns>
            /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
            public override long Length => this.inner.Length;

            /// <summary>Gets or sets the position within the current stream.</summary>
            /// <returns>The current position within the stream.</returns>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
            public override long Position
            {
                get
                {
                    return this.inner.Position;
                }

                set
                {
                    this.inner.Position = value;
                }
            }

            /// <summary>
            ///     Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before timing
            ///     out.
            /// </summary>
            /// <returns>A value, in milliseconds, that determines how long the stream will attempt to read before timing out.</returns>
            /// <exception cref="T:System.InvalidOperationException">
            ///     The <see cref="P:System.IO.Stream.ReadTimeout" /> method always
            ///     throws an <see cref="T:System.InvalidOperationException" />.
            /// </exception>
            public override int ReadTimeout
            {
                get
                {
                    return this.inner.ReadTimeout;
                }

                set
                {
                    this.inner.ReadTimeout = value;
                }
            }

            /// <summary>
            ///     Gets or sets a value, in milliseconds, that determines how long the stream will attempt to write before timing
            ///     out.
            /// </summary>
            /// <returns>A value, in milliseconds, that determines how long the stream will attempt to write before timing out.</returns>
            /// <exception cref="T:System.InvalidOperationException">
            ///     The <see cref="P:System.IO.Stream.WriteTimeout" /> method always
            ///     throws an <see cref="T:System.InvalidOperationException" />.
            /// </exception>
            public override int WriteTimeout
            {
                get
                {
                    return this.inner.WriteTimeout;
                }

                set
                {
                    this.inner.WriteTimeout = value;
                }
            }

            /// <summary>
            ///     Begins an asynchronous read operation. (Consider using
            ///     <see cref="M:System.IO.Stream.ReadAsync(System.Byte[],System.Int32,System.Int32)" /> instead; see the Remarks
            ///     section).
            /// </summary>
            /// <returns>An <see cref="T:System.IAsyncResult" /> that represents the asynchronous read, which could still be pending.</returns>
            /// <param name="buffer">The buffer to read the data into. </param>
            /// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data read from the stream. </param>
            /// <param name="count">The maximum number of bytes to read. </param>
            /// <param name="callback">An optional asynchronous callback, to be called when the read is complete. </param>
            /// <param name="state">
            ///     A user-provided object that distinguishes this particular asynchronous read request from other
            ///     requests.
            /// </param>
            /// <exception cref="T:System.IO.IOException">
            ///     Attempted an asynchronous read past the end of the stream, or a disk error
            ///     occurs.
            /// </exception>
            /// <exception cref="T:System.ArgumentException">One or more of the arguments is invalid. </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
            /// <exception cref="T:System.NotSupportedException">The current Stream implementation does not support the read operation. </exception>
            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return this.inner.BeginRead(buffer, offset, count, callback, state);
            }

            /// <summary>
            ///     Begins an asynchronous write operation. (Consider using
            ///     <see cref="M:System.IO.Stream.WriteAsync(System.Byte[],System.Int32,System.Int32)" /> instead; see the Remarks
            ///     section).
            /// </summary>
            /// <returns>An IAsyncResult that represents the asynchronous write, which could still be pending.</returns>
            /// <param name="buffer">The buffer to write data from. </param>
            /// <param name="offset">The byte offset in <paramref name="buffer" /> from which to begin writing. </param>
            /// <param name="count">The maximum number of bytes to write. </param>
            /// <param name="callback">An optional asynchronous callback, to be called when the write is complete. </param>
            /// <param name="state">
            ///     A user-provided object that distinguishes this particular asynchronous write request from other
            ///     requests.
            /// </param>
            /// <exception cref="T:System.IO.IOException">
            ///     Attempted an asynchronous write past the end of the stream, or a disk error
            ///     occurs.
            /// </exception>
            /// <exception cref="T:System.ArgumentException">One or more of the arguments is invalid. </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
            /// <exception cref="T:System.NotSupportedException">
            ///     The current Stream implementation does not support the write
            ///     operation.
            /// </exception>
            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return this.inner.BeginWrite(buffer, offset, count, callback, state);
            }

            /// <summary>
            ///     Closes the current stream and releases any resources (such as sockets and file handles) associated with the
            ///     current stream. Instead of calling this method, ensure that the stream is properly disposed.
            /// </summary>
            public override void Close()
            {
                this.inner.Close();

                this.callWhenClosed();
            }

            /// <summary>
            ///     Asynchronously reads the bytes from the current stream and writes them to another stream, using a specified
            ///     buffer size and cancellation token.
            /// </summary>
            /// <returns>A task that represents the asynchronous copy operation.</returns>
            /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
            /// <param name="bufferSize">
            ///     The size, in bytes, of the buffer. This value must be greater than zero. The default size is
            ///     81920.
            /// </param>
            /// <param name="cancellationToken">
            ///     The token to monitor for cancellation requests. The default value is
            ///     <see cref="P:System.Threading.CancellationToken.None" />.
            /// </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="destination" /> is null.
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///     <paramref name="bufferSize" /> is negative or zero.
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Either the current stream or the destination stream is disposed.</exception>
            /// <exception cref="T:System.NotSupportedException">
            ///     The current stream does not support reading, or the destination stream
            ///     does not support writing.
            /// </exception>
            public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            {
                return this.inner.CopyToAsync(destination, bufferSize, cancellationToken);
            }

#if NET45

            /// <summary>
            ///     Creates an object that contains all the relevant information required to generate a proxy used to communicate
            ///     with a remote object.
            /// </summary>
            /// <returns>Information required to generate a proxy.</returns>
            /// <param name="requestedType">
            ///     The <see cref="T:System.Type" /> of the object that the new
            ///     <see cref="T:System.Runtime.Remoting.ObjRef" /> will reference.
            /// </param>
            /// <exception cref="T:System.Runtime.Remoting.RemotingException">This instance is not a valid object. </exception>
            /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
            public override ObjRef CreateObjRef(Type requestedType)
            {
                return this.inner.CreateObjRef(requestedType);
            }
#endif

            /// <summary>
            ///     Waits for the pending asynchronous read to complete. (Consider using
            ///     <see cref="M:System.IO.Stream.ReadAsync(System.Byte[],System.Int32,System.Int32)" /> instead; see the Remarks
            ///     section).
            /// </summary>
            /// <returns>
            ///     The number of bytes read from the stream, between zero (0) and the number of bytes you requested. Streams
            ///     return zero (0) only at the end of the stream, otherwise, they should block until at least one byte is available.
            /// </returns>
            /// <param name="asyncResult">The reference to the pending asynchronous request to finish. </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="asyncResult" /> is null.
            /// </exception>
            /// <exception cref="T:System.ArgumentException">
            ///     A handle to the pending read operation is not available.-or-The pending
            ///     operation does not support reading.
            /// </exception>
            /// <exception cref="T:System.InvalidOperationException">
            ///     <paramref name="asyncResult" /> did not originate from a
            ///     <see
            ///         cref="M:System.IO.Stream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" />
            ///     method on the current stream.
            /// </exception>
            /// <exception cref="T:System.IO.IOException">The stream is closed or an internal error has occurred.</exception>
            public override int EndRead(IAsyncResult asyncResult)
            {
                return this.inner.EndRead(asyncResult);
            }

            /// <summary>
            ///     Ends an asynchronous write operation. (Consider using
            ///     <see cref="M:System.IO.Stream.WriteAsync(System.Byte[],System.Int32,System.Int32)" /> instead; see the Remarks
            ///     section).
            /// </summary>
            /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request. </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="asyncResult" /> is null.
            /// </exception>
            /// <exception cref="T:System.ArgumentException">
            ///     A handle to the pending write operation is not available.-or-The pending
            ///     operation does not support writing.
            /// </exception>
            /// <exception cref="T:System.InvalidOperationException">
            ///     <paramref name="asyncResult" /> did not originate from a
            ///     <see
            ///         cref="M:System.IO.Stream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" />
            ///     method on the current stream.
            /// </exception>
            /// <exception cref="T:System.IO.IOException">The stream is closed or an internal error has occurred.</exception>
            public override void EndWrite(IAsyncResult asyncResult)
            {
                this.inner.EndWrite(asyncResult);
            }

            /// <summary>
            ///     When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be
            ///     written to the underlying device.
            /// </summary>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
            public override void Flush()
            {
                this.inner.Flush();
            }

            /// <summary>
            ///     Asynchronously clears all buffers for this stream, causes any buffered data to be written to the underlying
            ///     device, and monitors cancellation requests.
            /// </summary>
            /// <returns>A task that represents the asynchronous flush operation.</returns>
            /// <param name="cancellationToken">
            ///     The token to monitor for cancellation requests. The default value is
            ///     <see cref="P:System.Threading.CancellationToken.None" />.
            /// </param>
            /// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return this.inner.FlushAsync(cancellationToken);
            }

            /// <summary>Obtains a lifetime service object to control the lifetime policy for this instance.</summary>
            /// <returns>
            ///     An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease" /> used to control the lifetime policy
            ///     for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new
            ///     lifetime service object initialized to the value of the
            ///     <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime" /> property.
            /// </returns>
            /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
            public override object InitializeLifetimeService()
            {
                return this.inner.InitializeLifetimeService();
            }

            /// <summary>
            ///     When overridden in a derived class, reads a sequence of bytes from the current stream and advances the
            ///     position within the stream by the number of bytes read.
            /// </summary>
            /// <returns>
            ///     The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
            ///     many bytes are not currently available, or zero (0) if the end of the stream has been reached.
            /// </returns>
            /// <param name="buffer">
            ///     An array of bytes. When this method returns, the buffer contains the specified byte array with the
            ///     values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced
            ///     by the bytes read from the current source.
            /// </param>
            /// <param name="offset">
            ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read
            ///     from the current stream.
            /// </param>
            /// <param name="count">The maximum number of bytes to be read from the current stream. </param>
            /// <exception cref="T:System.ArgumentException">
            ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is
            ///     larger than the buffer length.
            /// </exception>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="buffer" /> is null.
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
            /// </exception>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support reading. </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
            public override int Read(byte[] buffer, int offset, int count)
            {
                return this.inner.Read(buffer, offset, count);
            }

            /// <summary>
            ///     Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by
            ///     the number of bytes read, and monitors cancellation requests.
            /// </summary>
            /// <returns>
            ///     A task that represents the asynchronous read operation. The value of the result parameter
            ///     contains the total number of bytes read into the buffer. The result value can be less than the number of bytes
            ///     requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if
            ///     the end of the stream has been reached.
            /// </returns>
            /// <param name="buffer">The buffer to write the data into.</param>
            /// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data from the stream.</param>
            /// <param name="count">The maximum number of bytes to read.</param>
            /// <param name="cancellationToken">
            ///     The token to monitor for cancellation requests. The default value is
            ///     <see cref="P:System.Threading.CancellationToken.None" />.
            /// </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="buffer" /> is null.
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
            /// </exception>
            /// <exception cref="T:System.ArgumentException">
            ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is
            ///     larger than the buffer length.
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
            /// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous read operation. </exception>
            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return this.inner.ReadAsync(buffer, offset, count, cancellationToken);
            }

            /// <summary>
            ///     Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the
            ///     end of the stream.
            /// </summary>
            /// <returns>The unsigned byte cast to an <see cref="int" />, or -1 if at the end of the stream.</returns>
            /// <exception cref="T:System.NotSupportedException">The stream does not support reading. </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
            public override int ReadByte()
            {
                return this.inner.ReadByte();
            }

            /// <summary>When overridden in a derived class, sets the position within the current stream.</summary>
            /// <returns>The new position within the current stream.</returns>
            /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter. </param>
            /// <param name="origin">
            ///     A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to
            ///     obtain the new position.
            /// </param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
            /// <exception cref="T:System.NotSupportedException">
            ///     The stream does not support seeking, such as if the stream is
            ///     constructed from a pipe or console output.
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.inner.Seek(offset, origin);
            }

            /// <summary>When overridden in a derived class, sets the length of the current stream.</summary>
            /// <param name="value">The desired length of the current stream in bytes. </param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
            /// <exception cref="T:System.NotSupportedException">
            ///     The stream does not support both writing and seeking, such as if the
            ///     stream is constructed from a pipe or console output.
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
            public override void SetLength(long value)
            {
                this.inner.SetLength(value);
            }

            /// <summary>
            ///     When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current
            ///     position within this stream by the number of bytes written.
            /// </summary>
            /// <param name="buffer">
            ///     An array of bytes. This method copies <paramref name="count" /> bytes from
            ///     <paramref name="buffer" /> to the current stream.
            /// </param>
            /// <param name="offset">
            ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the
            ///     current stream.
            /// </param>
            /// <param name="count">The number of bytes to be written to the current stream. </param>
            /// <exception cref="T:System.ArgumentException">
            ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is
            ///     greater than the buffer length.
            /// </exception>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="buffer" />  is null.
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
            /// </exception>
            /// <exception cref="T:System.IO.IOException">An I/O error occured, such as the specified file cannot be found.</exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
            /// <exception cref="T:System.ObjectDisposedException">
            ///     <see cref="M:System.IO.Stream.Write(System.Byte[],System.Int32,System.Int32)" /> was called after the stream was
            ///     closed.
            /// </exception>
            public override void Write(byte[] buffer, int offset, int count)
            {
                this.inner.Write(buffer, offset, count);
            }

            /// <summary>
            ///     Asynchronously writes a sequence of bytes to the current stream, advances the current position within this
            ///     stream by the number of bytes written, and monitors cancellation requests.
            /// </summary>
            /// <returns>A task that represents the asynchronous write operation.</returns>
            /// <param name="buffer">The buffer to write data from.</param>
            /// <param name="offset">
            ///     The zero-based byte offset in <paramref name="buffer" /> from which to begin copying bytes to the
            ///     stream.
            /// </param>
            /// <param name="count">The maximum number of bytes to write.</param>
            /// <param name="cancellationToken">
            ///     The token to monitor for cancellation requests. The default value is
            ///     <see cref="P:System.Threading.CancellationToken.None" />.
            /// </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="buffer" /> is null.
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
            /// </exception>
            /// <exception cref="T:System.ArgumentException">
            ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is
            ///     larger than the buffer length.
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
            /// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous write operation. </exception>
            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return this.inner.WriteAsync(buffer, offset, count, cancellationToken);
            }

            /// <summary>Writes a byte to the current position in the stream and advances the position within the stream by one byte.</summary>
            /// <param name="value">The byte to write to the stream. </param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support writing, or the stream is already closed. </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
            public override void WriteByte(byte value)
            {
                this.inner.WriteByte(value);
            }
        }
    }
}