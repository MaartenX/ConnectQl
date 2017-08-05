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

namespace ConnectQl.Tools.Mef.SignatureHelp
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// The signature help source.
    /// </summary>
    internal class SignatureHelpSource : ISignatureHelpSource
    {
        /// <summary>
        /// The provider.
        /// </summary>
        private readonly SignatureHelpSourceProvider provider;

        /// <summary>
        /// The text buffer.
        /// </summary>
        private readonly ITextBuffer textBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureHelpSource"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="textBuffer">
        /// The text buffer.
        /// </param>
        public SignatureHelpSource(SignatureHelpSourceProvider provider, ITextBuffer textBuffer)
        {
            this.provider = provider;
            this.textBuffer = textBuffer;
        }

        /// <summary>
        /// The augment signature help session.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="signatures">
        /// The signatures.
        /// </param>
        public void AugmentSignatureHelpSession(ISignatureHelpSession session, IList<ISignature> signatures)
        {
            var point = session.GetTriggerPoint(this.textBuffer).GetPoint(this.textBuffer.CurrentSnapshot);
            var functionName = this.provider.NavigatorService.GetTextStructureNavigator(this.textBuffer).GetExtentOfWord(point - 1).Span.GetText();
            var applicableToSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(new Span(point, 0), SpanTrackingMode.EdgeInclusive, 0);

            foreach (var function in this.provider.DocumentProvider.GetDocument(this.textBuffer).GetFunctionsByName(functionName).ToArray())
            {
                signatures.Add(new Signature(this.textBuffer, function, applicableToSpan));
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// The get best match.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="ISignature"/>.
        /// </returns>
        public ISignature GetBestMatch(ISignatureHelpSession session)
        {
            return session.Signatures.FirstOrDefault();
        }
    }
}