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

namespace ConnectQl.Tools
{
    using System.ComponentModel.Composition;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Utilities;
    using static Microsoft.VisualStudio.VSConstants;

    /// <summary>
    /// The Visual Studio Package.
    /// </summary>
    [Guid(PackageId)]
    [ComVisible(true)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
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

    }
}
