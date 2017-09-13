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

namespace ConnectQl.Internal.Intellisense.Protocol
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.Interfaces;

    /// <summary>
    /// The serializable function descriptor.
    /// </summary>
    [DebuggerDisplay("{" + nameof(SerializableFunctionDescriptor.DisplayName) + ",nq}")]
    internal class SerializableFunctionDescriptor : IFunctionDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableFunctionDescriptor"/> class.
        /// </summary>
        public SerializableFunctionDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableFunctionDescriptor"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        public SerializableFunctionDescriptor(string name, IFunctionDescriptor function)
        {
            this.Name = name;
            this.Description = function.Description;
            this.Arguments = function.Arguments.Count == 0 ? null : function.Arguments.Select(argument => new SerializableArgumentDescriptor(argument)).ToArray();
            this.ReturnType = new SerializableTypeDescriptor(function.ReturnType);
            this.IsAggregateFunction = function.IsAggregateFunction;
            this.HasSideEffects = function.HasSideEffects;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the function has side effects and should not be evaluated for IntelliSense.
        /// </summary>
        public bool HasSideEffects { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        public SerializableArgumentDescriptor[] Arguments { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this function is an aggregate function.
        /// </summary>
        public bool IsAggregateFunction { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the return type.
        /// </summary>
        public SerializableTypeDescriptor ReturnType { get; set; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        IReadOnlyList<IArgumentDescriptor> IFunctionDescriptor.Arguments => this.Arguments ?? new IArgumentDescriptor[0];

        /// <summary>
        /// Gets the return type.
        /// </summary>
        ITypeDescriptor IFunctionDescriptor.ReturnType => this.ReturnType;

        /// <summary>
        /// Gets the display name.
        /// </summary>
        private string DisplayName => $"{this.Name}({string.Join(", ", (this.Arguments ?? Enumerable.Empty<IArgumentDescriptor>()).Select(a => a.Name))})";

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
            var other = obj as SerializableFunctionDescriptor;

            return other != null &&
                   this.IsAggregateFunction == other.IsAggregateFunction &&
                   string.Equals(this.Name, other.Name) &&
                   string.Equals(this.Description, other.Description) &&
                   object.Equals(this.ReturnType, other.ReturnType) &&
                   EnumerableComparer.Equals(this.Arguments, other.Arguments);
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
                var hashCode = EnumerableComparer.GetHashCode(this.Arguments);
                hashCode = (hashCode * 397) ^ (this.Description?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ this.IsAggregateFunction.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.ReturnType?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Gets the lambda expression.
        /// </summary>
        /// <returns>
        /// The lambda expression, or <c>null</c> if it's not supported.
        /// </returns>
        LambdaExpression IFunctionDescriptor.GetExpression() => null;

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool Equals(SerializableFunctionDescriptor other)
        {
            return object.Equals(this.Arguments, other.Arguments) && string.Equals(this.Description, other.Description) && this.IsAggregateFunction == other.IsAggregateFunction && string.Equals(this.Name, other.Name) && object.Equals(this.ReturnType, other.ReturnType);
        }
    }
}