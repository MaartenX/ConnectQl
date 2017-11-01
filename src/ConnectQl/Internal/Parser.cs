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

namespace ConnectQl.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The parser.
    /// </summary>
    internal partial class Parser
    {
        /// <summary>
        /// Gets the pos.
        /// </summary>
        [NotNull]
        private Position Pos
        {
            get
            {
                var token = this.scanner.Current;

                return token.ToPosition();
            }
        }

        /// <summary>
        /// The mark.
        /// </summary>
        /// <returns>
        /// The <see cref="IParserContext"/>.
        /// </returns>
        [NotNull]
        internal IParserContext Mark()
        {
            return new ParserContext(this);
        }

        /// <summary>
        /// Sets the context for the node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <typeparam name="T">
        /// The type of the node.
        /// </typeparam>
        /// <returns>
        /// The <typeparamref name="T"/>.
        /// </returns>
        internal T SetContext<T>(T node, [NotNull] IParserContext context)
            where T : Node
        {
            var idx = context.End.TokenIndex - 1;

            for (; this.Tokens[idx].IsComment; idx--)
            {
            }

            var end = idx > 0
                ? new Position
                {
                    Line = this.Tokens[idx - 1].Line,
                    Column = this.Tokens[idx - 1].Col + this.Tokens[idx - 1].Val.Length,
                    TokenIndex = idx,
                }
                : null;

            this.data.Set<IParserContext>(node, "Context", new FrozenContext(context.Start, end ?? context.Start));

            return node;
        }

        /// <summary>
        /// The parse string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ParseString(string value)
        {
            return new Regex("^'|'$").Replace(value, string.Empty).Replace("''", "'");
        }

        /// <summary>
        /// Catches exceptions and logs them as semantic errors.
        /// </summary>
        /// <param name="func">
        /// The function to execute and catch exceptions for.
        /// </param>
        /// <typeparam name="T">
        /// The type the function returns.
        /// </typeparam>
        /// <returns>
        /// The <typeparamref name="T"/>.
        /// </returns>
        private T CatchAll<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                this.SemErr(e.Message);
                return default(T);
            }
        }

        /// <summary>
        /// Parses a number.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The number.
        /// </returns>
        [NotNull]
        private object ParseNumber([NotNull] string value)
        {
            if (value.Contains("."))
            {
                return double.Parse(value, CultureInfo.InvariantCulture);
            }

            if (!long.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var longValue))
            {
                this.SemErr("Invalid number (too long).");
                return 0;
            }

            if (longValue <= int.MaxValue && longValue >= int.MinValue)
            {
                return (int)longValue;
            }

            return longValue;
        }

        /// <summary>
        /// The parse time span.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        /// <param name="objectValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        private TimeSpan ParseTimeSpan([NotNull] string unit, object objectValue)
        {
            var value = Convert.ToDouble(objectValue);

            switch (unit.ToUpperInvariant())
            {
                case "MILLISECOND":
                case "MILLISECONDS":
                    return TimeSpan.FromMilliseconds(value);

                case "SECOND":
                case "SECONDS":
                    return TimeSpan.FromSeconds(value);

                case "MINUTE":
                case "MINUTES":
                    return TimeSpan.FromMinutes(value);

                case "HOUR":
                case "HOURS":
                    return TimeSpan.FromHours(value);

                case "DAY":
                case "DAYS":
                    return TimeSpan.FromDays(value);

                case "WEEK":
                case "WEEKS":
                    return TimeSpan.FromDays(value * 7);

                default:
                    this.SemErr($"Invalid time unit specified: '{unit.ToUpperInvariant()}', must be MILLISECOND[S], SECOND[S], MINUTE[S], HOUR[S], DAY[S] or WEEK[S].");
                    return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// The frozen context.
        /// </summary>
        private class FrozenContext : IParserContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FrozenContext"/> class.
            /// </summary>
            /// <param name="start">
            /// The start.
            /// </param>
            /// <param name="end">
            /// The end.
            /// </param>
            public FrozenContext(Position start, Position end)
            {
                this.Start = start;
                this.End = end;
            }

            /// <summary>
            /// Gets the end.
            /// </summary>
            public Position End { get; }

            /// <summary>
            /// Gets the start.
            /// </summary>
            public Position Start { get; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            [NotNull]
            public override string ToString()
            {
                return $"{this.Start.Line}:{this.Start.Column}-{this.End.Line}:{this.End.Column}";
            }
        }

        /// <summary>
        /// The parser context.
        /// </summary>
        private class ParserContext : IParserContext
        {
            /// <summary>
            /// The parent.
            /// </summary>
            private readonly Parser parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="ParserContext"/> class.
            /// </summary>
            /// <param name="parent">
            /// The parent.
            /// </param>
            public ParserContext([NotNull] Parser parent)
            {
                this.parent = parent;
                this.Start = parent.Pos;
            }

            /// <summary>
            /// Gets the end.
            /// </summary>
            public Position End => this.parent.Pos;

            /// <summary>
            /// Gets the start.
            /// </summary>
            public Position Start { get; }

            /// <summary>
            /// Gets the token.
            /// </summary>
            public Token Token { get; }

            /// <summary>
            /// The sem err.
            /// </summary>
            /// <param name="msg">
            /// The msg.
            /// </param>
            public void SemErr(string msg)
            {
                this.parent.SemErr(this.Start.Line, this.Start.Column, this.Start.TokenIndex, this.End.Line, this.End.Column, this.End.TokenIndex, msg);
            }

            /// <summary>
            /// The sem err.
            /// </summary>
            /// <param name="start">
            /// The start.
            /// </param>
            /// <param name="end">
            /// The end.
            /// </param>
            /// <param name="msg">
            /// The msg.
            /// </param>
            public void SemErr(Position start, Position end, string msg)
            {
                this.parent.SemErr(this.Start.Line, this.Start.Column, this.Start.TokenIndex, this.End.Line, this.End.Column, this.End.TokenIndex, msg);
            }
        }
    }
}