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

namespace ConnectQl.Tools.Mef.Errors
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;

    /// <summary>
    /// The updated error list provider.
    /// </summary>
    internal class UpdatedErrorListProvider : ErrorListProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatedErrorListProvider"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public UpdatedErrorListProvider(IServiceProvider provider)
            : base(provider)
        {
        }

        /// <summary>
        /// The navigate to task.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        /// <param name="logicalView">
        /// The logical view.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool NavigateToTask(Task task, Guid logicalView)
        {
            if (VsShellUtilities.ShellIsShuttingDown)
            {
                return false;
            }

            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (string.IsNullOrEmpty(task.Document))
            {
                return false;
            }

            var openDocument = this.GetService(typeof(IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;

            if (openDocument == null)
            {
                return false;
            }

            var rguidLogicalView = logicalView;

            if (openDocument.OpenDocumentViaProject(task.Document, ref rguidLogicalView, out var serviceProvider, out var hierarchy, out var itemId, out var windowFrame) < 0 || windowFrame == null)
            {
                return false;
            }

            windowFrame.GetProperty(-4004, out var textBufferObject);

            var textBuffer = textBufferObject as VsTextBuffer;

            if (textBuffer == null)
            {
                if (textBufferObject is IVsTextBufferProvider textBufferProvider)
                {
                    var hr = textBufferProvider.GetTextBuffer(out var lines);

                    if (hr < 0)
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }

                    textBuffer = lines as VsTextBuffer;

                    if (textBuffer == null)
                    {
                        return false;
                    }
                }
            }

            var textManager = this.GetService(typeof(VsTextManagerClass)) as IVsTextManager;

            if (textManager == null)
            {
                return false;
            }

            textManager.NavigateToLineAndColumn(textBuffer, ref logicalView, task.Line, task.Column, task.Line, task.Column);

            return true;
        }
    }
}