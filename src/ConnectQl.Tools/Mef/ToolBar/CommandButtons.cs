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
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using ConnectQl.Tools.Mef.Helpers;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.PlatformUI;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// The tool bar that contains the command buttons for an editor window.
    /// </summary>
    /// <seealso cref="Microsoft.VisualStudio.Text.Editor.IWpfTextViewMargin" />
    internal class CommandButtons : IWpfTextViewMargin
    {
        private IWpfTextView textView;
        private ITextDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandButtons"/> class.
        /// </summary>
        /// <param name="textView">The text view.</param>
        /// <param name="document">The document.</param>
        public CommandButtons(IWpfTextView textView, ITextDocument document)
        {
            this.textView = textView;
            this.document = document;

            var button = new Button
            {
                Content = new Image
                {
                    Source = ImageHelper.GetIcon(KnownMonikers.Run)
                }
            };

            var toolbar = new CustomToolBar
            {
                Height = 24,
                Items =
                {
                    button
                }
            };

            this.VisualElement =
                new ToolBarTray
                {
                    ToolBars = {
                        toolbar
                    }
                };

            toolbar.SetResourceReference(Control.BackgroundProperty, VsBrushes.CommandBarGradientKey);
        
            this.VisualElement.SetResourceReference(Control.BackgroundProperty, VsBrushes.CommandBarGradientKey);
            this.VisualElement.SetResourceReference(Control.BorderBrushProperty, EnvironmentColors.CommandBarToolBarBorderBrushKey);
        }

        /// <summary>
        /// Gets the <see cref="T:System.Windows.FrameworkElement" /> that renders the margin.
        /// </summary>
        public FrameworkElement VisualElement { get; }

        /// <summary>
        /// Gets the size of the margin.
        /// </summary>
        public double MarginSize => this.VisualElement.ActualHeight;

        /// <summary>
        /// Gets a value indicating whether the margin is enabled.
        /// </summary>
        public bool Enabled => true;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" /> with the specified margin name.
        /// </summary>
        /// <param name="marginName">The name of the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" />.</param>
        /// <returns>
        /// The <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" /> named <paramref name="marginName" />, or null if no match is found.
        /// </returns>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return this;
        }
    }

    public class CustomToolBar : ToolBar
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var button = (ToggleButton)this.GetTemplateChild("OverflowButton");

            button.Background = this.Background;
            button.SetResourceReference(ForegroundProperty, VsBrushes.DropDownGlyphKey);
        }
    }
}
