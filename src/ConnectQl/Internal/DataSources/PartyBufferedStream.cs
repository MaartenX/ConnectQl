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

namespace ConnectQl.Internal.DataSources
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A stream that is partly buffered.
    /// </summary>
    internal class PartyBufferedStream : Stream
    {
        /// <summary>
        /// Stores the buffered items.
        /// </summary>
        private readonly byte[] cachedBuffer;

        /// <summary>
        /// The rest of the stream.
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// The offset in the stream.
        /// </summary>
        private int bufferOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyBufferedStream"/> class.
        /// </summary>
        /// <param name="cachedBuffer">The buffer.</param>
        /// <param name="stream">The stream.</param>
        public PartyBufferedStream(byte[] cachedBuffer, Stream stream)
        {
            this.cachedBuffer = cachedBuffer;
            this.stream = stream;
        }

        /// <summary>Gets a value indicating whether the current stream supports reading.</summary>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanRead => true;

        /// <summary>Gets a value indicating whether the current stream supports seeking.</summary>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanSeek => false;

        /// <summary>Gets a value indicating whether the current stream supports writing.</summary>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanWrite => false;

        /// <summary>Gets the length in bytes of the stream.</summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <filterpriority>1</filterpriority>
        public override long Length => this.stream.Length;

        /// <summary>Gets or sets the position within the current stream.</summary>
        /// <returns>The current position within the stream.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <filterpriority>1</filterpriority>
        public override long Position
        {
            get => this.stream.Position - this.cachedBuffer.Length + this.bufferOffset;
            set => throw new NotSupportedException();
        }

        /// <summary>Not supported in the stream..</summary>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests.</summary>
        /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the total number of bytes read into the buffer. The result value can be less than the number of bytes requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached. </returns>
        /// <param name="buffer">The buffer to write the data into.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data from the stream.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
        /// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous read operation. </exception>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (this.bufferOffset >= this.cachedBuffer.Length)
            {
                return await this.stream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
            }

            var copyCount = this.cachedBuffer.Length - this.bufferOffset >= count ? count : this.cachedBuffer.Length - this.bufferOffset;

            Array.Copy(this.cachedBuffer, this.bufferOffset, buffer, offset, copyCount);

            this.bufferOffset += copyCount;

            return copyCount < count
                ? copyCount + await this.stream.ReadAsync(buffer, offset + copyCount, count - copyCount, cancellationToken).ConfigureAwait(false)
                : count;
        }

        /// <summary>When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source. </param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream. </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream. </param>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <filterpriority>1</filterpriority>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.bufferOffset >= this.cachedBuffer.Length)
            {
                return this.stream.Read(buffer, offset, count);
            }

            var copyCount = this.cachedBuffer.Length - this.bufferOffset >= count ? count : this.cachedBuffer.Length - this.bufferOffset;
            Array.Copy(this.cachedBuffer, this.bufferOffset, buffer, offset, copyCount);
            return copyCount < count
                ? copyCount + this.stream.Read(buffer, offset + copyCount, count - copyCount)
                : count;
        }

        /// <summary>Not supported in the stream.</summary>
        /// <returns>The new position within the current stream.</returns>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter. </param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position. </param>
        /// <exception cref="T:System.NotSupportedException">Always thrown. The stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>Not supported in the stream.</summary>
        /// <param name="value">The desired length of the current stream in bytes. </param>
        /// <exception cref="T:System.NotSupportedException">Always thrown. The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>Not supported in the stream.</summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream. </param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream. </param>
        /// <param name="count">The number of bytes to be written to the current stream. </param>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" />  is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.NotSupportedException">Always thrown. The stream does not support writing.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}