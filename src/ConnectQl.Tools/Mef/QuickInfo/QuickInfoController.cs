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
    using System.Collections.Generic;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    /// <summary>
    /// The quick info controller.
    /// </summary>
    /// <seealso cref="Microsoft.VisualStudio.Language.Intellisense.IIntellisenseController" />
    internal class QuickInfoController : IIntellisenseController
    {
        /// <summary>
        /// The provider.
        /// </summary>
        private readonly QuickInfoControllerProvider provider;

        /// <summary>
        /// The subject buffers.
        /// </summary>
        private readonly IList<ITextBuffer> subjectBuffers;

        private IQuickInfoSession session;
        private ITextView textView;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickInfoController"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="textView">The text view.</param>
        /// <param name="subjectBuffers">The subject buffers.</param>
        public QuickInfoController(QuickInfoControllerProvider provider, ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            this.provider = provider;
            this.textView = textView;
            this.subjectBuffers = subjectBuffers;

            textView.MouseHover += this.TextViewOnMouseHover;
        }

        /// <summary>
        /// Detaches the specified view.
        /// </summary>
        /// <param name="viewToDetach">The view to detach.</param>
        public void Detach(ITextView viewToDetach)
        {
            if (this.textView == viewToDetach)
            {
                this.textView.MouseHover -= this.TextViewOnMouseHover;
                this.textView = null;
            }
        }

        /// <summary>
        /// Called when a new subject <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer" /> appears in the graph of buffers associated with the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />, due to a change in projection or content type.
        /// </summary>
        /// <param name="subjectBuffer">The newly-connected text buffer.</param>
        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        /// <summary>
        /// Called when a subject <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer" /> is removed from the graph of buffers associated with the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />, due to a change in projection or content type.
        /// </summary>
        /// <param name="subjectBuffer">The disconnected text buffer.</param>
        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        /// <summary>
        /// Gets called when the mouse hovers over the text view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="MouseHoverEventArgs"/> instance containing the event data.</param>
        private void TextViewOnMouseHover(object sender, MouseHoverEventArgs eventArgs)
        {
            var point = this.textView.BufferGraph.MapDownToFirstMatch(
                new SnapshotPoint(this.textView.TextSnapshot, eventArgs.Position),
                PointTrackingMode.Positive,
                snapshot => this.subjectBuffers.Contains(snapshot.TextBuffer),
                PositionAffinity.Predecessor);

            if (point == null)
            {
                return;
            }

            var triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);

            if (!this.provider.QuickInfoBroker.IsQuickInfoActive(this.textView))
            {
                this.session = this.provider.QuickInfoBroker.TriggerQuickInfo(this.textView, triggerPoint, true);
            }
        }
    }
}