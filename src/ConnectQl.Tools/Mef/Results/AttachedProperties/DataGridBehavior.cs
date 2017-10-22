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

namespace ConnectQl.Tools.Mef.Results.AttachedProperties
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    using JetBrains.Annotations;

    /// <summary>
    /// The data grid behavior.
    /// </summary>
    public class DataGridBehavior
    {
        /// <summary>
        /// The display row number property.
        /// </summary>
        public static readonly DependencyProperty DisplayRowNumberProperty = DependencyProperty.RegisterAttached("RowNumber", typeof(bool), typeof(DataGridBehavior), new FrameworkPropertyMetadata(false, DataGridBehavior.RowNumberChanged));

        /// <summary>
        /// Gets a value indicating whether to display row numbers.
        /// </summary>
        /// <param name="target">
        /// The target to get the number from.
        /// </param>
        /// <returns>
        /// <c>true</c> to display row numbers.
        /// </returns>
        public static bool GetDisplayRowNumber([NotNull] DataGrid target)
        {
            return (bool)target.GetValue(DataGridBehavior.DisplayRowNumberProperty);
        }

        /// <summary>
        /// Sets a value indicating whether to display row numbers.
        /// </summary>
        /// <param name="target">
        /// The target to get the number from.
        /// </param>
        /// <param name="value">
        /// <c>true</c> to display row numbers.
        /// </param>
        public static void SetDisplayRowNumber([NotNull] DataGrid target, bool value)
        {
            target.SetValue(DataGridBehavior.DisplayRowNumberProperty, value);
        }

        /// <summary>
        /// Called when the row number changed.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void RowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis
            if (!(target is DataGrid dataGrid))
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
            {
                return;
            }

            if (!(bool)e.NewValue)
            {
                return;
            }

            void LoadedRowHandler(object sender, DataGridRowEventArgs ea)
            {
                if (DataGridBehavior.GetDisplayRowNumber(dataGrid) == false)
                {
                    dataGrid.LoadingRow -= LoadedRowHandler;

                    return;
                }

                ea.Row.Header = ea.Row.GetIndex();
            }

            dataGrid.LoadingRow += LoadedRowHandler;

            void ItemsChangedHandler(object sender, ItemsChangedEventArgs ea)
            {
                if (DataGridBehavior.GetDisplayRowNumber(dataGrid) == false)
                {
                    dataGrid.ItemContainerGenerator.ItemsChanged -= ItemsChangedHandler;

                    return;
                }

                DataGridBehavior.GetVisualChildCollection<DataGridRow>(dataGrid).ForEach(d => d.Header = new TextBlock { Text = d.GetIndex().ToString(), HorizontalAlignment = HorizontalAlignment.Right });
            }

            dataGrid.ItemContainerGenerator.ItemsChanged += ItemsChangedHandler;
        }

        /// <summary>
        /// Gets the visual child collection.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items to get.
        /// </typeparam>
        /// <returns>
        /// The <see cref="List{T}"/> of items.
        /// </returns>
        [NotNull]
        private static List<T> GetVisualChildCollection<T>(object parent)
            where T : Visual
        {
            var visualCollection = new List<T>();

            DataGridBehavior.GetVisualChildCollection(parent as DependencyObject, visualCollection);

            return visualCollection;
        }

        /// <summary>
        /// Gets the visual children.
        /// </summary>
        /// <typeparam name="T">The type of the visuals.</typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="visualCollection">The collection to fill.</param>
        private static void GetVisualChildCollection<T>([NotNull] DependencyObject parent, ICollection<T> visualCollection)
            where T : Visual
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                {
                    visualCollection.Add(child as T);
                }

                DataGridBehavior.GetVisualChildCollection(child, visualCollection);
            }
        }
    }
}