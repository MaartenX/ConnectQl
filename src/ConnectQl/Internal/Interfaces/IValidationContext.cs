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

namespace ConnectQl.Internal.Interfaces
{
    using System.Collections.Generic;

    using ConnectQl.Interfaces;

    /// <summary>
    /// The ValidationContext interface.
    /// </summary>
    internal interface IValidationContext
    {
        /// <summary>
        /// Gets the node data.
        /// </summary>
        INodeDataProvider NodeData { get; }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        IMessageWriter Messages { get; }

        /// <summary>
        /// Gets the file formats.
        /// </summary>
        IFileFormats FileFormats { get; }

        /// <summary>
        /// Gets the loggers.
        /// </summary>
        ICollection<ILogger> Loggers { get; }

        /// <summary>
        /// Gets the plugin with the specified name.
        /// </summary>
        /// <returns>
        /// The plugin, or <c>null</c> if it wasn't found.
        /// </returns>
        IPluginCollection GetPlugins();
    }
}