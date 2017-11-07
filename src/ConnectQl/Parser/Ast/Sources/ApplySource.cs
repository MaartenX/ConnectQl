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

namespace ConnectQl.Parser.Ast.Sources
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Visitors;

    /// <summary>
    /// The apply source.
    /// </summary>
    internal class ApplySource : SourceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplySource"/> class.
        /// </summary>
        /// <param name="left">
        /// The source.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="isOuterApply">
        /// <c>true</c> if this is an OUTER APPLY, false if this is a CROSS APPLY.
        /// </param>
        public ApplySource(SourceBase left, SourceBase right, bool isOuterApply)
        {
            this.Left = left;
            this.Right = right;
            this.IsOuterApply = isOuterApply;
        }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public override IEnumerable<Node> Children
        {
            get
            {
                yield return this.Left;
                yield return this.Right;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is an OUTER APPLY or a CROSS APPLY.
        /// </summary>
        public bool IsOuterApply { get; }

        /// <summary>
        /// Gets the left source.
        /// </summary>
        public SourceBase Left { get; }

        /// <summary>
        /// Gets the right source.
        /// </summary>
        public SourceBase Right { get; }

        /// <summary>
        /// Dispatches the visitor to the correct visit-method.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        protected internal override Node Accept([NotNull] NodeVisitor visitor)
        {
            return visitor.VisitApplySource(this);
        }

        /// <summary>
        /// Visits the children of this node.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        [NotNull]
        protected internal override Node VisitChildren([NotNull] NodeVisitor visitor)
        {
            var left = visitor.Visit(this.Left);
            var right = visitor.Visit(this.Right);

            return object.ReferenceEquals(left, this.Left) && object.ReferenceEquals(right, this.Right)
                       ? this
                       : new ApplySource(left, right, this.IsOuterApply);
        }
    }
}