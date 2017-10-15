namespace ConnectQl.Tools.Mef.Results.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class DivideConverter : IValueConverter
    {
        public double By { get; set; }

        public Object Convert(Object value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return doubleValue / (this.By == 0 ? 1 : this.By);
            }

            return DependencyProperty.UnsetValue;
        }

        public Object ConvertBack(Object value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}