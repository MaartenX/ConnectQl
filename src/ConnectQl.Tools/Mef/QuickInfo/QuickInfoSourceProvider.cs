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

namespace ConnectQl.Tools.Mef.QuickInfo
{
    using System.ComponentModel.Composition;
    using Interfaces;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The quick info source provider.
    /// </summary>
    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("ConnectQl Quick Info Source")]
    [Order(Before = "Default Quick Info Presenter")]
    [ContentType("ConnectQl")]
    internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        /// <summary>
        /// Gets or sets the document provider.
        /// </summary>
        [Import]
        internal IDocumentProvider DocumentProvider { get; set; }

        /// <summary>
        /// Creates a Quick Info source for the specified context.
        /// </summary>
        /// <returns>
        /// A valid <see cref="T:Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSource"/>, or null if none could
        ///     be created.
        /// </returns>
        /// <param name="textBuffer">
        /// The text buffer for which to create a provider.
        /// </param>
        [NotNull]
        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new QuickInfoSource(this, textBuffer);
        }
    }
}