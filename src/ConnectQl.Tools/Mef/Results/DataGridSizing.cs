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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Contains attached properties for data grid sizing.
    /// </summary>
    public static class DataGridSizing
    {
        /// <summary>
        /// The initial width property.
        /// </summary>
        public static readonly DependencyProperty InitialWidthProperty = DependencyProperty.RegisterAttached(
            "InitialWidth",
            typeof(double),
            typeof(DataGridSizing),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, InitialWidthChanged));

        /// <summary>
        /// The initial width changed callback.
        /// </summary>
        private static readonly PropertyChangedCallback InitialWidthChanged = (o, e) =>
        {
            var columns = ((DataGrid)o).Columns;
            var total = columns.Count == 0 ? 0 : columns.Sum(c => c.ActualWidth);
            var newValue = (double)e.NewValue;

            foreach (var column in columns)
            {
                column.Width = total == 0 ? newValue / columns.Count : column.ActualWidth * newValue / total;
            }
        };

        /// <summary>
        /// Sets the initial width.
        /// </summary>
        /// <param name="dataGrid">The data grid.</param>
        /// <param name="value">The value.</param>
        public static void SetInitialWidth(DataGrid dataGrid, double value)
        {
            dataGrid.SetValue(InitialWidthProperty, value);
        }

        /// <summary>
        /// Gets the initial width.
        /// </summary>
        /// <param name="dataGrid">The data grid.</param>
        /// <returns>The value.</returns>
        public static double GetInitialWidth(DataGrid dataGrid)
        {
            return (double)dataGrid.GetValue(InitialWidthProperty);
        }
    }
}
