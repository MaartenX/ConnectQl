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

namespace ConnectQl.Tools.Mef
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using Interfaces;
    using Intellisense;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using VSLangProj;

    /// <summary>
    /// The ConnectQl document provider.
    /// </summary>
    [Export(typeof(IDocumentProvider))]
    [ContentType("ConnectQl")]
    internal class ConnectQlDocumentProvider : IDocumentProvider
    {
        /// <summary>
        /// The DTE.
        /// </summary>
        private readonly DTE dte;

        /// <summary>
        /// The Visual Studio solution.
        /// </summary>
        private readonly IVsSolution vsSolution;

        /// <summary>
        /// The projects.
        /// </summary>
        private readonly Dictionary<string, IntellisenseSession> projects = new Dictionary<string, IntellisenseSession>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectQlDocumentProvider"/> class.
        /// </summary>
        public ConnectQlDocumentProvider()
        {
            this.dte = (DTE)Package.GetGlobalService(typeof(SDTE));
            this.vsSolution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
        }

        /// <summary>
        /// Gets or sets the document factory service.
        /// </summary>
        [Import]
        internal ITextDocumentFactoryService DocumentFactoryService { get; set; }

        /// <summary>
        /// Gets or sets the error list provider.
        /// </summary>
        /// <value>
        /// The error list provider.
        /// </value>
        [Import]
        internal IErrorListProvider ErrorListProvider { get; set; }

        /// <summary>
        /// The get document.
        /// </summary>
        /// <param name="textBuffer">
        /// The text buffer.
        /// </param>
        /// <returns>
        /// The <see cref="IDocument"/>.
        /// </returns>
        public IDocument GetDocument(ITextBuffer textBuffer)
        {
            this.DocumentFactoryService.TryGetTextDocument(textBuffer, out var document);

            var uniqueName = this.dte.Solution.FindProjectItem(document.FilePath)?.ContainingProject?.UniqueName ?? "Unknown project";

            this.vsSolution.GetProjectOfUniqueName(uniqueName, out var projectHierarchyItem);

            if (!this.projects.TryGetValue(uniqueName, out var session))
            {
                session = this.projects[uniqueName] = new IntellisenseSession(this, uniqueName, projectHierarchyItem, this.ErrorListProvider.ErrorList);
            }

            return session.GetDocument(textBuffer);
        }
    }
}