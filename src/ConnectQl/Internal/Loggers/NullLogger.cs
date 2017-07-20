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

namespace ConnectQl.Internal.Loggers
{
    using System.Collections.Generic;

    using ConnectQl.Interfaces;

    /// <summary>
    /// The null logger.
    /// </summary>
    internal class NullLogger : ILog
    {
        /// <summary>
        /// The messages.
        /// </summary>
        private readonly List<string> messages = new List<string>();

        /// <summary>
        /// Writes a debug message to the log.
        /// </summary>
        /// <param name="format">
        /// The format string.
        /// </param>
        /// <param name="args">
        /// The objects to format.
        /// </param>
        public void Debug(string format, params object[] args)
        {
            this.messages.Add("D" + string.Format(format, args));
        }

        /// <summary>
        /// Writes a error message to the log.
        /// </summary>
        /// <param name="format">
        /// The format string.
        /// </param>
        /// <param name="args">
        /// The objects to format.
        /// </param>
        public void Error(string format, params object[] args)
        {
            this.messages.Add("E" + string.Format(format, args));
        }

        /// <summary>
        /// Forwards written messages to a logger.
        /// </summary>
        /// <param name="to">
        /// The logger to forward to.
        /// </param>
        public void ForwardMessages(ILog to)
        {
            foreach (var message in this.messages)
            {
                switch (message[0])
                {
                    case 'E':
                        to.Error(message.Substring(1));
                        break;
                    case 'W':
                        to.Warning(message.Substring(1));
                        break;
                    case 'I':
                        to.Information(message.Substring(1));
                        break;
                    case 'V':
                        to.Verbose(message.Substring(1));
                        break;
                    case 'D':
                        to.Debug(message.Substring(1));
                        break;
                }
            }
        }

        /// <summary>
        /// Writes an information message to the log.
        /// </summary>
        /// <param name="format">
        /// The format string.
        /// </param>
        /// <param name="args">
        /// The objects to format.
        /// </param>
        public void Information(string format, params object[] args)
        {
            this.messages.Add("I" + string.Format(format, args));
        }

        /// <summary>
        /// Writes a verbose message to the log.
        /// </summary>
        /// <param name="format">
        /// The format string.
        /// </param>
        /// <param name="args">
        /// The objects to format.
        /// </param>
        public void Verbose(string format, params object[] args)
        {
            this.messages.Add("V" + string.Format(format, args));
        }

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="format">
        /// The format string.
        /// </param>
        /// <param name="args">
        /// The objects to format.
        /// </param>
        public void Warning(string format, params object[] args)
        {
            this.messages.Add("W" + string.Format(format, args));
        }
    }
}