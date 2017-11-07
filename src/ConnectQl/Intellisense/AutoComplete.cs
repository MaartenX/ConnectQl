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

namespace ConnectQl.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ConnectQl.Interfaces;

    using JetBrains.Annotations;
    using ConnectQl.Internal;

    /// <summary>
    /// Extensions for the <see cref="IDocumentDescriptor"/>.
    /// </summary>
    public static class AutoComplete
    {
        private static readonly AutoCompletions RootCompletions = new AutoCompletions(new[] { "IMPORT", "DECLARE", "SELECT", "INSERT", "UPSERT", "USE" });

        private static readonly AutoCompletions ImportCompletions = new AutoCompletions(new[] { "PLUGIN" });

        private static readonly AutoCompletions PluginCompletions = new AutoCompletions(AutoCompleteType.Plugin);

        private static readonly string[] TimeLiterals = { "MILLISECOND", "MILLISECONDS", "SECOND", "SECONDS", "MINUTE", "MINUTES", "HOUR", "HOURS", "DAY", "DAYS", "WEEK", "WEEKS" };

        private static readonly Dictionary<Tuple<TokenScope, int>, TokenScope> Transitions = new Dictionary<Tuple<TokenScope, int>, TokenScope>
        {
            { Tuple.Create<TokenScope, int>(TokenScope.Any, ConnectQlParser.SelectLiteral), TokenScope.Select },
            { Tuple.Create<TokenScope, int>(TokenScope.Any, ConnectQlParser.InsertLiteral), TokenScope.Insert },
            { Tuple.Create<TokenScope, int>(TokenScope.Any, ConnectQlParser.UseLiteral), TokenScope.Use },
            { Tuple.Create<TokenScope, int>(TokenScope.Any, ConnectQlParser.ImportLiteral), TokenScope.Import },
            { Tuple.Create<TokenScope, int>(TokenScope.Any, ConnectQlParser.DeclareLiteral), TokenScope.Declare },
            { Tuple.Create<TokenScope, int>(TokenScope.Insert, ConnectQlParser.SelectLiteral), TokenScope.Select },
            { Tuple.Create<TokenScope, int>(TokenScope.Select, ConnectQlParser.FromLiteral), TokenScope.From },
            { Tuple.Create<TokenScope, int>(TokenScope.From, ConnectQlParser.WhereLiteral), TokenScope.Where },
            { Tuple.Create<TokenScope, int>(TokenScope.Import, ConnectQlParser.StringSymbol), TokenScope.Root },
        };

        /// <summary>
        /// The token scope.
        /// </summary>
        private enum TokenScope
        {
            /// <summary>
            /// The root.
            /// </summary>
            Root,

            /// <summary>
            /// The select.
            /// </summary>
            Select,

            /// <summary>
            /// The from.
            /// </summary>
            From,

            /// <summary>
            /// The where.
            /// </summary>
            Where,

            /// <summary>
            /// The insert.
            /// </summary>
            Insert,

            /// <summary>
            /// The use.
            /// </summary>
            Use,

            /// <summary>
            /// The declare.
            /// </summary>
            Declare,

            /// <summary>
            /// The import.
            /// </summary>
            Import,

            /// <summary>
            /// The any.
            /// </summary>
            Any,
        }

        /// <summary>
        /// The open parentheses.
        /// </summary>
        private enum OpenParens
        {
            /// <summary>
            /// The none.
            /// </summary>
            None,

            /// <summary>
            /// The function.
            /// </summary>
            Function,

            /// <summary>
            /// The expression.
            /// </summary>
            Expression
        }

        /// <summary>
        /// Gets the completions.
        /// </summary>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <returns>
        /// The <see cref="AutoCompletions"/>.
        /// </returns>
        public static AutoCompletions GetCompletions([NotNull] IReadOnlyList<IClassifiedToken> tokens, IClassifiedToken current)
        {
            var scope = TokenScope.Root;
            var openParens = new Stack<OpenParens>();

            var i = 0;

            for (i = 0; i < tokens.Count && (tokens[i].Start != current.Start || tokens[i].End != current.End); i++)
            {
                if (tokens[i].Kind == ConnectQlParser.LeftParenLiteral)
                {
                    openParens.Push(tokens[i - 1].Classification == Classification.Function ? OpenParens.Function : OpenParens.Expression);
                }

                if (tokens[i].Kind == ConnectQlParser.RightParenLiteral && openParens.Count > 0)
                {
                    openParens.Pop();
                }

                if (AutoComplete.Transitions.TryGetValue(Tuple.Create(scope, tokens[i].Kind), out var newScope) || AutoComplete.Transitions.TryGetValue(Tuple.Create(TokenScope.Any, tokens[i].Kind), out newScope))
                {
                    scope = newScope;
                    openParens.Clear();
                }
            }

            if (i == tokens.Count)
            {
                return AutoComplete.RootCompletions;
            }

            var classifcation = tokens[i].Classification;
            var kind = tokens[i].Kind;
            var prevClass = tokens[i - 1].Classification;
            var prevKind = tokens[i - 1].Kind;

            switch (scope)
            {
                case TokenScope.Root:

                    return AutoComplete.RootCompletions;

                case TokenScope.Select:

                    if (prevClass == Classification.Operator ||
                        prevClass == Classification.Keyword)
                    {
                        return new AutoCompletions(AutoCompleteType.Expression | AutoCompleteType.SourceAlias);
                    }

                    if (prevKind == ConnectQlParser.DotLiteral)
                    {
                        return new AutoCompletions(AutoCompleteType.Field);
                    }

                    break;

                case TokenScope.From:

                    if (prevClass == Classification.Keyword)
                    {
                        return new AutoCompletions(AutoCompleteType.Source);
                    }

                    break;

                case TokenScope.Import:

                    if (prevKind == ConnectQlParser.ImportLiteral)
                    {
                        return new AutoCompletions(AutoCompleteType.Literal, "PLUGIN");
                    }

                    if (prevClass == Classification.Keyword)
                    {
                        return new AutoCompletions(AutoCompleteType.Plugin);
                    }

                    break;

                case TokenScope.Use:

                    if (prevKind == ConnectQlParser.UseLiteral)
                    {
                        return new AutoCompletions(AutoCompleteType.Literal, "DEFAULT");
                    }

                    break;

                case TokenScope.Insert:

                    if (prevKind == ConnectQlParser.InsertLiteral)
                    {
                        return new AutoCompletions(AutoCompleteType.Literal, "INTO");
                    }

                    if (prevClass == Classification.Keyword)
                    {
                        return new AutoCompletions(AutoCompleteType.Target);
                    }

                    break;
            }

            if (prevClass == Classification.Number ||
                prevClass == Classification.String ||
                prevClass == Classification.Constant ||
                prevClass == Classification.Variable)
            {
                return new AutoCompletions(
                    AutoCompleteType.Operator | AutoCompleteType.Literal,
                    prevClass == Classification.Number ? AutoComplete.TimeLiterals : null,
                    openParens.Any() ? new[] { ")" } : null,
                    openParens.LastOrDefault() == OpenParens.Function ? new[] { "," } : null);
            }

            return new AutoCompletions(AutoCompleteType.Literal, string.Empty);
        }
    }
}
