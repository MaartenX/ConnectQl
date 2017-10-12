namespace ConnectQl.Tools.Mef.Results
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    public class VisibilityConverter : IValueConverter
    {
        public bool Inverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return (this.Inverted ? !boolValue : boolValue) ? Visibility.Visible : Visibility.Collapsed;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class DataGridHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[1] is int count && count == 1 && values[0] is double height)
            {
                return height;
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class DataGridMaxHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 && values[1] is int count && count > 1 && values[0] is double height)
            {
                var result = height / 2;

                if (values[2] is double change)
                {
                    result += change;
                }

                return result;
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ResizeThumb : Thumb
    {
        public static readonly DependencyProperty DragOffsetProperty = DependencyProperty.Register("DragOffset", typeof(double), typeof(ResizeThumb), new PropertyMetadata(default(double)));

        public ResizeThumb()
        {
            this.DragDelta += (o, e) => this.DragOffset += e.VerticalChange;
        }

        public double DragOffset
        {
            get
            {
                return (double)GetValue(DragOffsetProperty);
            }
            set
            {
                SetValue(DragOffsetProperty, value);
            }
        }
    }

}