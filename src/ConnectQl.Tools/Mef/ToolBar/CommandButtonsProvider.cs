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

namespace ConnectQl.Tools.Mef.CommandButtons
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Provides the results panel.
    /// </summary>
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(nameof(CommandButtonsProvider))]
    [Order(Before = PredefinedMarginNames.Top)]
    [MarginContainer(PredefinedMarginNames.Top)]
    [ContentType("ConnectQl")]
    [TextViewRole(PredefinedTextViewRoles.Debuggable)] // This is to prevent the margin from loading in the diff view

    internal class CommandButtonsProvider : IWpfTextViewMarginProvider
    {
        /// <summary>
        /// Gets or sets the document factory service.
        /// </summary>
        [Import]
        internal ITextDocumentFactoryService DocumentFactoryService { get; set; }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.VisualStudio.Text.Editor.IWpfTextViewMargin" /> for the given <see cref="T:Microsoft.VisualStudio.Text.Editor.IWpfTextViewHost" />.
        /// </summary>
        /// <param name="wpfTextViewHost">The <see cref="T:Microsoft.VisualStudio.Text.Editor.IWpfTextViewHost" /> for which to create the <see cref="T:Microsoft.VisualStudio.Text.Editor.IWpfTextViewMargin" />.</param>
        /// <param name="marginContainer">The margin that will contain the newly-created margin.</param>
        /// <returns>
        /// The <see cref="T:Microsoft.VisualStudio.Text.Editor.IWpfTextViewMargin" />.
        /// </returns>
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            if (!this.DocumentFactoryService.TryGetTextDocument(wpfTextViewHost.TextView.TextDataModel.DocumentBuffer, out var document))
            {
                return null;
            }

            return wpfTextViewHost.TextView.Properties.GetOrCreateSingletonProperty(() => new CommandButtons(wpfTextViewHost.TextView, document));
        }
    }
}
