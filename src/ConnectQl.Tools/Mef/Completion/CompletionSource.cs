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

namespace ConnectQl.Tools.Mef.Completion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using Helpers;
    using Interfaces;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// The ConnectQl completion source.
    /// </summary>
    public class CompletionSource : ICompletionSource
    {
        /// <summary>
        /// The function.
        /// </summary>
        private static readonly ImageSource Function = ImageHelper.GetIcon(KnownMonikers.ScalarFunction);

        /// <summary>
        /// The aggregate.
        /// </summary>
        private static readonly ImageSource Aggregate = ImageHelper.GetIcon(KnownMonikers.AutoSum);

        /// <summary>
        /// The variable.
        /// </summary>
        private static readonly ImageSource Variable = ImageHelper.GetIcon(KnownMonikers.MemberVariable);

        /// <summary>
        /// The plugin.
        /// </summary>
        private static readonly ImageSource Plugin = ImageHelper.GetIcon(KnownMonikers.AddIn);

        /// <summary>
        /// The column.
        /// </summary>
        private static readonly ImageSource Column = ImageHelper.GetIcon(KnownMonikers.Field);

        /// <summary>
        /// The field.
        /// </summary>
        private static readonly ImageSource Source = ImageHelper.GetIcon(KnownMonikers.DataTable);

        /// <summary>
        /// The keyword.
        /// </summary>
        private static readonly ImageSource Keyword = ImageHelper.GetIcon(KnownMonikers.IntellisenseKeyword);

        /// <summary>
        /// The root completions.
        /// </summary>
        private static readonly string[] RootCompletions =
            {
                "IMPORT", "DECLARE", "SELECT", "INSERT", "UPSERT", "USE"
            };

        /// <summary>
        /// The provider.
        /// </summary>
        private readonly CompletionSourceProvider provider;

        /// <summary>
        /// The text buffer.
        /// </summary>
        private readonly ITextBuffer textBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompletionSource"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="textBuffer">
        /// The text buffer.
        /// </param>
        public CompletionSource(CompletionSourceProvider provider, ITextBuffer textBuffer)
        {
            this.provider = provider;
            this.textBuffer = textBuffer;
        }

        /// <summary>
        /// Determines which <see cref="T:Microsoft.VisualStudio.Language.Intellisense.CompletionSet"/>s should be part
        ///     of the specified <see cref="T:Microsoft.VisualStudio.Language.Intellisense.ICompletionSession"/>.
        /// </summary>
        /// <param name="session">
        /// The session for which completions are to be computed.
        /// </param>
        /// <param name="completionSets">
        /// The set of <see cref="T:Microsoft.VisualStudio.Language.Intellisense.CompletionSet"/>
        ///     objects to be added to the session.
        /// </param>
        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            var subjectTriggerPoint = session.GetTriggerPoint(this.textBuffer.CurrentSnapshot) - 1;

            if (!subjectTriggerPoint.HasValue)
            {
                return;
            }

            var guess = this.GuessCompletions(subjectTriggerPoint.Value);

            var span = guess.Item1;
            var completions = guess.Item2;

            completionSets.Add(
                new CompletionSet(
                    "ConnectQl",
                    "ConnectQl",
                    span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive),
                    completions.OrderBy(c => c.DisplayText),
                    new Completion[0]));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets the function completions.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <returns>
        /// The span to replace and the completions.
        /// </returns>
        private static IEnumerable<Completion> GetFunctionCompletions(IDocument document)
        {
            return
                document.GetAvailableFunctions().Select(f => new Completion($"{f.Name.ToUpperInvariant()} ( {string.Join(", ", f.Arguments.Select(a => a.Name))} )", f.Name.ToUpperInvariant(), f.Description, f.IsAggregateFunction ? CompletionSource.Aggregate : CompletionSource.Function, "function"));
        }

        /// <summary>
        /// The get keyword completions.
        /// </summary>
        /// <param name="spanToReplace">
        /// The span to replace.
        /// </param>
        /// <param name="keywords">
        /// The keywords.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private static Tuple<SnapshotSpan, IEnumerable<Completion>> GetKeywordCompletions(SnapshotSpan spanToReplace, params string[] keywords)
        {
            return Tuple.Create(
                spanToReplace,
                keywords.Select(k => new Completion(k, k + " ", null, CompletionSource.Keyword, "keyword")));
        }

        /// <summary>
        /// The get plugin completions.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private static IEnumerable<Completion> GetPluginCompletions(IDocument document)
        {
            return document.GetAvailablePlugins().Select(p => new Completion($"'{p}'", $"'{p}'", null, CompletionSource.Plugin, "plugin"));
        }

        /// <summary>
        /// The get variable completions.
        /// </summary>
        /// <param name="spanToReplace">
        /// The span to replace.
        /// </param>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private static IEnumerable<Completion> GetVariableCompletions(SnapshotSpan spanToReplace, IDocument document)
        {
            return document.GetAvailableVariables(spanToReplace.End).Select(p => new Completion(p.Name, p.Name, p.Value, CompletionSource.Variable, "variable"));
        }

        private static IEnumerable<Completion> GetSourceCompletions(SnapshotSpan spanToReplace, IDocument document)
        {
            return document.GetAvailableSources(spanToReplace.End).Select(p => new Completion(p.Alias, p.Alias, null, CompletionSource.Source, "source"));
        }

        /// <summary>
        /// Gets the column completions.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The span and the completions.</returns>
        private static IEnumerable<Completion> GetColumnCompletions(IDataSourceDescriptor source)
        {
            return
                source.Columns.SelectMany(c =>
                    new[]
                        {
                            new Completion($"{c.Name}", $"[{c.Name}]", c.Description, CompletionSource.Column, "column")
                        });
        }

        /// <summary>
        /// The guess completions.
        /// </summary>
        /// <param name="subjectTriggerPoint">
        /// The subject trigger point.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private Tuple<SnapshotSpan, IEnumerable<Completion>> GuessCompletions(SnapshotPoint subjectTriggerPoint)
        {
            var document = this.provider.DocumentProvider.GetDocument(this.textBuffer);
            var navigator = this.provider.NavigatorService.GetTextStructureNavigator(this.textBuffer);
            var extend = navigator.GetExtentOfWord(subjectTriggerPoint);
            var current = document.GetTokenAt(subjectTriggerPoint);
            var previous = document.GetTokenBefore(current) ?? current;

            if (subjectTriggerPoint.Position < subjectTriggerPoint.Snapshot.Length)
            {
                switch (subjectTriggerPoint.Snapshot.GetText(new Span(subjectTriggerPoint.Position, 1)))
                {
                    case " ":
                    case ".":
                        current = document.GetTokenAt(subjectTriggerPoint + 2);
                        break;
                }
            }

            var spanToReplace = extend.Span;

            var completions = document.GetAutoCompletions(current);

            var result = new List<Completion>();

            if (completions.Type.HasFlag(AutoCompleteType.Expression))
            {
                result.AddRange(CompletionSource.GetFunctionCompletions(document));
                result.AddRange(CompletionSource.GetSourceCompletions(spanToReplace, document));
                result.AddRange(CompletionSource.GetVariableCompletions(spanToReplace, document));
            }

            if (completions.Type.HasFlag(AutoCompleteType.Plugin))
            {
                result.AddRange(CompletionSource.GetPluginCompletions(document));
            }

            if (completions.Type.HasFlag(AutoCompleteType.Literal))
            {
                result.AddRange(completions.Literals.Select(l => new Completion(l, l + " ", null, CompletionSource.Keyword, "keyword")));
            }

            if (completions.Type.HasFlag(AutoCompleteType.Field))
            {
                var sourceName = document.GetTokenAt(subjectTriggerPoint).Value;
                var source = document.GetAvailableSources(extend.Span.Start - 1).FirstOrDefault(s => s.Alias.Equals(sourceName, StringComparison.OrdinalIgnoreCase));

                result.Add(new Completion("*", "*", "All fields", CompletionSource.Keyword, "wildcard"));

                if (source != null)
                {
                    result.AddRange(CompletionSource.GetColumnCompletions(source));
                }
            }

            return Tuple.Create(spanToReplace, result.AsEnumerable());
        }
    }
}