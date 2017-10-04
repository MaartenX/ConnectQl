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

namespace ConnectQl.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// A collection of loggers.
    /// </summary>
    internal class LoggerCollection : ICollection<ILogger>, ILogger, IDisposable
    {
        private readonly ICollection<LogLine> logLines = new List<LogLine>();
        private readonly ICollection<ILogger> loggers;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerCollection"/> class.
        /// </summary>
        public LoggerCollection()
        {
            this.loggers = new List<ILogger>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerCollection"/> class.
        /// </summary>
        /// <param name="loggers">
        /// The collection to base this logger collection on.
        /// </param>
        public LoggerCollection([NotNull] ICollection<ILogger> loggers)
        {
            this.loggers = loggers.ToList();
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count => this.loggers.Count;

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly => this.loggers.IsReadOnly;

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ILogger> GetEnumerator()
        {
            return this.loggers.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.loggers).GetEnumerator();
        }

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(ILogger item)
        {
            foreach (var logLine in this.logLines)
            {
                logLine.WriteTo(item);
            }

            this.loggers.Add(item);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear()
        {
            this.loggers.Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(ILogger item)
        {
            return this.loggers.Contains(item);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo(ILogger[] array, int arrayIndex)
        {
            this.loggers.CopyTo(array, arrayIndex);
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(ILogger item)
        {
            return this.loggers.Remove(item);
        }

        /// <summary>
        /// Writes a message to the logger.
        /// </summary>
        /// <param name="logLevel">The log level to write.</param>
        /// <param name="exception">Optinonal. The <see cref="Exception"/> if there was one, <c>null</c> otherwise.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments.</param>
        public void Write(LogLevel logLevel, Exception exception, string format = "", params object[] args)
        {
            this.logLines.Add(new LogLine(logLevel, exception, format, args));

            foreach (var logger in this.loggers)
            {
                logger.Write(logLevel, exception, format, args);
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.logLines.Clear();
            this.loggers.Clear();
        }

        /// <summary>
        /// Stores a log line.
        /// </summary>
        private class LogLine
        {
            /// <summary>
            /// Stores the log level.
            /// </summary>
            private readonly LogLevel logLevel;

            /// <summary>
            /// Stores the exception.
            /// </summary>
            private readonly Exception exception;

            /// <summary>
            /// Stores the format string.
            /// </summary>
            private readonly string format;

            /// <summary>
            /// Stores the arguments.
            /// </summary>
            private readonly object[] args;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogLine"/> class.
            /// </summary>
            /// <param name="logLevel">The log level.</param>
            /// <param name="exception">The exception.</param>
            /// <param name="format">The format string.</param>
            /// <param name="args">The arguments.</param>
            public LogLine(LogLevel logLevel, Exception exception, string format, object[] args)
            {
                this.logLevel = logLevel;
                this.exception = exception;
                this.format = format;
                this.args = args;
            }

            /// <summary>
            /// Writes the log line to the logger.
            /// </summary>
            /// <param name="logger">
            /// The logger to write to.
            /// </param>
            public void WriteTo([NotNull] ILogger logger)
            {
                logger.Write(this.logLevel, this.exception, this.format, this.args);
            }
        }
    }
}