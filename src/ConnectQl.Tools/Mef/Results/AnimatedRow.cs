namespace ConnectQl.Tools.Mef.Results
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The animated row.
    /// </summary>
    public static class AnimatedRow
    {
        public static readonly DependencyProperty MultiplierProperty = DependencyProperty.RegisterAttached("Multiplier", typeof(double), typeof(AnimatedRow), new PropertyMetadata(1d));

        public static void SetMultiplier(DependencyObject element, double value)
        {
            element.SetValue(MultiplierProperty, value);
        }

        public static double GetMultiplier(DependencyObject element)
        {
            return (double)element.GetValue(MultiplierProperty);
        }

        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached(
            "Height",
            typeof(double),
            typeof(AnimatedRow),
            new PropertyMetadata(default(double), AnimatedRow.HeightChanged));

        private static void HeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RowDefinition)d).Height = new GridLength((double)e.NewValue * AnimatedRow.GetMultiplier(d), GridUnitType.Pixel);
        }

        public static void SetHeight(RowDefinition element, double value)
        {
            element.SetValue(AnimatedRow.HeightProperty, value);
        }

        public static double GetHeight(RowDefinition element)
        {
            return (double)element.GetValue(AnimatedRow.HeightProperty);
        }
    }
}