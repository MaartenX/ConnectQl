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
    using ConnectQl.Interfaces;

    /// <summary>
    /// The intellisense session.
    /// </summary>
    public interface IIntellisenseSession : IDisposable
    {
        /// <summary>
        /// Occurs when a document is updated.
        /// </summary>
        event EventHandler<DocumentUpdatedEventArgs> DocumentUpdated;

        /// <summary>
        /// Updates the document span.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="span">The new text for the span.</param>
        void UpdateDocumentSpan(string filename, int startIndex, int endIndex, string span);

        /// <summary>
        /// Updates the document or creates a new one if it didn't exist yet.
        /// </summary>
        /// <param name="filename">The name of the document.</param>
        /// <param name="newContents">The updated contents of the document.</param>
        void UpdateDocument(string filename, string newContents);

        /// <summary>
        /// Removes the document.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        void RemoveDocument(string filename);

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="filename">
        /// The filename of the document.
        /// </param>
        /// <returns>
        /// The document, or <c>null</c> if it doesn't exist in the session.
        /// </returns>
        IDocumentDescriptor GetDocument(string filename);
    }
}