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
    using System.Runtime.InteropServices;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using ConnectQl.Interfaces;
    using ConnectQl.Tools.Extensions;
    using ConnectQl.Tools.Interfaces;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text;
    using ConnectQl.Tools.Mef.Helpers;

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
        /// <param name="spanToReplace">
        /// The span to replace.
        /// </param>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <returns>
        /// The span to replace and the completions.
        /// </returns>
        private static Tuple<SnapshotSpan, IEnumerable<Completion>> GetFunctionCompletions(SnapshotSpan spanToReplace, IDocument document)
        {
            return Tuple.Create(
                spanToReplace,
                document.GetAvailableFunctions().Select(f => new Completion($"{f.Name.ToUpperInvariant()} ( {string.Join(", ", f.Arguments.Select(a => a.Name))} )", f.Name.ToUpperInvariant(), f.Description, f.IsAggregateFunction ? Aggregate : Function, "function")));
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
                keywords.Select(k => new Completion(k, k + " ", null, Keyword, "keyword")));
        }

        /// <summary>
        /// The get plugin completions.
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
        private static Tuple<SnapshotSpan, IEnumerable<Completion>> GetPluginCompletions(SnapshotSpan spanToReplace, IDocument document)
        {
            return Tuple.Create(
                spanToReplace,
                document.GetAvailablePlugins().Select(p => new Completion($"'{p}'", $"'{p}'", null, Plugin, "plugin")));
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
        private static Tuple<SnapshotSpan, IEnumerable<Completion>> GetVariableCompletions(SnapshotSpan spanToReplace, IDocument document)
        {
            return Tuple.Create(
                spanToReplace,
                document.GetAvailableVariables(spanToReplace.End).Select(p => new Completion(p.Name, p.Name, p.Value, Variable, "variable")));
        }

        private static Tuple<SnapshotSpan, IEnumerable<Completion>> GetSourceCompletions(SnapshotSpan spanToReplace, IDocument document)
        {
            return Tuple.Create(
                spanToReplace,
                document.GetAvailableSources(spanToReplace.End).Select(p => new Completion(p.Alias, p.Alias, null, Source, "source")));
        }

        /// <summary>
        /// Gets the column completions.
        /// </summary>
        /// <param name="spanToReplace">The span to replace.</param>
        /// <param name="source">The source.</param>
        /// <returns>The span and the completions.</returns>
        private static Tuple<SnapshotSpan, IEnumerable<Completion>> GetColumnCompletions(SnapshotSpan spanToReplace, IDataSourceDescriptor source)
        {
            return Tuple.Create(
                spanToReplace,
                source.Columns.SelectMany(c =>
                    new[]
                        {
                            new Completion($"[{c.Name}]", $"[{c.Name}]", c.Description, Column, "column"),
                            new Completion($"{c.Name}", $"[{c.Name}]", c.Description, Column, "column")
                        }));
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

            var spanToReplace = extend.Span;

            if (current != null)
            {
                switch (current.Scope)
                {
                    case ClassificationScope.SelectExpression:

                        if (extend.Span.GetText() == ".")
                        {
                            var sourceName = navigator.GetExtentOfWord(extend.Span.Start - 1).Span.GetText();
                            var source = document.GetAvailableSources(extend.Span.Start - 1).FirstOrDefault(s => s.Alias.Equals(sourceName, StringComparison.OrdinalIgnoreCase));

                            if (source != null)
                            {
                                return GetColumnCompletions(new SnapshotSpan(extend.Span.Start + 1, 0), source);
                            }
                        }

                        return GetFunctionCompletions(spanToReplace, document)
                            .Concat(GetVariableCompletions(spanToReplace, document))
                            .Concat(GetSourceCompletions(spanToReplace, document));

                    case ClassificationScope.Expression:

                        return GetFunctionCompletions(spanToReplace, document)
                            .Concat(GetVariableCompletions(spanToReplace, document));

                    case ClassificationScope.Function:

                        return GetFunctionCompletions(spanToReplace, document);

                    case ClassificationScope.Import:

                        if (previous.Is("IMPORT"))
                        {
                            return GetKeywordCompletions(spanToReplace, "PLUGIN");
                        }

                        return GetPluginCompletions(extend.IncludeLeft("'").Span, document);

                    case ClassificationScope.Root:

                        switch (previous.Scope)
                        {
                            case ClassificationScope.Import:

                                if (previous.Is("PLUGIN"))
                                {
                                    return GetPluginCompletions(extend.IncludeLeft("'").Span, document);
                                }

                                break;

                            case ClassificationScope.Root:

                                if (previous.Is("DEFAULT"))
                                {
                                    return GetFunctionCompletions(spanToReplace, document);
                                }

                                break;
                        }

                        if (current.Is("USE") || previous.Is("USE") && !current.Is("DEFAULT"))
                        {
                            return GetKeywordCompletions(spanToReplace, "DEFAULT");
                        }

                        if (current.In("INSERT", "UPSERT") || (previous.In("INSERT", "UPSERT") && !current.Is("INTO")))
                        {
                            return GetKeywordCompletions(spanToReplace, "INTO");
                        }

                        return GetKeywordCompletions(spanToReplace, RootCompletions);
                }

                if (current.Start > subjectTriggerPoint)
                {
                    return GetKeywordCompletions(new SnapshotSpan(subjectTriggerPoint, 1), RootCompletions);
                }
            }

            return GetKeywordCompletions(spanToReplace);
        }
    }
}