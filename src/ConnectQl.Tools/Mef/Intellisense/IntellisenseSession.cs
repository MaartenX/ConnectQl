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

namespace ConnectQl.Tools.Mef.Intellisense
{
    using System;
    using System.Collections.Generic;
    using ConnectQl.Intellisense;
    using ConnectQl.Tools.Interfaces;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// The intellisense session.
    /// </summary>
    internal class IntellisenseSession
    {
        /// <summary>
        /// The documents.
        /// </summary>
        private readonly Dictionary<string, ConnectQlDocument> documents = new Dictionary<string, ConnectQlDocument>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The project unique name.
        /// </summary>
        private readonly string projectUniqueName;

        /// <summary>
        /// The provider.
        /// </summary>
        private readonly ConnectQlDocumentProvider provider;

        /// <summary>
        /// The proxy.
        /// </summary>
        private IntellisenseProxy proxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntellisenseSession"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="projectUniqueName">
        /// The project unique name.
        /// </param>
        public IntellisenseSession(ConnectQlDocumentProvider provider, string projectUniqueName)
        {
            this.provider = provider;
            this.projectUniqueName = projectUniqueName;

            var newProxy = new IntellisenseProxy(projectUniqueName, this.documents);

            newProxy.Initialized += this.ProxyOnInitialized;
        }

        /// <summary>
        /// The get document.
        /// </summary>
        /// <param name="textBuffer">
        /// The text buffer.
        /// </param>
        /// <returns>
        /// The <see cref="IDocument"/>.
        /// </returns>
        public IDocument GetDocument(ITextBuffer textBuffer)
        {
            this.provider.DocumentFactoryService.TryGetTextDocument(textBuffer, out var document);

            if (this.documents.TryGetValue(document.FilePath, out var result))
            {
                return result;
            }

            result = this.documents[document.FilePath] =
                         new ConnectQlDocument(document.FilePath)
                             {
                                 Content = textBuffer.CurrentSnapshot.GetText()
                             };

            this.proxy?.UpdateDocument(document.FilePath, result.Content);

            textBuffer.Changed += (o, e) => this.proxy?.UpdateDocument(document.FilePath, result.Content = textBuffer.CurrentSnapshot.GetText());

            return result;
        }

        /// <summary>
        /// The proxy on document updated.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="documentUpdatedEventArgs">
        /// The document updated event args.
        /// </param>
        private void ProxyOnDocumentUpdated(object sender, DocumentUpdatedEventArgs documentUpdatedEventArgs)
        {
            if (this.documents.TryGetValue(documentUpdatedEventArgs.Document.Filename, out var doc))
            {
                doc.UpdateClassification(documentUpdatedEventArgs.Document);
            }
        }

        /// <summary>
        /// The proxy on initialized.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void ProxyOnInitialized(object sender, EventArgs eventArgs)
        {
            ((IntellisenseProxy)sender).Initialized -= this.ProxyOnInitialized;

            var oldProxy = this.proxy;

            this.proxy = (IntellisenseProxy)sender;
            this.proxy.ReloadRequested += this.ProxyOnReloadRequested;
            this.proxy.DocumentUpdated += this.ProxyOnDocumentUpdated;

            if (oldProxy != null)
            {
                oldProxy.DocumentUpdated -= this.ProxyOnDocumentUpdated;
                oldProxy.Dispose();
            }
        }

        /// <summary>
        /// The proxy on reload requested.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void ProxyOnReloadRequested(object sender, EventArgs eventArgs)
        {
            this.proxy.ReloadRequested -= this.ProxyOnReloadRequested;

            var newProxy = new IntellisenseProxy(this.projectUniqueName, this.documents);

            newProxy.Initialized += this.ProxyOnInitialized;
        }
    }
}