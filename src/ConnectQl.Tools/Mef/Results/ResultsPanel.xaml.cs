﻿// MIT License
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
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using ConnectQl.Results;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Classification;
    using System.Windows.Media;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Interaction logic for ResultsViewer.xaml
    /// </summary>
    internal partial class ResultsPanel : UserControl, IWpfTextViewMargin
    {
        /// <summary>
        /// The text view.
        /// </summary>
        private IWpfTextView textView;

        /// <summary>
        /// The document.
        /// </summary>
        private ITextDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsPanel"/> class.
        /// </summary>
        /// <param name="textView">The text view.</param>
        /// <param name="document">The document.</param>
        public ResultsPanel(IWpfTextView textView, ITextDocument document)
        {
            this.DataContext = this;

            this.InitializeComponent();

            if (textView.Properties.TryGetProperty<ResultsPanelScrollBar>(typeof(ResultsPanelScrollBar), out var scrollBar))
            {
                scrollBar.Panel = this;
            }

            this.textView = textView;
            this.document = document;

            this.BindPanelAsync();
        }

        /// <summary>
        /// Gets or sets the scroll bar.
        /// </summary>
        /// <value>
        /// The scroll bar.
        /// </value>
        public ResultsPanelScrollBar ScrollBar { get; set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public ObservableCollection<object> Items { get; } = new ObservableCollection<object>();


        public Brush EditorBackgroundBrush
        {
            get
            {

                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Windows.FrameworkElement" /> that renders the margin.
        /// </summary>
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
        /// Binds the result to this viewer.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        public async Task BindResultAsync(IExecuteResult result)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.Items.Clear();

                foreach (var qr in result.QueryResults)
                {
                    if (qr.Rows == null)
                    {
                        this.Items.Add(new AffectedRecordsViewModel(qr.AffectedRecords));
                    }
                    else
                    {
                        this.Items.Add(new RowsViewModel(qr.Rows));
                    }
                }
            });
        }

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
        ITextViewMargin ITextViewMargin.GetTextViewMargin(string marginName)
        {
            return this;
        }

        private void OnAutogeneratingColumn(object sender ,DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        /// <summary>
        /// Binds the panel asynchronously.
        /// </summary>
        private async void BindPanelAsync()
        {
            await this.BindResultAsync(await new ConnectQlContext().ExecuteAsync("SELECT INT(a.Item) as Blieb FROM SPLIT( '1,2,3', ',') a SELECT a.Item as a, a.item as b, a.item + ' Item' as c, 2 * a.item as double FROM SPLIT( '1,2,3,5,6,7,8,9,10', ',') a"));
        }
    }
}
