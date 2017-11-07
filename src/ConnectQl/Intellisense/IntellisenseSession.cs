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

namespace ConnectQl.Intellisense
{
    using System;
    using System.Collections.Generic;

    using ConnectQl.Intellisense.Protocol;
    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The <c>Intellisense</c> session.
    /// </summary>
    internal class IntellisenseSession : IIntellisenseSession
    {
        /// <summary>
        /// The documents.
        /// </summary>
        private readonly Dictionary<string, Document> documents = new Dictionary<string, Document>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="IntellisenseSession"/> class.
        /// </summary>
        /// <param name="context">
        /// The context to create the intellisense session for.
        /// </param>
        internal IntellisenseSession([NotNull] ConnectQlContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Occurs when a document is updated.
        /// </summary>
        public event EventHandler<DocumentUpdatedEventArgs> DocumentUpdated;

        /// <summary>
        /// Occurs when a document is updated (used internally for cross-appdomain communication).
        /// </summary>
        [UsedImplicitly]
        public event EventHandler<byte[]> InternalDocumentUpdated;

        /// <summary>
        /// Gets the context.
        /// </summary>
        internal ConnectQlContext Context { get; }

        /// <summary>
        /// Deserializes a serialized document.
        /// </summary>
        /// <param name="serializedDocument">
        /// The serialized document.
        /// </param>
        /// <returns>
        /// The <see cref="IDocumentDescriptor"/>.
        /// </returns>
        public static IDocumentDescriptor Deserialize(byte[] serializedDocument)
        {
            return ProtocolSerializer.Deserialize<SerializableDocumentDescriptor>(serializedDocument);
        }

        /// <summary>
        /// Gets the document as byte array.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// The byte array.
        /// </returns>
        [CanBeNull]
        [UsedImplicitly]
        public byte[] GetDocumentAsByteArray(string filename)
        {
            return this.documents.TryGetValue(filename, out var document)
                ? ProtocolSerializer.Serialize(document.Descriptor)
                : null;
        }

        /// <summary>
        /// Gets a document from the session.
        /// </summary>
        /// <param name="filename">
        /// The filename of the document.
        /// </param>
        /// <returns>
        /// The document, of <c>null</c> if it can't be found.
        /// </returns>
        [CanBeNull]
        public IDocumentDescriptor GetDocument(string filename)
        {
            return this.documents.TryGetValue(filename, out var result) ? result : null;
        }

        /// <summary>
        /// Removes the document.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void RemoveDocument(string filename)
        {
            this.documents.Remove(filename);
        }

        /// <summary>
        /// Updates the document.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="contents">
        /// The contents.
        /// </param>
        /// <param name="documentVersion">
        /// The document version.
        /// </param>
        public void UpdateDocument(string filename, string contents, int documentVersion)
        {
            if (!this.documents.TryGetValue(filename, out var doc))
            {
                doc = this.documents[filename] = new Document(this, filename);
            }

            doc.Update(contents, documentVersion);
        }

        /// <summary>
        /// Updates a span in the document.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="endIndex">
        /// The end index.
        /// </param>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <param name="documentVersion">
        /// The new version of the document.
        /// </param>
        public void UpdateDocumentSpan(string filename, int startIndex, int endIndex, string span, int documentVersion)
        {
            if (!this.documents.TryGetValue(filename, out var doc))
            {
                return;
            }

            doc.Update(doc.Contents.Substring(0, startIndex) + span + doc.Contents.Substring(endIndex), documentVersion);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Sends the classification changed event.
        /// </summary>
        /// <param name="document">
        /// The serialized document.
        /// </param>
        internal void OnDocumentChanged(SerializableDocumentDescriptor document)
        {
            this.DocumentUpdated?.Invoke(this, new DocumentUpdatedEventArgs(document));
            this.InternalDocumentUpdated?.Invoke(this, ProtocolSerializer.Serialize(document));
        }
    }
}