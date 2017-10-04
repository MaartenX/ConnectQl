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

namespace ConnectQl.Tools.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// The ConnectQl document.
    /// </summary>
    internal interface IDocument
    {
        /// <summary>
        /// Raised when the document changed.
        /// </summary>
        event EventHandler<DocumentChangedEventArgs> DocumentChanged;

        /// <summary>
        /// Gets the filename.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// Gets the version of the document.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Gets the classified tokens for the specified span.
        /// </summary>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <returns>
        /// The tokens.
        /// </returns>
        IEnumerable<IClassifiedToken> GetClassifiedTokens(SnapshotSpan span);

        /// <summary>
        /// Gets the classified tokens at the specified token.
        /// </summary>
        /// <param name="point">
        /// The token to get the token at.
        /// </param>
        /// <returns>
        /// The tokens.
        /// </returns>
        IClassifiedToken GetTokenAt(SnapshotPoint point);

        /// <summary>
        /// Gets the classified tokens at the specified token.
        /// </summary>
        /// <param name="token">
        /// The token to get the token at.
        /// </param>
        /// <returns>
        /// The tokens.
        /// </returns>
        IClassifiedToken GetTokenBefore(IClassifiedToken token);

        /// <summary>
        /// Gets the messages for the document.
        /// </summary>
        /// <returns>All messages for this document.</returns>
        IEnumerable<IMessage> GetMessages();

        /// <summary>
        /// Gets the messages for the specified span.
        /// </summary>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <returns>
        /// The messages.
        /// </returns>
        IEnumerable<IMessage> GetMessages(SnapshotSpan span);

        /// <summary>
        /// Gets the functions by name.
        /// </summary>
        /// <param name="name">
        /// The name of the functions.
        /// </param>
        /// <returns>
        /// The function descriptors.
        /// </returns>
        IEnumerable<IFunctionDescriptor> GetFunctionsByName(string name);

        /// <summary>
        /// Gets the available functions.
        /// </summary>
        /// <returns>
        /// The functions.
        /// </returns>
        IEnumerable<IFunctionDescriptor> GetAvailableFunctions();

        /// <summary>
        /// Gets the available sources.
        /// </summary>
        /// <param name="snapshotPoint">
        /// The snapshot token.
        /// </param>
        /// <returns>
        /// The sources.
        /// </returns>
        IEnumerable<IDataSourceDescriptor> GetAvailableSources(SnapshotPoint snapshotPoint);

        /// <summary>
        /// Gets the available variables..
        /// </summary>
        /// <param name="snapshotPoint">
        /// The snapshot token.
        /// </param>
        /// <returns>
        /// The variables.
        /// </returns>
        IEnumerable<IVariableDescriptor> GetAvailableVariables(SnapshotPoint snapshotPoint);

        /// <summary>
        /// Gets the available plugins.
        /// </summary>
        /// <returns>
        /// The list of plugin names.
        /// </returns>
        IEnumerable<string> GetAvailablePlugins();

        /// <summary>
        /// Gets the automatic completions.
        /// </summary>
        /// <param name="current">The current token.</param>
        /// <returns>The completions.</returns>
        IAutoCompletions GetAutoCompletions(IClassifiedToken current);

        /// <summary>
        /// Executes the query in the document.
        /// </summary>
        /// <returns></returns>
        Task<IExecuteResult> ExecuteAsync();
    }
}