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

namespace ConnectQl.Tools.Mef.SignatureHelp
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;

    /// <summary>
    /// The ConnectQl signature help command handler.
    /// </summary>
    internal sealed class SignatureHelpCommandTarget : IOleCommandTarget
    {
        /// <summary>
        /// The next.
        /// </summary>
        private readonly IOleCommandTarget next;

        /// <summary>
        /// The listener.
        /// </summary>
        private readonly SignatureHelpViewCreationListener listener;

        /// <summary>
        /// The text view.
        /// </summary>
        private readonly ITextView textView;

        /// <summary>
        /// The signature help session.
        /// </summary>
        private ISignatureHelpSession signatureHelpSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureHelpCommandTarget"/> class.
        /// </summary>
        /// <param name="listener">
        /// The listener.
        /// </param>
        /// <param name="textView">
        /// The text view.
        /// </param>
        /// <param name="textViewAdapter">
        /// The text view adapter.
        /// </param>
        internal SignatureHelpCommandTarget(SignatureHelpViewCreationListener listener,  ITextView textView, IVsTextView textViewAdapter)
        {
            this.listener = listener;
            this.textView = textView;

            textViewAdapter.AddCommandFilter(this, out this.next);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <returns>
        /// This method returns S_OK on success. Other possible return values include the following.Return
        ///     codeDescriptionOLECMDERR_E_UNKNOWNGROUPThe <paramref name="pguidCmdGroup"/> parameter is not null but does not
        ///     specify a recognized command group.OLECMDERR_E_NOTSUPPORTEDThe <paramref name="nCmdID"/> parameter is not a valid
        ///     command in the group identified by <paramref name="pguidCmdGroup"/>.OLECMDERR_E_DISABLEDThe command identified by
        ///     <paramref name="nCmdID"/> is currently disabled and cannot be executed.OLECMDERR_E_NOHELPThe caller has asked for
        ///     help on the command identified by <paramref name="nCmdID"/>, but no help is available.OLECMDERR_E_CANCELEDThe user
        ///     canceled the execution of the command.
        /// </returns>
        /// <param name="pguidCmdGroup">
        /// The GUID of the command group.
        /// </param>
        /// <param name="nCmdID">
        /// The command ID.
        /// </param>
        /// <param name="nCmdexecopt">
        /// Specifies how the object should execute the command. Possible values are taken from the
        ///     <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDEXECOPT"/> and
        ///     <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDID_WINDOWSTATE_FLAG"/> enumerations.
        /// </param>
        /// <param name="pvaIn">
        /// The input arguments of the command.
        /// </param>
        /// <param name="pvaOut">
        /// The output arguments of the command.
        /// </param>
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            try
            {
                if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
                {
                    var typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    if (typedChar.Equals('('))
                    {
                        this.signatureHelpSession = this.listener.SignatureHelpBroker.TriggerSignatureHelp(this.textView);
                    }
                    else if (typedChar.Equals(')') && this.signatureHelpSession != null)
                    {
                        this.signatureHelpSession.Dismiss();
                        this.signatureHelpSession = null;
                    }
                }
            }
            catch (Exception)
            {
                // Ignore.
            }

            return this.next.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// Queries the object for the status of one or more commands generated by user interface events.
        /// </summary>
        /// <returns>
        /// This method returns S_OK on success. Other possible return values include the following.Return
        ///     codeDescriptionE_FAILThe operation failed.E_UNEXPECTEDAn unexpected error has occurred.E_POINTERThe
        ///     <paramref name="prgCmds"/> argument is null.OLECMDERR_E_UNKNOWNGROUPThe <paramref name="pguidCmdGroup"/>
        ///     parameter is not null but does not specify a recognized command group.
        /// </returns>
        /// <param name="pguidCmdGroup">
        /// The GUID of the command group.
        /// </param>
        /// <param name="cCmds">
        /// The number of commands in <paramref name="prgCmds"/>.
        /// </param>
        /// <param name="prgCmds">
        /// An array of <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMD"/> structures that indicate the commands for
        ///     which the caller needs status information. This method fills the <c>cmdf</c> member of each structure with values
        ///     taken from the <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDF"/> enumeration.
        /// </param>
        /// <param name="pCmdText">
        /// An <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT"/> structure in which to return name and/or status
        ///     information of a single command. This parameter can be null to indicate that the caller does not need this
        ///     information.
        /// </param>
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return this.next.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}