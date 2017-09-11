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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;

    /// <summary>
    /// Stores the auto completions.
    /// </summary>
    public class AutoCompletions : IAutoCompletions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompletions"/> class.
        /// </summary>
        /// <param name="literals">The literals.</param>
        public AutoCompletions(IReadOnlyList<string> literals)
            : this(AutoCompleteType.Literal, literals)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompletions"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public AutoCompletions(AutoCompleteType type)
            : this(type, (IReadOnlyList<string>)null)
        {
            if (type.HasFlag(AutoCompleteType.Literal))
            {
                throw new ArgumentException("When AutoCompleteOptions.Literal is set, parameter literals must be supplied.", nameof(type));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompletions"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="literals">The literals.</param>
        public AutoCompletions(AutoCompleteType type, params IEnumerable<string>[] literals)
            : this(type, literals.Where(l => l != null).SelectMany(l => l).Distinct().ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompletions"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="keyword">The keyword.</param>
        public AutoCompletions(AutoCompleteType type, string keyword)
            : this(type, new[] { keyword })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompletions"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="literals">The literals.</param>
        public AutoCompletions(AutoCompleteType type, IReadOnlyList<string> literals)
        {
            this.Type = type;
            this.Literals = literals;
        }

        /// <summary>
        /// Gets the auto complete type that is valid at this point.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public AutoCompleteType Type { get; }

        /// <summary>
        /// Gets the literals that are valid at this point. This property is <c>null</c> when <see cref="Type"/> does not have the
        /// flag <see cref="AutoCompleteType.Literal"/> set.
        /// </summary>
        /// <value>
        /// The literals.
        /// </value>
        public IReadOnlyList<string> Literals { get; }
    }
}