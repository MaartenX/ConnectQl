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

namespace ConnectQl.Tools.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// The tuple extensions.
    /// </summary>
    public static class TupleExtensions
    {
        /// <summary>
        /// Concatenates two tuples of spans and completions.
        /// </summary>
        /// <param name="tuple">
        /// The tuple.
        /// </param>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The concatenated items.
        /// </returns>
        [NotNull]
        public static Tuple<SnapshotSpan, IEnumerable<Completion>> Concat([NotNull] this Tuple<SnapshotSpan, IEnumerable<Completion>> tuple, [NotNull] Tuple<SnapshotSpan, IEnumerable<Completion>> other)
        {
            return Tuple.Create(
                tuple.Item1,
                tuple.Item2.Concat(other.Item2));
        }
    }
}