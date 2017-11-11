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

namespace ConnectQl.Query.Factories
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;

    using JetBrains.Annotations;
    
    /// <summary>
    /// The factory.
    /// </summary>
    /// <typeparam name="T">
    /// The return type.
    /// </typeparam>
    public class Factory<T>
    {
        /// <summary>
        /// The expression.
        /// </summary>
        private readonly Expression expression;

        /// <summary>
        /// Lazy evaluated check if this expression has <see cref="TaskExpression"/>s.
        /// </summary>
        private readonly Lazy<bool> hasTasks;

        /// <summary>
        /// Initializes a new instance of the <see cref="Factory{T}"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        private Factory([NotNull] Expression expression)
        {
            if (expression.Type.IsConstructedGenericType && expression.Type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                this.expression = ConnectQlExpression.MakeTask(expression);
            }

            this.expression = expression;

            Debug.Assert(this.expression != null, "Null");
            Debug.Assert(this.expression.Type == typeof(T), $"Cannot assign expression of type {this.expression.Type} to a Factory<{typeof(T)}>.");

            var tasksFound = false;

            this.hasTasks = new Lazy<bool>(() => GenericVisitor.Visit(
                                                     (TaskExpression t) =>
                                                     {
                                                         tasksFound = true;
                                                         return t;
                                                     },
                                                     (LambdaExpression e) => e,
                                                     expression) != null && tasksFound);
        }

        /// <summary>
        /// Gets a value indicating whether this factory has tasks, and will return an asynchronous result.
        /// </summary>
        public bool HasTasks => this.hasTasks.Value;

        /// <summary>
        /// Implicitly converts a factory to an expression.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>
        /// </returns>
        [NotNull]
        public static implicit operator Expression([NotNull] Factory<T> factory)
        {
            return factory?.expression;
        }

        /// <summary>
        /// Implicitly converts an expression to a factory.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="Factory{T}"/>.
        /// </returns>
        [NotNull]
        public static implicit operator Factory<T>([NotNull] Expression expression)
        {
            return new Factory<T>(expression);
        }
    }
}
