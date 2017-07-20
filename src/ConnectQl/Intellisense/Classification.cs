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
    /// <summary>
    /// The classification.
    /// </summary>
    public enum Classification
    {
        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The comment.
        /// </summary>
        Comment,

        /// <summary>
        /// The identifier.
        /// </summary>
        Identifier,

        /// <summary>
        /// The number.
        /// </summary>
        Number,

        /// <summary>
        /// The string.
        /// </summary>
        String,

        /// <summary>
        /// The variable.
        /// </summary>
        Variable,

        /// <summary>
        /// The function.
        /// </summary>
        Function,

        /// <summary>
        /// The keyword.
        /// </summary>
        Keyword,

        /// <summary>
        /// The parentheses.
        /// </summary>
        Parens,

        /// <summary>
        /// The operator.
        /// </summary>
        Operator,

        /// <summary>
        /// The constant.
        /// </summary>
        Constant,

        /// <summary>
        /// The source.
        /// </summary>
        Source,
    }
}