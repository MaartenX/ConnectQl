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
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Extensions for the loggeri nterface.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Writes a debug-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        [Conditional("DEBUG")]
        public static void Debug(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Debug, exception, format, args);
        }

        /// <summary>
        /// Writes a debug-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        [Conditional("DEBUG")]
        public static void Debug(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Debug, null, format, args);
        }

        /// <summary>
        /// Writes a verbose-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Verbose(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Verbose, exception, format, args);
        }

        /// <summary>
        /// Writes a verbose-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Verbose(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Verbose, null, format, args);
        }

        /// <summary>
        /// Writes a information-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Information(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Information, exception, format, args);
        }

        /// <summary>
        /// Writes a information-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Information(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Information, null, format, args);
        }

        /// <summary>
        /// Writes a warning-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Warning(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Warning, exception, format, args);
        }

        /// <summary>
        /// Writes a warning-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Warning(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Warning, null, format, args);
        }

        /// <summary>
        /// Writes a error-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Error(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Error, exception, format, args);
        }

        /// <summary>
        /// Writes a error-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Error(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Error, null, format, args);
        }
    }
}