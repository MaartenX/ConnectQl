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

namespace ConnectQl.Tools.Mef.Errors
{
    using System.ComponentModel.Composition;
    using ConnectQl.Tools.Interfaces;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The error tagger provider.
    /// </summary>
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("ConnectQl")]
    [TagType(typeof(ErrorTag))]
    internal sealed class ErrorTaggerProvider : IViewTaggerProvider
    {
        /// <summary>
        /// Gets or sets the document provider.
        /// </summary>
        [Import]
        internal IDocumentProvider DocumentProvider { get; set; }

        /// <summary>
        /// Gets or sets the error list provider.
        /// </summary>
        [Import]
        internal IErrorListProvider ErrorListProvider { get; set; }

        /// <summary>
        /// Creates a tag provider for the specified view and buffer.
        /// </summary>
        /// <returns>
        /// The <see cref="T:Microsoft.VisualStudio.Text.Tagging.ITagAggregator`1"/> of the correct type for
        ///     <paramref name="textView"/>.
        /// </returns>
        /// <param name="textView">
        /// The <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView"/>.
        /// </param>
        /// <param name="buffer">
        /// The <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer"/>.
        /// </param>
        /// <typeparam name="T">
        /// The type of the tag.
        /// </typeparam>
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer)
            where T : ITag
        {
            return new ErrorTagger(this, textView, buffer) as ITagger<T>;
        }
    }
}