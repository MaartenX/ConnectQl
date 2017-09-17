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

namespace ConnectQl.Logger.Console
{
    using System;
    using ConnectQl.Interfaces;

    /// <summary>
    /// Logs to the console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Writes a message to the logger.
        /// </summary>
        /// <param name="logLevel">The log level to write.</param>
        /// <param name="exception">Optinonal. The <see cref="System.Exception"/> if there was one, <c>null</c> otherwise.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments.</param>
        public void Write(LogLevel logLevel, Exception exception, string format = "", params object[] args)
        {
            var color = ConsoleColor.Gray;

            switch (logLevel)
            {
                case LogLevel.Debug:
                    color = ConsoleColor.DarkGray;
                    break;
                case LogLevel.Verbose:
                    color = ConsoleColor.Gray;
                    break;
                case LogLevel.Warning:
                    color = ConsoleColor.DarkYellow;
                    break;
                case LogLevel.Error:
                    color = ConsoleColor.Red;
                    break;
            }

            var previous = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = color;

                if (format == null && exception != null)
                {
                    format = exception.Message;
                    args = new object[0];
                }

                Console.WriteLine(format ?? string.Empty, args);
            }
            finally
            {
                Console.ForegroundColor = previous;
            }
        }
    }
}