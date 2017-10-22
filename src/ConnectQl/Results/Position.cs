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
namespace ConnectQl.Results
{
    using System.Diagnostics;

    using JetBrains.Annotations;

    /// <summary>
    ///     The position.
    /// </summary>
    [DebuggerDisplay("ln {Line, nq}, col {Column, nq}")]
    public class Position
    {
        /// <summary>
        ///     Gets or sets the column.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        ///     Gets or sets the line.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        ///     Gets or sets the token index.
        /// </summary>
        public int TokenIndex { get; set; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first position.</param>
        /// <param name="second">The second position.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==([CanBeNull] Position first, [CanBeNull] Position second)
        {
            return first?.Equals(second) ?? second?.Equals(null) ?? true;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first position.</param>
        /// <param name="second">The second position.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=([CanBeNull] Position first, [CanBeNull] Position second)
        {
            return !(first == second);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as Position;
            return other != null && this.Column == other.Column && this.Line == other.Line && this.TokenIndex == other.TokenIndex;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Column;
                hashCode = (hashCode * 397) ^ this.Line;
                hashCode = (hashCode * 397) ^ this.TokenIndex;
                return hashCode;
            }
        }
    }
}