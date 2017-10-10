namespace ConnectQl.Tools.Mef.Results
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    using JetBrains.Annotations;

    public class DataGridBehavior
    {
        public static DependencyProperty DisplayRowNumberProperty = DependencyProperty.RegisterAttached("RowNumber", typeof(bool), typeof(DataGridBehavior), new FrameworkPropertyMetadata(false, DataGridBehavior.RowNumberChanged));

        public static bool GetDisplayRowNumber(DependencyObject target)
        {
            return (bool)target.GetValue(DataGridBehavior.DisplayRowNumberProperty);
        }

        public static void SetDisplayRowNumber(DependencyObject target, bool value)
        {
            target.SetValue(DataGridBehavior.DisplayRowNumberProperty, value);
        }

        private static void RowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!(target is DataGrid dataGrid))
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

        [NotNull]
        private static List<T> GetVisualChildCollection<T>(object parent)
            where T : Visual
        {
            var visualCollection = new List<T>();

            DataGridBehavior.GetVisualChildCollection(parent as DependencyObject, visualCollection);

            return visualCollection;
        }

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