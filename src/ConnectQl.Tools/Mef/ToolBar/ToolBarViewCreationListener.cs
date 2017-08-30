using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectQl.Tools.Mef.ToolBar
{
    /// <summary>
    /// The completion source view creation listener.
    /// </summary>
    [Export(typeof(IVsTextViewCreationListener))]
    [Name("ConnectQl Toolbar handler")]
    [ContentType("ConnectQl")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class ToolBarViewCreationListener : IVsTextViewCreationListener
    {
        /// <summary>
        /// Gets or sets the adapter service.
        /// </summary>
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService { get; set; }
        
        /// <summary>
        /// Called when a <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextView"/> adapter has been created
        ///     and initialized.
        /// </summary>
        /// <param name="textViewAdapter">
        /// The newly created and initialized text view adapter.
        /// </param>
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = this.AdapterService.GetWpfTextView(textViewAdapter);

            textView?.Properties.GetOrCreateSingletonProperty(() => new ToolBarCommandTarget(textViewAdapter, textView, this));
        }
    }
}
