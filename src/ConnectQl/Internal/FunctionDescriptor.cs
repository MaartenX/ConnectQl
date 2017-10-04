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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The function descriptor.
    /// </summary>
    internal class FunctionDescriptor : IFunctionDescriptor
    {
        /// <summary>
        /// The arguments.
        /// </summary>
        private readonly ArgumentDescriptor[] arguments;

        /// <summary>
        /// The lambda.
        /// </summary>
        private readonly LambdaExpression lambda;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionDescriptor"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="hasSideEffects">
        /// <c>true</c> if the function has side effects and should not be evaluated for intellisense.
        /// </param>
        /// <param name="lambda">
        /// The lambda.
        /// </param>
        public FunctionDescriptor(string name, bool hasSideEffects, [NotNull] LambdaExpression lambda)
        {
            this.Name = name;
            this.HasSideEffects = hasSideEffects;
            this.lambda = lambda;
            this.arguments = lambda.Parameters.Select(parameter => new ArgumentDescriptor(parameter.Name, new TypeDescriptor(parameter.Type))).ToArray();
            this.ReturnType = new TypeDescriptor(lambda.ReturnType);
            this.IsAggregateFunction = this.arguments.Any(argument => argument.Type.Interfaces.Any(i => i == typeof(IAsyncEnumerable<>) || i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)));
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public IReadOnlyList<IArgumentDescriptor> Arguments => this.arguments;

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this function is an aggregate function.
        /// </summary>
        public bool IsAggregateFunction { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the function has side effects and should not be evaluated for IntelliSense.
        /// </summary>
        public bool HasSideEffects { get; }

        /// <summary>
        /// Gets the return type.
        /// </summary>
        public ITypeDescriptor ReturnType { get; }

        /// <summary>
        /// Gets the lambda expression.
        /// </summary>
        /// <returns>
        /// The lambda expression, or <c>null</c> if it's not supported.
        /// </returns>
        public LambdaExpression GetExpression()
        {
            return this.lambda;
        }

        /// <summary>
        /// The set argument description.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public void SetArgumentDescription(int index, string description)
        {
            this.arguments[index].SetDescription(description);
        }

        /// <summary>
        /// The set description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        public void SetDescription(string description)
        {
            this.Description = description;
        }
    }
}