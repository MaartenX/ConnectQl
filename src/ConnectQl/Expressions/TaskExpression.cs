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

namespace ConnectQl.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// The task expression.
    /// </summary>
    public sealed class TaskExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskExpression"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the expression is not a <see cref="Task{TResult}"/>.
        /// </exception>
        internal TaskExpression([NotNull] Expression expression)
        {
            this.Expression = expression;
            this.Type = ConvertType(expression.Type);
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [NotNull]
        public override Type Type { get; }

        /// <summary>
        /// Gets the node type of this <see cref="T:System.Linq.Expressions.Expression"/>.
        /// </summary>
        /// <returns>
        /// <see cref="ExpressionType.Extension"/>.
        /// </returns>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// Gets a value indicating whether this expression can be reduced.
        /// </summary>
        public override bool CanReduce => true;

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// The reduce.
        /// </summary>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public override Expression Reduce()
        {
            return this.Expression;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NotNull]
        public override string ToString()
        {
            return $"await {this.Expression}";
        }

        /// <summary>
        /// The visit children.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        [NotNull]
        protected override Expression VisitChildren([NotNull] ExpressionVisitor visitor)
        {
            var result = visitor.Visit(this.Expression);

            return object.ReferenceEquals(result, this.Expression) ? this : new TaskExpression(result);
        }

        /// <summary>
        /// Converts a <see cref="Task{T}"/> into a T.
        /// </summary>
        /// <param name="taskType">
        /// The task type.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the type is not a <see cref="Task{T}"/>.
        /// </exception>
        private static Type ConvertType(Type taskType)
        {
            var typeInfo = taskType.GetTypeInfo();

            if (!(typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Task<>)))
            {
                throw new ArgumentException("Expression must be of type Task<T>.", nameof(taskType));
            }

            return typeInfo.GenericTypeArguments[0];
        }
    }
}