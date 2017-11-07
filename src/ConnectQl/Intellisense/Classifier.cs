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
    using System.Collections.Generic;
    using System.Linq;

    using ConnectQl.Parser;

    using JetBrains.Annotations;
    
    /// <summary>
    /// Changes classifications for the tokens.
    /// </summary>
    internal static class Classifier
    {
        /// <summary>
        /// Classifies the tokens.
        /// </summary>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        /// <returns>
        /// The classified tokens.
        /// </returns>
        [NotNull]
        public static IReadOnlyList<ConnectQlContext.ClassifiedToken> Classify([NotNull] IList<Token> tokens)
        {
            var result = new List<ConnectQlContext.ClassifiedToken>(tokens.Count);

            if (tokens.Count > 0)
            {
                foreach (var classifiedToken in Classifier.GetClassificiations(tokens))
                {
                    result.Add(new ConnectQlContext.ClassifiedToken(
                        classifiedToken.Token.CharPos,
                        classifiedToken.Token.CharPos + classifiedToken.Token.Val.Length,
                        classifiedToken.Classification,
                        classifiedToken.Token.Kind,
                        classifiedToken.Token.Val));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the classifications from an enumerable of tokens.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>The classified tokens.</returns>
        [ItemNotNull]
        private static IEnumerable<TokenInfo> GetClassificiations([NotNull] IEnumerable<Token> tokens)
        {
            Token current = null, last = null;

            foreach (var next in tokens.Concat(new Token[] { null }))
            {
                if (current == null)
                {
                    current = next;

                    continue;
                }

                yield return new TokenInfo(current, Classifier.ClassifyToken(last, current, next));

                last = current;
                current = next;
            }
        }

        /// <summary>
        /// The classify token.
        /// </summary>
        /// <param name="previous">
        /// The previous token.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="next">
        /// The next token.
        /// </param>
        /// <returns>
        /// The <see cref="Classification"/>.
        /// </returns>
        private static Classification ClassifyToken(Token previous, [NotNull] Token token, Token next)
        {
            switch (token.Kind)
            {
                case ConnectQlParser.Comment0Symbol:
                case ConnectQlParser.Comment1Symbol:
                case ConnectQlParser.Comment2Symbol:
                    return Classification.Comment;

                case ConnectQlParser.IdentifierSymbol:
                    if (previous?.Kind == ConnectQlParser.RightParenLiteral)
                    {
                        return Classification.Source;
                    }

                    switch (next?.Kind)
                    {
                        case ConnectQlParser.DotLiteral:
                            return Classification.SourceReference;
                        case ConnectQlParser.LeftParenLiteral:
                            return Classification.Function;
                        default:
                            if (previous?.Kind == ConnectQlParser.NumberSymbol)
                            {
                                return Classification.Keyword;
                            }
                            else
                            {
                                return Classification.Identifier;
                            }
                    }

                case ConnectQlParser.BracketedidentifierSymbol:
                    return Classification.Identifier;

                case ConnectQlParser.NumberSymbol:
                    return Classification.Number;

                case ConnectQlParser.StringSymbol:
                    return Classification.String;

                case ConnectQlParser.VariableSymbol:
                    return Classification.Variable;

                case ConnectQlParser.UseLiteral:
                case ConnectQlParser.DefaultLiteral:
                case ConnectQlParser.ForLiteral:
                case ConnectQlParser.DeclareLiteral:
                case ConnectQlParser.JobLiteral:
                case ConnectQlParser.TriggeredLiteral:
                case ConnectQlParser.EveryLiteral:
                case ConnectQlParser.AfterLiteral:
                case ConnectQlParser.ByLiteral:
                case ConnectQlParser.OrLiteral:
                case ConnectQlParser.BeginLiteral:
                case ConnectQlParser.EndLiteral:
                case ConnectQlParser.TriggerLiteral:
                case ConnectQlParser.ImportLiteral:
                case ConnectQlParser.PluginLiteral:
                case ConnectQlParser.InsertLiteral:
                case ConnectQlParser.UpsertLiteral:
                case ConnectQlParser.IntoLiteral:
                case ConnectQlParser.UnionLiteral:
                case ConnectQlParser.SelectLiteral:
                case ConnectQlParser.FromLiteral:
                case ConnectQlParser.WhereLiteral:
                case ConnectQlParser.GroupLiteral:
                case ConnectQlParser.HavingLiteral:
                case ConnectQlParser.OrderLiteral:
                case ConnectQlParser.AscLiteral:
                case ConnectQlParser.DescLiteral:
                case ConnectQlParser.JoinLiteral:
                case ConnectQlParser.InnerLiteral:
                case ConnectQlParser.LeftLiteral:
                case ConnectQlParser.OnLiteral:
                case ConnectQlParser.CrossLiteral:
                case ConnectQlParser.ApplyLiteral:
                case ConnectQlParser.OuterLiteral:
                case ConnectQlParser.SequentialLiteral:
                case ConnectQlParser.AsLiteral:
                case ConnectQlParser.AndLiteral:
                case ConnectQlParser.NotLiteral:
                    return Classification.Keyword;

                case ConnectQlParser.LeftParenLiteral:
                case ConnectQlParser.RightParenLiteral:
                    return Classification.Parens;

                case ConnectQlParser.EqualsLiteral:
                case ConnectQlParser.CommaLiteral:
                case ConnectQlParser.GreaterThanLiteral:
                case ConnectQlParser.GreaterThanEqualsLiteral:
                case ConnectQlParser.LessThanEqualsLiteral:
                case ConnectQlParser.LessThanLiteral:
                case ConnectQlParser.LessThanGreaterThanLiteral:
                case ConnectQlParser.PlusLiteral:
                case ConnectQlParser.MinusLiteral:
                case ConnectQlParser.MulLiteral:
                case ConnectQlParser.DivLiteral:
                case ConnectQlParser.ModLiteral:
                case ConnectQlParser.CircumflexLiteral:
                case ConnectQlParser.ExclamationMarkLiteral:
                    return Classification.Operator;

                case ConnectQlParser.TrueLiteral:
                case ConnectQlParser.FalseLiteral:
                case ConnectQlParser.NullLiteral:
                    return Classification.Number;

                case ConnectQlParser.DotLiteral:
                    return Classification.Identifier;
                case ConnectQlParser.UnknownIdentifierLiteral:
                    return Classification.Unknown;
            }

            return Classification.Unknown;
        }

        private class TokenInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TokenInfo"/> class.
            /// </summary>
            /// <param name="token">The token.</param>
            /// <param name="classification">The classification.</param>
            /// <param name="completions">The completions.</param>
            public TokenInfo(Token token, Classification classification, [CanBeNull] AutoCompletions completions = null)
            {
                this.Token = token;
                this.Classification = classification;
                this.AutoCompletions = completions;
            }

            public Token Token { get; }

            public Classification Classification { get; }

            public AutoCompletions AutoCompletions { get; }
        }
    }
}