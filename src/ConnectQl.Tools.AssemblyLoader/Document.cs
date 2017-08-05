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

namespace ConnectQl.Tools.AssemblyLoader
{
    using System;

    /// <summary>
    /// The document.
    /// </summary>
    public class Document : MarshalByRefObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        public Document(string filename, string content)
        {
            this.Filename = filename;
            this.Content = content;
        }

        /// <summary>
        /// The classification changed.
        /// </summary>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        /// <summary>
        /// Gets the content.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Updates the content in its entirety.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public void UpdateContent(string content)
        {
        }

        /// <summary>
        /// Updates the span.
        /// </summary>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="endIndex">
        /// The end index.
        /// </param>
        /// <param name="replacement">
        /// The replacement.
        /// </param>
        public void UpdateSpan(int startIndex, int endIndex, string replacement)
        {
        }

        /// <summary>
        /// The on classification changed event.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void OnClassificationChangedEvent(ClassificationChangedEventArgs eventArgs)
        {
            this.ClassificationChanged?.Invoke(this, eventArgs);
        }
    }
}