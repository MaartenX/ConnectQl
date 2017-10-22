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
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;

    /// <summary>
    /// The animated row.
    /// </summary>
    internal static class AnimatedRow
    {
        /// <summary>
        /// Dependency property for the animation multiplier.
        /// </summary>
        public static readonly DependencyProperty MultiplierProperty = DependencyProperty.RegisterAttached("Multiplier", typeof(double), typeof(AnimatedRow), new PropertyMetadata(1d));

        /// <summary>
        /// Dependency property for the animation height.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached(
            "Height",
            typeof(double),
            typeof(AnimatedRow),
            new PropertyMetadata(default(double), AnimatedRow.HeightChanged));

        /// <summary>
        /// Sets the animation multiplier.
        /// </summary>
        /// <param name="element">
        /// The row definition.
        /// </param>
        /// <param name="value">
        /// The new value of the multiplier.
        /// </param>
        [UsedImplicitly]
        public static void SetMultiplier([NotNull] RowDefinition element, double value)
        {
            element.SetValue(AnimatedRow.MultiplierProperty, value);
        }

        /// <summary>
        /// Gets the animation multiplier.
        /// </summary>
        /// <param name="element">
        /// The row definition.
        /// </param>
        /// <returns>The value of the multiplier.</returns>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Dependency property will never be null.")]
        public static double GetMultiplier([NotNull] RowDefinition element)
        {
            return (double)element.GetValue(AnimatedRow.MultiplierProperty);
        }

        /// <summary>
        /// Sets the height of the row definition.
        /// </summary>
        /// <param name="element">The row definition.</param>
        /// <param name="value">The new height.</param>
        [UsedImplicitly]
        public static void SetHeight([NotNull] RowDefinition element, double value)
        {
            element.SetValue(AnimatedRow.HeightProperty, value);
        }

        /// <summary>
        /// Gets the height of the row definition.
        /// </summary>
        /// <param name="element">The row definition.</param>
        /// <returns>
        /// The height.
        /// </returns>
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Dependency property will never be null.")]
        public static double GetHeight([NotNull] RowDefinition element)
        {
            return (double)element.GetValue(AnimatedRow.HeightProperty);
        }

        /// <summary>
        /// Called when the height changes. Sets the height of the row definition to the value of the <see cref="HeightProperty"/> times the <see cref="MultiplierProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private static void HeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (dependencyObject is RowDefinition rowDefinition)
            {
                rowDefinition.Height = new GridLength((double)eventArgs.NewValue * AnimatedRow.GetMultiplier(rowDefinition), GridUnitType.Pixel);
            }
        }
    }
}