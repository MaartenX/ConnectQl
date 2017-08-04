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

namespace ConnectQl.Tools.Extensions
{
    using System;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;

    /// <summary>
    /// The text extent extensions.
    /// </summary>
    public static class TextExtentExtensions
    {
        /// <summary>
        /// The include surrounding.
        /// </summary>
        /// <param name="extent">
        /// The extent.
        /// </param>
        /// <param name="surrounding">
        /// The surrounding.
        /// </param>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        /// <returns>
        /// The <see cref="TextExtent"/>.
        /// </returns>
        public static TextExtent IncludeSurrounding(this TextExtent extent, string surrounding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return extent.IncludeLeft(surrounding, comparison).IncludeRight(surrounding, comparison);
        }

        /// <summary>
        /// The include surrounding.
        /// </summary>
        /// <param name="extent">
        /// The extent.
        /// </param>
        /// <param name="surrounding">
        /// The surrounding.
        /// </param>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        /// <returns>
        /// The <see cref="TextExtent"/>.
        /// </returns>
        public static TextExtent IncludeLeft(this TextExtent extent, string surrounding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var span = extent.Span;
            var start = extent.Span.Start;
            var length = extent.Span.Length;

            if (span.Snapshot.GetText(span.Start - surrounding.Length, surrounding.Length).Equals(surrounding, comparison))
            {
                start -= surrounding.Length;
                length += surrounding.Length;
            }

            return new TextExtent(new SnapshotSpan(span.Snapshot, start, length), extent.IsSignificant);
        }

        /// <summary>
        /// The include surrounding.
        /// </summary>
        /// <param name="extent">
        /// The extent.
        /// </param>
        /// <param name="surrounding">
        /// The surrounding.
        /// </param>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        /// <returns>
        /// The <see cref="TextExtent"/>.
        /// </returns>
        public static TextExtent IncludeRight(this TextExtent extent, string surrounding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var span = extent.Span;
            var start = extent.Span.Start;
            var length = extent.Span.Length;

            if (span.Snapshot.GetText(span.End, surrounding.Length).Equals(surrounding, comparison))
            {
                length += surrounding.Length;
            }

            return new TextExtent(new SnapshotSpan(span.Snapshot, start, length), extent.IsSignificant);
        }
    }
}