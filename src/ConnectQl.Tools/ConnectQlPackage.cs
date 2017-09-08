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

[assembly: System.Resources.NeutralResourcesLanguage("en", System.Resources.UltimateResourceFallbackLocation.Satellite)]

namespace ConnectQl.Tools
{
    using System;
    using System.ComponentModel.Composition;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Media;
    using ConnectQl.Tools.Mef.Classification;
    using ConnectQl.Tools.Resources;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using static Microsoft.VisualStudio.VSConstants;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// The Visual Studio Package.
    /// </summary>
    [Guid(PackageId)]
    [ComVisible(true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [PackageRegistration(UseManagedResourcesOnly = false)]
    [InstalledProductRegistration("#110", "#112", "1.0.0.0", IconResourceID = 400)]
    [ProvideAutoLoad("1501ac94-e5fa-4e6b-b780-0959421d99a4")]
    [ProvideUIContextRule(
        "1501ac94-e5fa-4e6b-b780-0959421d99a4",
        name: "Auto load",
        expression: "FullyLoaded & (SingleProject | MultipleProjects)",
        termNames: new[] { "FullyLoaded", "SingleProject", "MultipleProjects" },
        termValues: new[] { UICONTEXT.SolutionExistsAndFullyLoaded_string, UICONTEXT.SolutionHasSingleProject_string, UICONTEXT.SolutionHasMultipleProjects_string },
        delay: 500)]
    public class ConnectQlPackage : AsyncPackage
    {
        /// <summary>
        /// The package id.
        /// </summary>
        public const string PackageId = "16dce247-ec5c-460f-b9e7-c0327a7799f1";

#pragma warning disable CS0169

        /// <summary>
        /// The function type.
        /// </summary>
        [Export]
        [Name(ClassifierNames.Function)]
        private static ClassificationTypeDefinition functionType;

        /// <summary>
        /// The ConnectQl content type definition.
        /// </summary>
        [Export]
        [Name("ConnectQl")]
        [BaseDefinition("text")]
        private static ContentTypeDefinition storageSqlContentTypeDefinition;

        /// <summary>
        /// The ConnectQl file extension definition.
        /// </summary>
        [Export]
        [FileExtension(".cql")]
        [ContentType("ConnectQl")]
        private static FileExtensionToContentTypeDefinition cqlFileExtensionDefinition;

        /// <summary>
        /// The ConnectQl file extension definition.
        /// </summary>
        [Export]
        [FileExtension(".connectql")]
        [ContentType("ConnectQl")]
        private static FileExtensionToContentTypeDefinition connectqlFileExtensionDefinition;

#pragma warning restore CS0169

        /// <summary>
        /// Initializes the package asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>
        /// The task that completes when initialization is done.
        /// </returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            var r = Resource.Get("112");

            await base.InitializeAsync(cancellationToken, progress);
        }

        /// <summary>
        /// The function format definition.
        /// </summary>
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = ClassifierNames.Function)]
        [UserVisible(true)]
        [Name(ClassifierNames.Function)]
        private sealed class FunctionFormatDefinition : ClassificationFormatDefinition
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FunctionFormatDefinition"/> class.
            /// </summary>
            public FunctionFormatDefinition()
            {
                this.ForegroundColor = Color.FromRgb(255, 0, 255);
                this.DisplayName = "StorageSQL Function";
            }
        }
    }
}
