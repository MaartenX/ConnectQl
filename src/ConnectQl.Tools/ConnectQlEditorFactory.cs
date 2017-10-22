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
    using System;
    using System.Runtime.InteropServices;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;

    /// <summary>
    /// The editor factory.
    /// </summary>
    [Guid("C08E64C0-CBBF-4070-95B0-247E8B07EED7")]
    internal class ConnectQlEditorFactory : IVsEditorFactory
    {
        private readonly ConnectQlPackage package;
        private Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectQlEditorFactory"/> class.
        /// </summary>
        /// <param name="package">The package.</param>
        public ConnectQlEditorFactory(ConnectQlPackage package)
        {
            this.package = package;
        }

        /// <summary>
        /// Used by the editor factory architecture to create editors that support data/view separation.
        /// </summary>
        /// <param name="grfCreateDoc">[in] Flags whose values are taken from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__VSCREATEEDITORFLAGS" /> enumeration that defines the conditions under which to create the editor. Only open and silent flags are valid.</param>
        /// <param name="pszMkDocument">[in] String form of the moniker identifier of the document in the project system. In the case of documents that are files, this is always the path to the file. This parameter can also be used to specify documents that are not files. For example, in a database-oriented project, this parameter could contain a string that refers to records in a table.</param>
        /// <param name="pszPhysicalView">[in] Name of the physical view. See Remarks for details.</param>
        /// <param name="pvHier">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface.</param>
        /// <param name="itemid">[in] Item identifier of this editor instance.</param>
        /// <param name="punkDocDataExisting">[in] Must be the <paramref name="ppunkDocData" /> object that is registered in the Running Document Table (RDT). This parameter is used to determine if a document buffer (DocData object) has already been created. When an editor factory is asked to create a secondary view, then this parameter will be non-null indicating that there is no document buffer. If the file is open, return VS_E_INCOMPATIBLEDOCDATA and the environment will ask the user to close it.</param>
        /// <param name="ppunkDocView">[out] Pointer to the IUnknown interface for the DocView object. Returns null if an external editor exists, otherwise returns the view of the document.</param>
        /// <param name="ppunkDocData">[out] Pointer to the IUnknown interface for the DocData object. Returns the buffer for the document.</param>
        /// <param name="pbstrEditorCaption">[out] Initial caption defined by the document editor for the document window. This is typically a string enclosed in square brackets, such as "[Form]". This value is passed as an input parameter to the <see cref="M:Microsoft.VisualStudio.Shell.Interop.IVsUIShell.CreateDocumentWindow(System.UInt32,System.String,Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy,System.UInt32,System.IntPtr,System.IntPtr,System.Guid@,System.String,System.Guid@,Microsoft.VisualStudio.OLE.Interop.IServiceProvider,System.String,System.String,System.Int32[],Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame@)" /> method. If the file is [ReadOnly] the caption will be set during load of the file.</param>
        /// <param name="pguidCmdUI">[out] Returns the Command UI GUID. This GUID is active when this editor is activated. Any UI element that is visible in the editor has to use this GUID. This GUID is used in the .ctc file in the satellite DLL where it indicates which menus and toolbars should be displayed when the document is active.</param>
        /// <param name="pgrfCDW">[out, retval] enum of type <see cref="T:Microsoft.VisualStudio.Shell.Interop.__VSEDITORCREATEDOCWIN" />. These flags are passed to <see cref="M:Microsoft.VisualStudio.Shell.Interop.IVsUIShell.CreateDocumentWindow(System.UInt32,System.String,Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy,System.UInt32,System.IntPtr,System.IntPtr,System.Guid@,System.String,System.Guid@,Microsoft.VisualStudio.OLE.Interop.IServiceProvider,System.String,System.String,System.Int32[],Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame@)" />.</param>
        /// <returns>
        /// If the document has a format that cannot be opened in the editor, <see cref="F:Microsoft.VisualStudio.Shell.Interop.VSErrorCodes.VS_E_UNSUPPORTEDFORMAT" /> is returned.If the document is open in an incompatible editor (or <see cref="F:Microsoft.VisualStudio.VSConstants.E_NOINTERFACE" />), <see cref="F:Microsoft.VisualStudio.Shell.Interop.VSErrorCodes.VS_E_INCOMPATIBLEDOCDATA" /> is returned. If the file could not be opened for any other reason, another HRESULT error code is returned.
        /// </returns>
        public int CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, string pszPhysicalView, IVsHierarchy pvHier, uint itemid, IntPtr punkDocDataExisting, out IntPtr ppunkDocView, out IntPtr ppunkDocData, [CanBeNull] out string pbstrEditorCaption, out Guid pguidCmdUI, out int pgrfCDW)
        {
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pguidCmdUI = new Guid("467B0037-F474-459B-BE32-D61D382F78ED");
            pgrfCDW = 0;
            pbstrEditorCaption = null;

            if ((grfCreateDoc & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
            {
                return VSConstants.E_INVALIDARG;
            }

            if (punkDocDataExisting != IntPtr.Zero)
            {
                return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
            }

            var clsidTextBuffer = typeof(VsTextBufferClass).GUID;
            var iidTextBuffer = VSConstants.IID_IUnknown;
            object pTextBuffer = pTextBuffer = this.package.CreateInstance(
                  ref clsidTextBuffer,
                  ref iidTextBuffer,
                  typeof(object));

            if (pTextBuffer != null)
            {
                if (pTextBuffer is IObjectWithSite textBufferSite)
                {
                    textBufferSite.SetSite(this.serviceProvider);
                }

                var clsidCodeWindow = typeof(VsCodeWindowClass).GUID;
                var iidCodeWindow = typeof(IVsCodeWindow).GUID;
                var pCodeWindow = (IVsCodeWindow)this.package.CreateInstance(ref clsidCodeWindow, ref iidCodeWindow, typeof(IVsCodeWindow));

                if (pCodeWindow != null)
                {
                    pCodeWindow.SetBuffer((IVsTextLines)pTextBuffer);

                    ppunkDocView = Marshal.GetIUnknownForObject(pCodeWindow);
                    ppunkDocData = Marshal.GetIUnknownForObject(pTextBuffer);

                    pguidCmdUI = VSConstants.GUID_TextEditorFactory;

                    pbstrEditorCaption = " [MyPackage]";

                    return VSConstants.S_OK;
                }
            }

            return VSConstants.E_FAIL;
        }

        /// <summary>
        /// Initializes an editor in the environment.
        /// </summary>
        /// <param name="psp">[in] Pointer to the <see cref="T:System.IServiceProvider" /> interface which can be used by the factory to obtain other interfaces.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            this.serviceProvider = psp;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Releases all cached interface pointers and unregisters any event sinks.
        /// </summary>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int Close()
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Maps a logical view to a physical view.
        /// </summary>
        /// <param name="rguidLogicalView">[in] Unique identifier of the logical view.</param>
        /// <param name="pbstrPhysicalView">[out, retval] Pointer to the physical view to which the logical view is to be mapped.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int MapLogicalView(ref Guid rguidLogicalView, [CanBeNull] out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null;

            return (rguidLogicalView.Equals(VSConstants.LOGVIEWID_Designer) || rguidLogicalView.Equals(VSConstants.LOGVIEWID_Primary))
            ? VSConstants.S_OK
            : VSConstants.E_NOTIMPL;
        }
    }
}