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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// The quick info source.
    /// </summary>
    internal class QuickInfoSource : IQuickInfoSource
    {
        /// <summary>
        /// The provider.
        /// </summary>
        private readonly QuickInfoSourceProvider provider;

        /// <summary>
        /// The text buffer.
        /// </summary>
        private readonly ITextBuffer textBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickInfoSource"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="textBuffer">
        /// The text buffer.
        /// </param>
        public QuickInfoSource(QuickInfoSourceProvider provider, ITextBuffer textBuffer)
        {
            this.provider = provider;
            this.textBuffer = textBuffer;
        }

        /// <summary>
        /// Determines which pieces of Quick Info content should be part of the specified
        ///     <see cref="T:Microsoft.VisualStudio.Language.Intellisense.IQuickInfoSession"/>.
        /// </summary>
        /// <param name="session">
        /// The session for which completions are to be computed.
        /// </param>
        /// <param name="quickInfoContent">
        /// The QuickInfo content to be added to the session.
        /// </param>
        /// <param name="applicableToSpan">
        /// The <see cref="T:Microsoft.VisualStudio.Text.ITrackingSpan"/> to which this session
        ///     applies.
        /// </param>
        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            var subjectTriggerPoint = session.GetTriggerPoint(this.textBuffer.CurrentSnapshot);

            applicableToSpan = null;

            if (!subjectTriggerPoint.HasValue)
            {
                return;
            }

            var document = this.provider.DocumentProvider.GetDocument(this.textBuffer);
            var token = document.GetTokenAt(subjectTriggerPoint.Value);

            if (token == null)
            {
                return;
            }

            string description;

            switch (token.Classification)
            {
                case Classification.Function:
                    description = document.GetFunctionsByName(token.Value).FirstOrDefault()?.Description;

                    if (description != null)
                    {
                        applicableToSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(token.Start, token.End - token.Start, SpanTrackingMode.EdgeInclusive);

                        quickInfoContent.Add(description);
                    }

                    break;

                case Classification.Variable:
                    var variable = document.GetAvailableVariables(subjectTriggerPoint.Value).FirstOrDefault(v => v.Name.Equals(token.Value, StringComparison.OrdinalIgnoreCase));

                    description = variable == null ? null : variable.WasEvaluated ? variable.Value : "<Expression has side effects and cannot be evaluated>";

                    if (description != null)
                    {
                        applicableToSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(token.Start, token.End - token.Start, SpanTrackingMode.EdgeInclusive);

                        quickInfoContent.Add(description);
                    }

                    break;

                case Classification.Source:
                    var source = document.GetAvailableSources(subjectTriggerPoint.Value).FirstOrDefault(v => v.Alias.Equals(token.Value, StringComparison.OrdinalIgnoreCase));

                    description = source == null ? null : $"{source.Alias}, columns:\n{string.Join(",\n", source.Columns.Select(c => $"{c.Name}: {c.Type}"))}";

                    if (description != null)
                    {
                        applicableToSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(token.Start, token.End - token.Start, SpanTrackingMode.EdgeInclusive);

                        quickInfoContent.Add(description);
                    }

                    break;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }
    }
}