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

namespace ConnectQl.Comparers
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Uses to sort comparers to get the most specific comparer first.
    /// </summary>
    internal class MostSpecificComparer : IComparer<BinaryExpression>
    {
        /// <summary>
        /// The default.
        /// </summary>
        public static readonly MostSpecificComparer Default = new MostSpecificComparer();

        /// <summary>
        /// Compares the two <see cref="BinaryExpression"/>s.
        /// </summary>
        /// <param name="x">
        /// The first expression.
        /// </param>
        /// <param name="y">
        /// The second expression.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Compare(BinaryExpression x, BinaryExpression y)
        {
            return MostSpecificComparer.SortOrder(x) - MostSpecificComparer.SortOrder(y);
        }

        /// <summary>
        /// Converts a <see cref="BinaryExpression"/> to a sort order.
        /// </summary>
        /// <param name="x">
        /// The expression.
        /// </param>
        /// <returns>
        /// The sort order.
        /// </returns>
        private static int SortOrder([NotNull] BinaryExpression x)
        {
            switch (x.NodeType)
            {
                case ExpressionType.Equal:
                    return 0;
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.GreaterThan:
                    return 1;
                case ExpressionType.NotEqual:
                    return 2;
                default:
                    return 3;
            }
        }
    }
}