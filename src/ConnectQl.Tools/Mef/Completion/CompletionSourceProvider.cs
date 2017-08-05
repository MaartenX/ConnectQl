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

namespace ConnectQl.Tools.Mef.Completion
{
    using System.ComponentModel.Composition;
    using ConnectQl.Tools.Interfaces;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The ConnectQl completion source provider.
    /// </summary>
    [Export(typeof(ICompletionSourceProvider))]
    [Name("ConnectQl Completion Source provider")]
    [ContentType("ConnectQl")]
    [Order(Before = "High")]
    public class CompletionSourceProvider : ICompletionSourceProvider
    {
#pragma warning disable CS0169
        /// <summary>
        /// Gets or sets the document provider.
        /// </summary>
        [Import]
        internal IDocumentProvider DocumentProvider { get; set; }

        /// <summary>
        /// Gets or sets the navigator provider.
        /// </summary>
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

#pragma warning restore CS0169

        /// <summary>
        /// Creates a completion provider for the given context.
        /// </summary>
        /// <returns>
        /// A valid <see cref="T:Microsoft.VisualStudio.Language.Intellisense.ICompletionSource"/> instance, or null if none
        ///     could be created.
        /// </returns>
        /// <param name="textBuffer">
        /// The text buffer over which to create a provider.
        /// </param>
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new CompletionSource(this, textBuffer));
        }
    }
}