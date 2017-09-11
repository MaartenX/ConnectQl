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

namespace ConnectQl.Tools.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConnectQl.Interfaces;
    using Interfaces;
    using Internal.Intellisense;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// The ConnectQl document.
    /// </summary>
    internal class ConnectQlDocument : IDocument
    {
        /// <summary>
        /// The functions.
        /// </summary>
        private IReadOnlyList<IFunctionDescriptor> functions = new IFunctionDescriptor[0];

        /// <summary>
        /// The messages.
        /// </summary>
        private IReadOnlyList<IMessage> messages = new IMessage[0];

        /// <summary>
        /// The sources.
        /// </summary>
        private IReadOnlyList<IDataSourceDescriptorRange> sources = new IDataSourceDescriptorRange[0];

        /// <summary>
        /// The tokens.
        /// </summary>
        private IReadOnlyList<IClassifiedToken> tokens = new IClassifiedToken[0];

        /// <summary>
        /// The variables.
        /// </summary>
        private IReadOnlyList<IVariableDescriptorRange> variables = new IVariableDescriptorRange[0];

        /// <summary>
        /// The plugins.
        /// </summary>
        private IReadOnlyList<string> plugins = new string[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectQlDocument"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public ConnectQlDocument(string filename)
        {
            this.Filename = filename;
        }

        /// <summary>
        /// Raised when the document changed.
        /// </summary>
        public event EventHandler<DocumentChangedEventArgs> DocumentChanged;

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Gets or sets the version of the document.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets the classified tokens for the specified span.
        /// </summary>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <returns>
        /// The tokens.
        /// </returns>
        public IEnumerable<IClassifiedToken> GetClassifiedTokens(SnapshotSpan span)
        {
            return this.tokens.SkipWhile(t => t.End < span.Start.Position).TakeWhile(t => t.Start < span.End.Position);
        }

        /// <summary>
        /// Gets the functions by name.
        /// </summary>
        /// <param name="name">
        /// The name of the functions.
        /// </param>
        /// <returns>
        /// The function descriptors.
        /// </returns>
        public IEnumerable<IFunctionDescriptor> GetFunctionsByName(string name)
        {
            return this.functions.Where(f => string.Equals(f.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Gets the available functions.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<IFunctionDescriptor> GetAvailableFunctions()
        {
            return this.functions;
        }

        /// <summary>
        /// Gets the messages for the document.
        /// </summary>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <returns>
        /// The messages.
        /// </returns>
        public IEnumerable<IMessage> GetMessages(SnapshotSpan span)
        {
            return this.messages;
        }

        /// <summary>
        /// Gets the messages for the document.
        /// </summary>
        /// <returns>
        /// All messages for this document.
        /// </returns>
        public IEnumerable<IMessage> GetMessages()
        {
            return this.messages;
        }

        /// <summary>
        /// Gets the classified tokens at the specified point.
        /// </summary>
        /// <param name="point">
        /// The point to get the token at.
        /// </param>
        /// <returns>
        /// The tokens.
        /// </returns>
        public IClassifiedToken GetTokenAt(SnapshotPoint point)
        {
            return this.tokens.SkipWhile(t => t.End < point.Position).FirstOrDefault();
        }



        /// <summary>
        /// Updates the classifications for this document.
        /// </summary>
        /// <param name="document">
        /// The document received from the app domain.
        /// </param>
        public void UpdateClassification(IDocumentDescriptor document)
        {
            var changeType = DocumentChangeType.None;

            if (document.Tokens != null)
            {
                this.tokens = document.Tokens;

                changeType |= DocumentChangeType.Tokens;
            }

            if (document.Messages != null)
            {
                this.messages = document.Messages;

                changeType |= DocumentChangeType.Messages;
            }

            if (document.Functions != null)
            {
                this.functions = document.Functions;

                changeType |= DocumentChangeType.Functions;
            }

            if (document.Sources != null)
            {
                this.sources = document.Sources;

                changeType |= DocumentChangeType.Sources;
            }

            if (document.Variables != null)
            {
                this.variables = document.Variables;

                changeType |= DocumentChangeType.Variables;
            }

            if (document.Plugins != null)
            {
                this.plugins = document.Plugins;

                changeType |= DocumentChangeType.Plugins;
            }

            this.DocumentChanged?.Invoke(this, new DocumentChangedEventArgs(changeType));
        }

        /// <summary>
        /// Gets the available sources.
        /// </summary>
        /// <param name="snapshotPoint">
        /// The snapshot point.
        /// </param>
        /// <returns>
        /// The sources.
        /// </returns>
        public IEnumerable<IDataSourceDescriptor> GetAvailableSources(SnapshotPoint snapshotPoint)
        {
            return this.sources.Where(s => s.Start <= snapshotPoint.Position && s.End >= snapshotPoint.Position).Select(s => s.DataSource);
        }

        /// <summary>
        /// Gets the available variables..
        /// </summary>
        /// <param name="snapshotPoint">
        /// The snapshot point.
        /// </param>
        /// <returns>
        /// The variables.
        /// </returns>
        public IEnumerable<IVariableDescriptor> GetAvailableVariables(SnapshotPoint snapshotPoint)
        {
            return this.variables.Where(v => v.Start < snapshotPoint.Position).OrderByDescending(v => v.Start).Select(v => v.Variable);
        }

        /// <summary>
        /// Gets the classified tokens at the specified token.
        /// </summary>
        /// <param name="token">
        /// The token to get the token at.
        /// </param>
        /// <returns>
        /// The tokens.
        /// </returns>
        public IClassifiedToken GetTokenBefore(IClassifiedToken token)
        {
            return this.tokens.TakeWhile(t => !t.Equals(token)).LastOrDefault();
        }

        /// <summary>
        /// Gets the available plugins.
        /// </summary>
        /// <returns>
        /// The list of plugin names.
        /// </returns>
        public IEnumerable<string> GetAvailablePlugins()
        {
            return this.plugins;
        }

        /// <summary>
        /// Gets the automatic completions.
        /// </summary>
        /// <param name="current">The current token.</param>
        /// <returns>The completions.</returns>
        public IAutoCompletions GetAutoCompletions(IClassifiedToken current)
        {
            return AutoComplete.GetCompletions(this.tokens, current);
        }
    }
}