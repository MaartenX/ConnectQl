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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConnectQl.Intellisense;
    using ConnectQl.Tools.Interfaces;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;

    /// <summary>
    /// Classifier that classifies all text as an instance of the <see cref="Classifier"/> classification
    ///     type.
    /// </summary>
    internal class Classifier : IClassifier
    {
        /// <summary>
        /// The mapping.
        /// </summary>
        private static readonly Dictionary<Classification, string> Mapping =
            new Dictionary<Classification, string>
                {
                    {
                        Classification.Unknown, PredefinedClassificationTypeNames.Identifier
                    },
                    {
                        Classification.Comment, PredefinedClassificationTypeNames.Comment
                    },
                    {
                        Classification.String, PredefinedClassificationTypeNames.String
                    },
                    {
                        Classification.Identifier, PredefinedClassificationTypeNames.Identifier
                    },
                    {
                        Classification.Variable, PredefinedClassificationTypeNames.SymbolReference
                    },
                    {
                        Classification.Source, PredefinedClassificationTypeNames.SymbolDefinition
                    },
                    {
                        Classification.Number, PredefinedClassificationTypeNames.Number
                    },
                    {
                        Classification.Function, ClassifierNames.Function
                    },
                    {
                        Classification.Keyword, PredefinedClassificationTypeNames.Keyword
                    },
                    {
                        Classification.Operator, PredefinedClassificationTypeNames.Operator
                    },
                    {
                        Classification.Constant, PredefinedClassificationTypeNames.Literal
                    },
                    {
                        Classification.Parens, PredefinedClassificationTypeNames.Operator
                    }
                };

        /// <summary>
        /// The ConnectQl AST.
        /// </summary>
        private readonly IDocument document;

        /// <summary>
        /// The classification types.
        /// </summary>
        private readonly Dictionary<Classification, IClassificationType> classificationTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Classifier"/> class.
        /// </summary>
        /// <param name="buffer">
        ///     The text buffer.
        /// </param>
        /// <param name="document">
        ///     The document.
        /// </param>
        /// <param name="registry">
        ///     The registry.
        /// </param>
        internal Classifier(ITextBuffer buffer, IDocument document, IClassificationTypeRegistryService registry)
        {
            this.document = document;
            this.document.DocumentChanged += (o, e) =>
                {
                    if (e.Change.HasFlag(DocumentChangeType.Tokens))
                    {
                        this.ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length)));
                    }
                };

            this.classificationTypes = Mapping.ToDictionary(kv => kv.Key, kv => registry.GetClassificationType(kv.Value));
        }

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        ///     for example typing /* would cause the classification to change in C# without directly
        ///     affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        ///     In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">
        /// The span currently being classified.
        /// </param>
        /// <returns>
        /// A list of ClassificationSpans that represent spans identified to be of this classification.
        /// </returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            try
            {
                return this.document.GetClassifiedTokens(span)
                    .Select(t => new ClassificationSpan(new SnapshotSpan(span.Snapshot, Math.Min(t.Start, span.End.Position), Math.Min(t.Length, span.End.Position - t.Start)), this.classificationTypes[t.Classification]))
                    .ToArray();
            }
            catch
            {
                return new ClassificationSpan[0];
            }
        }
    }
}