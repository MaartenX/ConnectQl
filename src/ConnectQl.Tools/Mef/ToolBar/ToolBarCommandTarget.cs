using System;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;

namespace ConnectQl.Tools.Mef.ToolBar
{
    internal class ToolBarCommandTarget : IOleCommandTarget
    {
        private IVsTextView textViewAdapter;
        private ITextView textView;
        private ToolBarViewCreationListener toolBarViewCreationListener;
        private IOleCommandTarget next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolBarCommandTarget"/> class.
        /// </summary>
        /// <param name="textViewAdapter">The text view adapter.</param>
        /// <param name="textView">The text view.</param>
        /// <param name="toolBarViewCreationListener">The tool bar view creation listener.</param>
        public ToolBarCommandTarget(IVsTextView textViewAdapter, ITextView textView, ToolBarViewCreationListener toolBarViewCreationListener)
        {
            this.textViewAdapter = textViewAdapter;
            this.textView = textView;
            this.toolBarViewCreationListener = toolBarViewCreationListener;

            // add the command to the command chain
            textViewAdapter.AddCommandFilter(this, out this.next);
        }

        /// <summary>
        /// Queries the status.
        /// </summary>
        /// <param name="commandGroup">The command group.</param>
        /// <param name="commandCount">The command count.</param>
        /// <param name="commands">The commands.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>The status code.</returns>
        public int QueryStatus(ref Guid commandGroup, uint commandCount, OLECMD[] commands, IntPtr commandText)
        {
            if (commandGroup == Commands.ConnectQlCommandSet)
            {
                for (var i = 0; i < commandCount; i++)
                {
                    switch (commands[i].cmdID)
                    {
                        case Commands.ConnectQlToolBarGroupId:
                            commands[i].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                            return VSConstants.S_OK;
                        case Commands.RunScriptCommandId:
                            commands[i].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                            return VSConstants.S_OK;
                    }
                }
            }

            return this.next.QueryStatus(ref commandGroup, commandCount, commands, commandText);
        }

        /// <summary>
        /// Executes the specified command group.
        /// </summary>
        /// <param name="commandGroup">The command group.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandExecutionOptions">The command execution options.</param>
        /// <param name="inputArguments">The input arguments.</param>
        /// <param name="outputArguments">The output arguments.</param>
        /// <returns>The status code.</returns>
        public int Exec(ref Guid commandGroup, uint commandId, uint commandExecutionOptions, IntPtr inputArguments, IntPtr outputArguments)
        {
            if (commandGroup == Commands.ConnectQlCommandSet)
            {   

            }

            return this.next.Exec(ref commandGroup, commandId, commandExecutionOptions, inputArguments, outputArguments);
        }
    }
}