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

namespace ConnectQl.Tools.Mef.Helpers
{
    using System.Runtime.InteropServices;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.Imaging.Interop;
    using Microsoft.VisualStudio.PlatformUI;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// The image helpers.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// The get icon.
        /// </summary>
        /// <param name="moniker">
        /// The moniker.
        /// </param>
        /// <param name="backColor">
        /// The background color.
        /// </param>
        /// <returns>
        /// The <see cref="ImageSource"/>.
        /// </returns>
        [CanBeNull]
        public static ImageSource GetIcon(ImageMoniker moniker, Color backColor = default(Color))
        {
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis
            if (!(ServiceProvider.GlobalProvider.GetService(typeof(SVsImageService)) is IVsImageService2 iconService))
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
            {
                return null;
            }

            var imageAttributes = new ImageAttributes
            {
                Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags,
                ImageType = (uint)_UIImageType.IT_Bitmap,
                Format = (uint)_UIDataFormat.DF_WPF,
                LogicalHeight = 20,
                LogicalWidth = 20,
                StructSize = Marshal.SizeOf(typeof(ImageAttributes))
            };

            if (backColor != default(Color))
            {
                imageAttributes.Background = backColor.ToRgba();
                imageAttributes.Flags |= unchecked((uint)_ImageAttributesFlags.IAF_Background);
            }

            iconService.GetImage(moniker, imageAttributes).get_Data(out var data);

            var glyph = data as BitmapSource;

            glyph?.Freeze();

            return glyph;
        }
    }
}
