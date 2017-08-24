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

namespace ConnectQl.Tools.Mef.Results
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.PlatformUI;

    /// <summary>
    /// Scroll bar for the results panel.
    /// </summary>
    internal class ResultsPanelScrollBar : IWpfTextViewMargin
    {
        private readonly ScrollBar scrollbar = new ScrollBar
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Visibility = Visibility.Visible
        };

        private readonly IWpfTextView textView;
        private readonly ITextDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsPanelScrollBar"/> class.
        /// </summary>
        /// <param name="textView">The text view.</param>
        /// <param name="document">The document.</param>
        public ResultsPanelScrollBar(IWpfTextView textView, ITextDocument document)
        {
            this.textView = textView;
            this.document = document;

            Grid.SetRow(this.scrollbar, 1);

            var border = new Border();

            border.SetResourceReference(Border.BackgroundProperty, VsBrushes.CommandShelfBackgroundGradientKey);

            this.VisualElement = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(5, GridUnitType.Pixel)},
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto)}
                },
                Children =
                {
                    border,
                    this.scrollbar
                }
            };

            if (textView.Properties.TryGetProperty<ResultsPanel>(typeof(ResultsPanel), out var panel))
            {
                this.Panel = panel;
            }
        }

        /// <summary>
        /// Sets the panel.
        /// </summary>
        /// <value>
        /// The panel.
        /// </value>
        public ResultsPanel Panel
        {
            set
            {
                this.scrollbar.SetBinding(FrameworkElement.HeightProperty, new Binding(nameof(Control.ActualHeight)) { Source = value, Mode = BindingMode.OneWay });
                this.scrollbar.SetBinding(RangeBase.MaximumProperty, new Binding(nameof(ScrollViewer.ScrollableHeight)) { Source = value.ScrollViewer, Mode = BindingMode.OneWay });
                this.scrollbar.SetBinding(ScrollBar.ViewportSizeProperty, new Binding(nameof(ScrollViewer.ViewportHeight)) { Source = value.ScrollViewer, Mode = BindingMode.OneWay });

                this.scrollbar.AddHandler(
                    ScrollBar.ScrollEvent,
                    (ScrollEventHandler)((o, e) => value.ScrollViewer.ScrollToVerticalOffset(e.NewValue)));

                value.ScrollViewer.AddHandler(
                    ScrollViewer.ScrollChangedEvent,
                    (ScrollChangedEventHandler)((o, e) => this.scrollbar.Value = e.VerticalOffset));
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Windows.FrameworkElement" /> that renders the margin.
        /// </summary>
        public FrameworkElement VisualElement { get; }

        /// <summary>
        /// Gets the size of the margin.
        /// </summary>
        public double MarginSize => 500;

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
}