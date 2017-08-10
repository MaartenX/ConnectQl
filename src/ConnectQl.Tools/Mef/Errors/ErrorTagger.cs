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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConnectQl.Tools.Interfaces;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// The error tagger.
    /// </summary>
    internal sealed class ErrorTagger : ITagger<ErrorTag>
    {
        /// <summary>
        /// The document.
        /// </summary>
        private readonly IDocument document;

        /// <summary>
        /// The view.
        /// </summary>
        private readonly ITextView view;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorTagger"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        internal ErrorTagger(
            ErrorTaggerProvider provider,
            ITextView view,
            ITextBuffer buffer)
        {
            this.view = view;
            this.document = provider.DocumentProvider.GetDocument(buffer);
            this.document.DocumentChanged += (o, e) =>
                {
                    if (e.Change.HasFlag(DocumentChangeType.Messages))
                    {
                        this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(buffer.CurrentSnapshot, new Span(0, buffer.CurrentSnapshot.Length))));
                    }
                };
        }

        /// <summary>
        /// The tags changed.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Gets all the tags that intersect the specified spans.
        /// </summary>
        /// <param name="spans">The spans to visit.</param>
        /// <returns>
        /// A <see cref="T:Microsoft.VisualStudio.Text.Tagging.TagSpan`1" /> for each tag.
        /// </returns>
        IEnumerable<ITagSpan<ErrorTag>> ITagger<ErrorTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var span in spans)
            {
                var messages = this.document.GetMessages(span);
                var tokens = this.document.GetClassifiedTokens(new SnapshotSpan(span.Snapshot, 0, span.Snapshot.Length)).ToArray();

                foreach (var message in messages)
                {
                    if (tokens.Length <= message.Start.TokenIndex || tokens.Length <= message.End.TokenIndex)
                    {
                        continue;
                    }

                    var start = Math.Min(tokens[message.Start.TokenIndex].Start, span.Snapshot.Length);
                    var end = Math.Min(tokens[message.End.TokenIndex].End, span.Snapshot.Length);

                    yield return new TagSpan<ErrorTag>(new SnapshotSpan(span.Snapshot, start, end - start), new ErrorTag("syntax error", message.Text));
                }
            }
        }
    }
}