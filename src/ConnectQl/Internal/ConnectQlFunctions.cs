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
    using System.Collections.Generic;
    using System.Diagnostics;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Interfaces;

    /// <summary>
    /// The ConnectQl functions.
    /// </summary>
    internal class ConnectQlFunctions : IConnectQlFunctions, IFunctionDictionary
    {
        /// <summary>
        /// The dictionary.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, IFunctionDescriptor> dictionary;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Func<ILogger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectQlFunctions"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        internal ConnectQlFunctions(Dictionary<string, IFunctionDescriptor> dictionary, Func<ILogger> logger)
        {
            this.dictionary = dictionary;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger => this.logger();

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        Dictionary<string, IFunctionDescriptor> IFunctionDictionary.Dictionary => this.dictionary;

        /// <summary>
        /// Adds a key/value pair of key'1 =&gt; function to the dictionary.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when a lambda with the specified number of parameters is already in the dictionary.
        /// </exception>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions AddFunction(string name, IFunctionDescriptor function)
        {
            var keyName = $"{name}'{function.Arguments.Count}";

            if (this.dictionary.ContainsKey(keyName))
            {
                throw new ArgumentException($"Function '{name.ToUpperInvariant()}' with {function.Arguments.Count} parameters is already registered.");
            }

            this.dictionary[keyName] = function;

            return this;
        }
    }
}