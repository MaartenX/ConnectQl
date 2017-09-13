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

namespace ConnectQl.Internal.Intellisense
{
    using System.Collections.Generic;
    using System.Linq;

    using ConnectQl.Intellisense;

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
        public static IReadOnlyList<ConnectQlContext.ClassifiedToken> Classify(IList<Token> tokens)
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

        private class TokenInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TokenInfo"/> class.
            /// </summary>
            /// <param name="token">The token.</param>
            /// <param name="classification">The classification.</param>
            /// <param name="completions">The completions.</param>
            public TokenInfo(Token token, Classification classification, AutoCompletions completions = null)
            {
                this.Token = token;
                this.Classification = classification;
                this.AutoCompletions = completions;
            }

            public Token Token { get; }

            public Classification Classification { get; }

            public AutoCompletions AutoCompletions { get; }
        }

        /// <summary>
        /// Gets the classifications from an enumerable of tokens.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>The classified tokens.</returns>
        private static IEnumerable<TokenInfo> GetClassificiations(IEnumerable<Token> tokens)
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
        private static Classification ClassifyToken(Token previous, Token token, Token next)
        {
            switch (token.Kind)
            {
                case Parser.Comment0Symbol:
                case Parser.Comment1Symbol:
                case Parser.Comment2Symbol:
                    return Classification.Comment;

                case Parser.IdentifierSymbol:
                    if (previous?.Kind == Parser.RightParenLiteral)
                    {
                        return Classification.Source;
                    }

                    switch (next?.Kind)
                    {
                        case Parser.DotLiteral:
                            return Classification.SourceReference;
                        case Parser.LeftParenLiteral:
                            return Classification.Function;
                        default:
                            if (previous?.Kind == Parser.NumberSymbol)
                            {
                                return Classification.Keyword;
                            }
                            else
                            {
                                return Classification.Identifier;
                            }
                    }

                case Parser.BracketedidentifierSymbol:
                    return Classification.Identifier;

                case Parser.NumberSymbol:
                    return Classification.Number;

                case Parser.StringSymbol:
                    return Classification.String;

                case Parser.VariableSymbol:
                    return Classification.Variable;

                case Parser.UseLiteral:
                case Parser.DefaultLiteral:
                case Parser.ForLiteral:
                case Parser.DeclareLiteral:
                case Parser.JobLiteral:
                case Parser.TriggeredLiteral:
                case Parser.EveryLiteral:
                case Parser.AfterLiteral:
                case Parser.ByLiteral:
                case Parser.OrLiteral:
                case Parser.BeginLiteral:
                case Parser.EndLiteral:
                case Parser.TriggerLiteral:
                case Parser.ImportLiteral:
                case Parser.PluginLiteral:
                case Parser.InsertLiteral:
                case Parser.UpsertLiteral:
                case Parser.IntoLiteral:
                case Parser.UnionLiteral:
                case Parser.SelectLiteral:
                case Parser.FromLiteral:
                case Parser.WhereLiteral:
                case Parser.GroupLiteral:
                case Parser.HavingLiteral:
                case Parser.OrderLiteral:
                case Parser.AscLiteral:
                case Parser.DescLiteral:
                case Parser.JoinLiteral:
                case Parser.InnerLiteral:
                case Parser.LeftLiteral:
                case Parser.OnLiteral:
                case Parser.CrossLiteral:
                case Parser.ApplyLiteral:
                case Parser.OuterLiteral:
                case Parser.SequentialLiteral:
                case Parser.AsLiteral:
                case Parser.AndLiteral:
                case Parser.NotLiteral:
                    return Classification.Keyword;

                case Parser.LeftParenLiteral:
                case Parser.RightParenLiteral:
                    return Classification.Parens;

                case Parser.EqualsLiteral:
                case Parser.CommaLiteral:
                case Parser.GreaterThanLiteral:
                case Parser.GreaterThanEqualsLiteral:
                case Parser.LessThanEqualsLiteral:
                case Parser.LessThanLiteral:
                case Parser.LessThanGreaterThanLiteral:
                case Parser.PlusLiteral:
                case Parser.MinusLiteral:
                case Parser.MulLiteral:
                case Parser.DivLiteral:
                case Parser.ModLiteral:
                case Parser.CircumflexLiteral:
                case Parser.ExclamationMarkLiteral:
                    return Classification.Operator;

                case Parser.TrueLiteral:
                case Parser.FalseLiteral:
                case Parser.NullLiteral:
                    return Classification.Number;

                case Parser.DotLiteral:
                    return Classification.Identifier;
                case Parser.UnknownIdentifierLiteral:
                    return Classification.Unknown;
            }

            return Classification.Unknown;
        }
    }
}