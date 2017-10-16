namespace ConnectQl.Tools.Mef.Results.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    public class ResizeThumb : Thumb
    {
        public static readonly DependencyProperty DragOffsetProperty = DependencyProperty.Register("DragOffset", typeof(double), typeof(ResizeThumb), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty InitialHeightProperty = DependencyProperty.Register("InitialHeight", typeof(double), typeof(ResizeThumb), new PropertyMetadata(default(double), (o, args) => ((ResizeThumb)o).OnDragDelta(o, new DragDeltaEventArgs(0, 0))));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(ResizeThumb), new PropertyMetadata(default(double), (o, args) => ((ResizeThumb)o).OnDragDelta(o, new DragDeltaEventArgs(0, 0))));

        public double MinValue
        {
            get
            {
                return (double)this.GetValue(ResizeThumb.MinValueProperty);
            }
            set
            {
                this.SetValue(ResizeThumb.MinValueProperty, value);
            }
        }

        public double InitialHeight
        {
            get
            {
                return (double)this.GetValue(ResizeThumb.InitialHeightProperty);
            }
            set
            {
                this.SetValue(ResizeThumb.InitialHeightProperty, value);
            }
        }

        public ResizeThumb()
        {
            this.DragDelta += this.OnDragDelta;
        }

        private void OnDragDelta(object o, DragDeltaEventArgs e)
        {
            this.DragOffset = Math.Max(this.DragOffset + e.VerticalChange, Math.Min(0, -this.InitialHeight + this.MinValue));
        }

        public double DragOffset
        {
            get
            {
                return (double)this.GetValue(ResizeThumb.DragOffsetProperty);
            }
            set
            {
                this.SetValue(ResizeThumb.DragOffsetProperty, value);
            }
        }
    }
}