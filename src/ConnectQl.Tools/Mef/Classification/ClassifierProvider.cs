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

namespace ConnectQl.Tools.Mef.Classification
{
    using System.ComponentModel.Composition;
    using Interfaces;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Classifier provider for ConnectQl.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("ConnectQl")]
    internal class ClassifierProvider : IClassifierProvider
    {
#pragma warning disable CS0649

        /// <summary>
        /// Classification registry to be used for getting a reference to the custom classification type later.
        /// </summary>
        [Import]
        private IClassificationTypeRegistryService classificationRegistry;

        /// <summary>
        /// The AST provider.
        /// </summary>
        [Import]
        private IDocumentProvider documentProvider;

#pragma warning restore CS0649

        /// <summary>
        /// Gets a classifier for the given text buffer.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="ITextBuffer"/> to classify.
        /// </param>
        /// <returns>
        /// A classifier for the text buffer, or null if the provider cannot do so in its current state.
        /// </returns>
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new Classifier(buffer, this.documentProvider.GetDocument(buffer), this.classificationRegistry));
        }
    }
}