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

namespace ConnectQl.Tools.Mef.ToolBar
{
    using System;
    using System.IO;

    using ConnectQl.Tools.Interfaces;
    using ConnectQl.Tools.Mef.Results;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;

    /// <summary>
    /// The tool bar command target.
    /// </summary>
    internal class ToolBarCommandTarget : IOleCommandTarget
    {
        private IVsTextView textViewAdapter;
        private ITextView textView;
        private ToolBarViewCreationListener toolBarViewCreationListener;
        private IOleCommandTarget next;
        private bool isScriptRunning;

        private IVsUIShell shell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

        private IVsStatusbar statusbar = (IVsStatusbar)Package.GetGlobalService(typeof(SVsStatusbar));

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolBarCommandTarget"/> class.
        /// </summary>
        /// <param name="textViewAdapter">The text view adapter.</param>
        /// <param name="textView">The text view.</param>
        /// <param name="toolBarViewCreationListener">The tool bar view creation listener.</param>
        public ToolBarCommandTarget([NotNull] IVsTextView textViewAdapter, ITextView textView, ToolBarViewCreationListener toolBarViewCreationListener)
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
                            commands[i].cmdf = (this.isScriptRunning ? 0 : (uint)OLECMDF.OLECMDF_ENABLED) | (uint)OLECMDF.OLECMDF_SUPPORTED;
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
            if (commandGroup != Commands.ConnectQlCommandSet)
            {
                return this.next.Exec(ref commandGroup, commandId, commandExecutionOptions, inputArguments, outputArguments);
            }

            var document = this.toolBarViewCreationListener.DocumentProvider.GetDocument(this.textView.TextBuffer);

            if (this.textView.Properties.TryGetProperty<ResultsPanel>(typeof(ResultsPanel), out var panel))
            {
                this.ExecuteDocument(document, panel);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Executes the document.
        /// </summary>
        /// <param name="document">The document to execute.</param>
        /// <param name="resultsPanel">The results panel to send the result to.</param>
        private async void ExecuteDocument([NotNull] IDocument document, [NotNull] ResultsPanel resultsPanel)
        {
            this.statusbar.GetText(out var text);

            try
            {
                this.isScriptRunning = true;

                this.statusbar.SetText($"Running {Path.GetFileName(document.Filename)}...");

                resultsPanel.Result = await document.ExecuteAsync();
            }
            finally
            {
                this.isScriptRunning = false;

                this.statusbar.SetText(text);

                this.shell.UpdateCommandUI(VSConstants.S_FALSE);
            }
        }
    }
}