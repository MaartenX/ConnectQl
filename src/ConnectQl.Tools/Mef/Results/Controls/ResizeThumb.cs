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
    using System.Windows;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;

    /// <summary>
    /// A thumb that is used to resize list items.
    /// </summary>
    public class ResizeThumb : Thumb
    {
        /// <summary>
        /// The <see cref="DragOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DragOffsetProperty = DependencyProperty.Register("DragOffset", typeof(double), typeof(ResizeThumb), new PropertyMetadata(default(double)));

        /// <summary>
        /// The <see cref="InitialHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InitialHeightProperty = DependencyProperty.Register("InitialHeight", typeof(double), typeof(ResizeThumb), new PropertyMetadata(default(double), (o, args) => ((ResizeThumb)o).OnDragDelta(o, new DragDeltaEventArgs(0, 0))));

        /// <summary>
        /// The <see cref="MinValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(ResizeThumb), new PropertyMetadata(default(double), (o, args) => ((ResizeThumb)o).OnDragDelta(o, new DragDeltaEventArgs(0, 0))));

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeThumb"/> class.
        /// </summary>
        public ResizeThumb()
        {
            this.DragDelta += this.OnDragDelta;
        }

        /// <summary>
        /// Gets or sets the minimum value for the height of the item.
        /// </summary>
        [PublicAPI]
        public double MinValue
        {
            // ReSharper disable once PossibleNullReferenceException
            get => (double)this.GetValue(ResizeThumb.MinValueProperty);
            set => this.SetValue(ResizeThumb.MinValueProperty, value);
        }

        /// <summary>
        /// Gets or sets the initial height.
        /// </summary>
        [PublicAPI]
        public double InitialHeight
        {
            // ReSharper disable once PossibleNullReferenceException
            get => (double)this.GetValue(ResizeThumb.InitialHeightProperty);
            set => this.SetValue(ResizeThumb.InitialHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the drag offset.
        /// </summary>
        [PublicAPI]
        public double DragOffset
        {
            // ReSharper disable once PossibleNullReferenceException
            get => (double)this.GetValue(ResizeThumb.DragOffsetProperty);
            set => this.SetValue(ResizeThumb.DragOffsetProperty, value);
        }

        /// <summary>
        /// Called when the thumb is dragged.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event arguments.
        /// </param>
        private void OnDragDelta(object sender, [NotNull] DragDeltaEventArgs eventArgs)
        {
            this.DragOffset = Math.Max(this.DragOffset + eventArgs.VerticalChange, Math.Min(0, -this.InitialHeight + this.MinValue));
        }
    }
}