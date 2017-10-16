using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectQl.Tools.Mef.Results.Controls
{
    using System.Windows.Controls;

    public class ScrollableTabControl : TabControl
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.ScrollViewer = (ScrollViewer)this.Template.FindName("PART_ScrollViewer", this);
        }

        public ScrollViewer ScrollViewer { get; private set; }
    }
}
