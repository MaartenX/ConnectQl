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
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.PlatformUI;
    using Microsoft.VisualStudio.Text.Editor;

    /// <summary>
    /// Interaction logic for ResultsPanelScrollBar.xaml
    /// </summary>
    internal partial class ResultsPanelScrollBar : UserControl, IWpfTextViewMargin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectQl.Tools.Mef.Results.ResultsPanelScrollBar"/> class.
        /// </summary>
        /// <param name="textView">The text view.</param>
        public ResultsPanelScrollBar([NotNull] IWpfTextView textView)
        {
            this.InitializeComponent();

            this.VisualElement = this;

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
                this.ScrollBar.SetBinding(FrameworkElement.HeightProperty, new Binding(nameof(FrameworkElement.ActualHeight)) { Source = value, Mode = BindingMode.OneWay, Converter = new SubtractConverter { Amount = () => this.GridSplitter.ActualHeight } });
                this.ScrollBar.SetBinding(RangeBase.MaximumProperty, new Binding(nameof(ScrollViewer.ScrollableHeight)) { Source = value.ScrollViewer, Mode = BindingMode.OneWay });
                this.ScrollBar.SetBinding(ScrollBar.ViewportSizeProperty, new Binding(nameof(ScrollViewer.ViewportHeight)) { Source = value.ScrollViewer, Mode = BindingMode.OneWay });
                this.GridSplitter.SetBinding(UIElement.VisibilityProperty, new Binding(nameof(UIElement.Visibility)) { Source = value.Splitter, Mode = BindingMode.OneWay });
                this.ResultsButton.SetBinding(UIElement.IsEnabledProperty, new Binding(nameof(value.Result)) { Source = value, Mode = BindingMode.OneWay, Converter = new NotNullConverter() });
                this.TopRow.SetBinding(RowDefinition.HeightProperty, new Binding(nameof(value.PanelOffset)) { Source = value, Mode = BindingMode.OneWay, Converter = new DoubleToGridLengthConverter()});

                this.ResultsButton.AddHandler(
                    ButtonBase.ClickEvent,
                    (RoutedEventHandler)((o, e) => value.IsExpanded = !value.IsExpanded));

                this.ScrollBar.AddHandler(
                    ScrollBar.ScrollEvent,
                    (ScrollEventHandler)((o, e) => value.ScrollViewer.ScrollToVerticalOffset(e.NewValue)));

                value.ScrollViewer.AddHandler(
                    ScrollViewer.ScrollChangedEvent,
                    (ScrollChangedEventHandler)((o, e) => this.ScrollBar.Value = e.VerticalOffset));
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
        [NotNull]
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return this;
        }

        /// <summary>
        /// Subtracts an amount from a value.
        /// </summary>
        private class SubtractConverter : IValueConverter
        {
            /// <summary>
            /// Gets or sets a function that returns the amount to subtract.
            /// </summary>
            public Func<double> Amount { get; set; }

            /// <summary>Converts a value. </summary>
            /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
            /// <param name="value">The value produced by the binding source.</param>
            /// <param name="targetType">The type of the binding target property.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is double doubleValue)
                {
                    return Math.Max(doubleValue - (this.Amount?.Invoke() ?? 0), 0);
                }

                return DependencyProperty.UnsetValue;
            }

            /// <summary>Not supported.</summary>
            /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
            /// <param name="value">The value that is produced by the binding target.</param>
            /// <param name="targetType">The type to convert to.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            /// <exception cref="NotSupportedException">Always trown.</exception>
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a double to a grid length.
        /// </summary>
        private class DoubleToGridLengthConverter : IValueConverter
        {
            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            public GridUnitType Type { get; set; } = GridUnitType.Pixel;

            /// <summary>Converts a value. </summary>
            /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
            /// <param name="value">The value produced by the binding source.</param>
            /// <param name="targetType">The type of the binding target property.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is double doubleValue)
                {
                    return new GridLength(doubleValue, this.Type);
                }

                return DependencyProperty.UnsetValue;
            }

            /// <summary>Not supported.</summary>
            /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
            /// <param name="value">The value that is produced by the binding target.</param>
            /// <param name="targetType">The type to convert to.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            /// <exception cref="NotSupportedException">Always trown.</exception>
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
