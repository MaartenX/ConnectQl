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

namespace ConnectQl.Tools.Mef.Results.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using ConnectQl.Results;
    using ConnectQl.Tools.Mef.Results.ViewModels;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Interaction logic for ResultsViewer.xaml
    /// </summary>
    internal partial class ResultsPanel : UserControl, IWpfTextViewMargin
    {
        /// <summary>
        /// The is expanded property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ResultsPanel), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The panel height.
        /// </summary>
        public static readonly DependencyProperty PanelHeightProperty = DependencyProperty.Register("PanelHeight", typeof(double), typeof(ResultsPanel), new PropertyMetadata(400d));

        /// <summary>
        /// The result.
        /// </summary>
        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof(IExecuteResult), typeof(ResultsPanel), new PropertyMetadata(default(IExecuteResult), ResultsPanel.ResultChanged));

        /// <summary>
        /// The offset relative to the parent panel.
        /// </summary>
        public static readonly DependencyProperty PanelOffsetProperty = DependencyProperty.Register("PanelOffset", typeof(double), typeof(ResultsPanel), new PropertyMetadata(default(double)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsPanel"/> class.
        /// </summary>
        /// <param name="textView">The text view.</param>
        public ResultsPanel([NotNull] IPropertyOwner textView)
        {
            this.DataContext = this;

            this.InitializeComponent();

            if (textView.Properties.TryGetProperty<ResultsPanelScrollBar>(typeof(ResultsPanelScrollBar), out var scrollBar))
            {
                scrollBar.Panel = this;
            }

            UIElement parent = null;

            this.LayoutUpdated += (o, e) => this.PanelOffset = this.TranslatePoint(new Point(0, 0), parent ?? (parent = (UIElement)VisualTreeHelper.GetParent(this))).Y;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the panel is expanded.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Dependency property will never be null.")]
        public bool IsExpanded
        {
            get => (bool)this.GetValue(ResultsPanel.IsExpandedProperty);
            set => this.SetValue(ResultsPanel.IsExpandedProperty, value);
        }

        /// <summary>
        /// Gets or sets the offset relative to the parent panel.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Dependency property will never be null.")]
        public double PanelOffset
        {
            get => (double)this.GetValue(ResultsPanel.PanelOffsetProperty);
            set => this.SetValue(ResultsPanel.PanelOffsetProperty, value);
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        [CanBeNull]
        public IExecuteResult Result
        {
            get => (IExecuteResult)this.GetValue(ResultsPanel.ResultProperty);
            set => this.SetValue(ResultsPanel.ResultProperty, value);
        }

        /// <summary>
        /// Gets or sets the panel height.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Dependency property will never be null.")]
        public double PanelHeight
        {
            get => (double)this.GetValue(ResultsPanel.PanelHeightProperty);
            set => this.SetValue(ResultsPanel.PanelHeightProperty, value);
        }
        
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public ObservableCollection<object> Results { get; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets the <see cref="T:System.Windows.FrameworkElement" /> that renders the margin.
        /// </summary>
        [NotNull]
        FrameworkElement IWpfTextViewMargin.VisualElement => this;
        

        /// <summary>
        /// Gets the size of the margin.
        /// </summary>
        double ITextViewMargin.MarginSize => 500;

        /// <summary>
        /// Gets a value indicating whether the margin is enabled.
        /// </summary>
        bool ITextViewMargin.Enabled => true;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
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
        ITextViewMargin ITextViewMargin.GetTextViewMargin(string marginName)
        {
            return this;
        }

        /// <summary>
        /// Called when the result for this panel is changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private static void ResultChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (dependencyObject is ResultsPanel panel)
            {
                panel.BindResultAsync(eventArgs.NewValue as IExecuteResult);
            }
        }

        /// <summary>
        /// Binds the result to this viewer.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private async void BindResultAsync(IExecuteResult result)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.Results.Clear();

                this.IsExpanded = result != null;

                if (result == null)
                {
                    return;
                }

                foreach (var qr in result.QueryResults)
                {
                    if (qr.Rows != null)
                    {
                        this.Results.Add(new RowsViewModel(qr));
                    }
                }
            });
        }

        /// <summary>
        /// Called when the grid splitter is dragged.
        /// </summary>
        /// <param name="sender">
        /// The splitter.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void GridSplitterDragCompleted(object sender, RoutedEventArgs e)
        {
            this.PanelHeight = ((Grid)((GridSplitter)sender).Parent).RowDefinitions[2].ActualHeight;
        }
    }
}
