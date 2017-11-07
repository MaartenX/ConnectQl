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

namespace ConnectQl.Parser
{
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The token extensions.
    /// </summary>
    internal static class TokenExtensions
    {
        /// <summary>
        /// The to position.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Position"/>.
        /// </returns>
        [NotNull]
        public static Position ToPosition([CanBeNull] this Token token)
        {
            return token == null
                       ? new Position
                             {
                                 Column = 1,
                                 Line = 1,
                                 TokenIndex = -1,
                             }
                       : token.Col == 0 && token.Line == 0
                           ? new Position
                                 {
                                     Column = 1,
                                     Line = 1,
                                     TokenIndex = -1,
                                 }
                           : new Position
                                 {
                                     Column = token.Col,
                                     Line = token.Line,
                                     TokenIndex = token.Index - 1,
                                 };
        }
    }
}