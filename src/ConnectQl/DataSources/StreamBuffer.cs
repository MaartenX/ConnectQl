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
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Contains a stream and a buffer of the first bytes in the stream.
    /// </summary>
    internal class StreamBuffer : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamBuffer"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">The buffer.</param>
        private StreamBuffer(Stream stream, byte[] buffer)
        {
            this.Stream = stream;
            this.Buffer = buffer;
        }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets the tream.
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// Creates a stream buffer.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="bufferSize">The size of the bufer.</param>
        /// <returns>A <see cref="StreamBuffer"/></returns>
        [ItemNotNull]
        public static async Task<StreamBuffer> CreateAsync([NotNull] Stream stream, int bufferSize)
        {
            int bufferItems;
            var buffer = new byte[bufferSize];

            if (stream.CanSeek)
            {
                var pos = stream.Position;

                stream.Seek(0, SeekOrigin.Begin);

                bufferItems = await stream.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false);

                if (bufferItems < buffer.Length)
                {
                    Array.Resize(ref buffer, bufferItems);
                }

                stream.Seek(pos, SeekOrigin.Begin);

                return new StreamBuffer(stream, buffer);
            }

            bufferItems = await stream.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false);

            if (bufferItems >= buffer.Length)
            {
                return new StreamBuffer(new PartyBufferedStream(buffer, stream), buffer);
            }

            stream.Dispose();

            Array.Resize(ref buffer, bufferItems);

            return new StreamBuffer(new MemoryStream(buffer), buffer);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Buffer = null;
            this.Stream.Dispose();
        }
    }
}