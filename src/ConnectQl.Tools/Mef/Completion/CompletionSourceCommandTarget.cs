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

namespace ConnectQl.Tools.Mef.Completion
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using IServiceProvider = System.IServiceProvider;

    /// <summary>
    /// The completion source command target.
    /// </summary>
    internal class CompletionSourceCommandTarget : IOleCommandTarget
    {
        /// <summary>
        /// The listener.
        /// </summary>
        private readonly CompletionSourceViewCreationListener listener;

        /// <summary>
        /// The next.
        /// </summary>
        private readonly IOleCommandTarget next;

        /// <summary>
        /// The text view.
        /// </summary>
        private readonly ITextView textView;

        /// <summary>
        /// The session.
        /// </summary>
        private ICompletionSession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompletionSourceCommandTarget"/> class.
        /// </summary>
        /// <param name="textViewAdapter">
        /// The text view adapter.
        /// </param>
        /// <param name="textView">
        /// The text view.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        internal CompletionSourceCommandTarget(IVsTextView textViewAdapter, ITextView textView, CompletionSourceViewCreationListener provider)
        {
            this.textView = textView;
            this.listener = provider;

            // add the command to the command chain
            textViewAdapter.AddCommandFilter(this, out this.next);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <returns>
        /// This method returns S_OK on success. Other possible return values include the following.Return
        ///     codeDescriptionOLECMDERR_E_UNKNOWNGROUPThe <paramref name="commandGroup"/> parameter is not null but does not
        ///     specify a recognized command group.OLECMDERR_E_NOTSUPPORTEDThe <paramref name="commandId"/> parameter is not a
        ///     valid command in the group identified by <paramref name="commandGroup"/>.OLECMDERR_E_DISABLEDThe command
        ///     identified by <paramref name="commandId"/> is currently disabled and cannot be executed.OLECMDERR_E_NOHELPThe
        ///     caller has asked for help on the command identified by <paramref name="commandId"/>, but no help is
        ///     available.OLECMDERR_E_CANCELEDThe user canceled the execution of the command.
        /// </returns>
        /// <param name="commandGroup">
        /// The GUID of the command group.
        /// </param>
        /// <param name="commandId">
        /// The command ID.
        /// </param>
        /// <param name="commandExecutionOptions">
        /// Specifies how the object should execute the command. Possible values are taken
        ///     from the <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDEXECOPT"/> and
        ///     <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDID_WINDOWSTATE_FLAG"/> enumerations.
        /// </param>
        /// <param name="inputArguments">
        /// The input arguments of the command.
        /// </param>
        /// <param name="outputArguments">
        /// The output arguments of the command.
        /// </param>
        public int Exec(ref Guid commandGroup, uint commandId, uint commandExecutionOptions, IntPtr inputArguments, IntPtr outputArguments)
        {
            if (VsShellUtilities.IsInAutomationFunction((IServiceProvider)Package.GetGlobalService(typeof(IServiceProvider))))
            {
                return this.next.Exec(ref commandGroup, commandId, commandExecutionOptions, inputArguments, outputArguments);
            }

            var typedChar = char.MinValue;

            if (commandGroup == VsMenus.guidStandardCommandSet2K && (commandId == (uint)VSConstants.VSStd2KCmdID.COMPLETEWORD || commandId == (uint)VSConstants.VSStd2KCmdID.AUTOCOMPLETE))
            {
                if (this.session == null || this.session.IsDismissed)
                {
                    this.TriggerCompletion();
                }

                return VSConstants.S_OK;
            }

            if (commandGroup == VSConstants.VSStd2K && commandId == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(inputArguments);
            }

            if (commandId == (uint)VSConstants.VSStd2KCmdID.RETURN || commandId == (uint)VSConstants.VSStd2KCmdID.TAB || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar)))
            {
                if (this.session != null && !this.session.IsDismissed)
                {
                    if (this.session.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        this.session.Commit();

                        return char.IsPunctuation(typedChar)
                            ? this.next.Exec(ref commandGroup, commandId, commandExecutionOptions, inputArguments, outputArguments)
                            : VSConstants.S_OK;
                    }

                    this.session.Dismiss();
                }
            }

            var result = this.next.Exec(ref commandGroup, commandId, commandExecutionOptions, inputArguments, outputArguments);

            if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar) || typedChar == '@' || typedChar == '.' || typedChar == '\'')
            {
                if (this.session == null || this.session.IsDismissed)
                {
                    if (this.TriggerCompletion())
                    {
                        this.session?.Filter();
                    }
                }
                else
                {
                    this.session?.Filter();
                }

                return VSConstants.S_OK;
            }

            if (commandId == (uint)VSConstants.VSStd2KCmdID.BACKSPACE || commandId == (uint)VSConstants.VSStd2KCmdID.DELETE)
            {
                if (this.session != null && !this.session.IsDismissed)
                {
                    this.session.Filter();
                }

                return VSConstants.S_OK;
            }

            return result;
        }

        /// <summary>
        /// Queries the object for the status of one or more commands generated by user interface events.
        /// </summary>
        /// <returns>
        /// This method returns S_OK on success. Other possible return values include the following.Return
        ///     codeDescriptionE_FAILThe operation failed.E_UNEXPECTEDAn unexpected error has occurred.E_POINTERThe
        ///     <paramref name="commands"/> argument is null.OLECMDERR_E_UNKNOWNGROUPThe <paramref name="commandGroup"/>
        ///     parameter is not null but does not specify a recognized command group.
        /// </returns>
        /// <param name="commandGroup">
        /// The GUID of the command group.
        /// </param>
        /// <param name="commandCount">
        /// The number of commands in <paramref name="commands"/>.
        /// </param>
        /// <param name="commands">
        /// An array of <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMD"/> structures that indicate
        ///     the commands for which the caller needs status information. This method fills the <c>cmdf</c> member
        ///     of each structure with values taken from the <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDF"/>
        ///     enumeration.
        /// </param>
        /// <param name="commandText">
        /// An <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT"/> structure in which to
        ///     return name and/or status information of a single command. This parameter can be null to indicate that the caller
        ///     does not need this information.
        /// </param>
        public int QueryStatus(ref Guid commandGroup, uint commandCount, OLECMD[] commands, IntPtr commandText)
        {
            if (commandGroup == VsMenus.guidStandardCommandSet2K && (commands[0].cmdID == (uint)VSConstants.VSStd2KCmdID.COMPLETEWORD || commands[0].cmdID == (uint)VSConstants.VSStd2KCmdID.AUTOCOMPLETE))
            {
                commands[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED;
                commands[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;

                return VSConstants.S_OK;
            }

            return this.next.QueryStatus(ref commandGroup, commandCount, commands, commandText);
        }

        /// <summary>
        /// Called when the intellisense session is dismissed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnSessionDismissed(object sender, EventArgs e)
        {
            this.session.Dismissed -= this.OnSessionDismissed;
            this.session = null;
        }

        /// <summary>
        /// Triggers the completion session.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TriggerCompletion()
        {
            var caretPoint = this.textView.Caret.Position.Point.GetPoint(textBuffer => !textBuffer.ContentType.IsOfType("projection"), PositionAffinity.Predecessor);

            if (!caretPoint.HasValue)
            {
                return false;
            }

            this.session = this.listener.CompletionBroker.CreateCompletionSession(this.textView, caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive), true);
            this.session.Dismissed += this.OnSessionDismissed;
            this.session?.Start();

            return true;
        }
    }
}