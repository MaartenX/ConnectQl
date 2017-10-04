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
    using System.ComponentModel.Composition;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The quick info controller provider.
    /// </summary>
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("ConnectQl QuickInfo Controller")]
    [ContentType("ConnectQl")]
    internal class QuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        /// <summary>
        /// Gets or sets the quick info broker.
        /// </summary>
        [Import]
        internal IQuickInfoBroker QuickInfoBroker { get; set; }

        /// <summary>
        /// Attempts to create an IntelliSense controller for a specific text view.
        /// </summary>
        /// <returns>
        /// A valid IntelliSense controller, or null if none could be created.
        /// </returns>
        /// <param name="textView">
        /// The text view for which a controller should be created.
        /// </param>
        /// <param name="subjectBuffers">
        /// The set of text buffers with matching content types that are potentially visible in the
        ///     view.
        /// </param>
        [NotNull]
        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new QuickInfoController(this, textView, subjectBuffers);
        }
    }
}