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

namespace ConnectQl.Internal.Ast.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using ConnectQl.Internal.Ast.Visitors;

    using JetBrains.Annotations;

    /// <summary>
    /// The function call.
    /// </summary>
    internal class FunctionCallSqlExpression : SqlExpressionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCallSqlExpression"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public FunctionCallSqlExpression(string name, ReadOnlyCollection<SqlExpressionBase> arguments)
        {
            this.Name = name;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public ReadOnlyCollection<SqlExpressionBase> Arguments { get; }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public override IEnumerable<Node> Children => this.Arguments;

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// True if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        public override bool Equals(object obj)
        {
            var other = obj as FunctionCallSqlExpression;

            return other != null && string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase) && this.Arguments.SequenceEqual(other.Arguments);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return this.Arguments.Aggregate(0, (total, expression) => (total * 397) ^ expression?.GetHashCode() ?? 0) * 397 ^ (this.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Name) : 0);
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NotNull]
        public override string ToString() => $"{this.Name.ToUpperInvariant()}({string.Join(", ", this.Arguments)})";

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
            return visitor.VisitFunctionCallSqlExpression(this);
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
            var arguments = visitor.Visit(this.Arguments);

            return arguments != this.Arguments ? new FunctionCallSqlExpression(this.Name, arguments) : this;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// True if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// The other.
        /// </param>
        protected bool Equals([NotNull] FunctionCallSqlExpression other)
        {
            return this.Arguments.SequenceEqual(other.Arguments) && string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}