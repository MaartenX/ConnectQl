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

    using ConnectQl.Internal.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// The custom expression.
    /// </summary>
    public abstract class CustomExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomExpression"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        protected CustomExpression(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the node type of this <see cref="T:System.Linq.Expressions.Expression"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Linq.Expressions.ExpressionType"/> values.
        /// </returns>
        public override sealed ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// Gets the type.
        /// </summary>
        public override sealed Type Type { get; }

        /// <summary>
        /// The execution context.
        /// </summary>
        /// <returns>
        /// The <see cref="ExecutionContextExpression"/>.
        /// </returns>
        [NotNull]
        public static ExecutionContextExpression ExecutionContext()
        {
            return new ExecutionContextExpression();
        }

        /// <summary>
        /// Creates a compare expression.
        /// </summary>
        /// <param name="compareType">
        /// The compare type.
        /// </param>
        /// <param name="left">
        /// The left expression.
        /// </param>
        /// <param name="right">
        /// The right expression.
        /// </param>
        /// <returns>
        /// The <see cref="CompareExpression"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when an invalid <see cref="ExpressionType"/> is passed in.
        /// </exception>
        [NotNull]
        public static CompareExpression MakeCompare(ExpressionType compareType, Expression left, Expression right)
        {
            switch (compareType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), "Compare type must be an expression type that compares two values.");
            }

            return new CompareExpression(compareType, left, right);
        }

        /// <summary>
        /// Creates a <see cref="FieldExpression"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="FieldExpression"/>.
        /// </returns>
        [NotNull]
        public static FieldExpression MakeField(string source, string name)
        {
            return new FieldExpression(source, name, typeof(object));
        }

        /// <summary>
        /// Creates a <see cref="FieldExpression"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="FieldExpression"/>.
        /// </returns>
        [NotNull]
        public static FieldExpression MakeField(string source, string name, Type type)
        {
            return new FieldExpression(source, name, type);
        }

        /// <summary>
        /// Creates a RangeExpression, given the minimum and maximum, by calling an appropriate factory method.
        /// </summary>
        /// <param name="min">
        /// The minimum value in the range.
        /// </param>
        /// <param name="max">
        /// The maximum value in the range.
        /// </param>
        /// <param name="type">
        /// The type of the expression.
        /// </param>
        /// <returns>
        /// The RangeExpression that results from calling the appropriate factory method.
        /// </returns>
        [NotNull]
        internal static RangeExpression MakeRange(object min, object max, Type type)
        {
            return new RangeExpression(min, max, type);
        }

        /// <summary>
        /// Creates a RangeExpression, given the minimum and maximum, by calling an appropriate factory method.
        /// </summary>
        /// <param name="min">
        /// The minimum value in the range.
        /// </param>
        /// <param name="max">
        /// The maximum value in the range.
        /// </param>
        /// <returns>
        /// The RangeExpression that results from calling the appropriate factory method.
        /// </returns>
        [NotNull]
        internal static RangeExpression MakeRange([CanBeNull] object min, [CanBeNull] object max)
        {
            return new RangeExpression(min, max, min?.GetType() ?? max?.GetType() ?? typeof(object));
        }

        /// <summary>
        /// Creates a <see cref="SourceFieldExpression"/>, given the source name and field name, by calling an appropriate
        ///     factory method.
        /// </summary>
        /// <param name="sourceName">
        /// The name that represents the source.
        /// </param>
        /// <param name="fieldName">
        /// The name that represents the field.
        /// </param>
        /// <returns>
        /// The FieldExpression that results from calling the appropriate factory method.
        /// </returns>
        [NotNull]
        internal static SourceFieldExpression MakeSourceField(string sourceName, string fieldName)
        {
            return new SourceFieldExpression(sourceName, fieldName, false, typeof(object));
        }

        /// <summary>
        /// Creates a <see cref="SourceFieldExpression"/>, given the source name and field name, and a value indicating
        ///     whether this is an internal name.
        /// </summary>
        /// <param name="sourceName">
        /// The name that represents the source.
        /// </param>
        /// <param name="fieldName">
        /// The name that represents the field.
        /// </param>
        /// <param name="useInternalName">
        /// <c>true</c> to use the internal name, <c>false</c> otherwise.
        /// </param>
        /// <returns>
        /// The FieldExpression that results from calling the appropriate factory method.
        /// </returns>
        [NotNull]
        internal static SourceFieldExpression MakeSourceField(string sourceName, string fieldName, bool useInternalName)
        {
            return new SourceFieldExpression(sourceName, fieldName, useInternalName, typeof(object));
        }

        /// <summary>
        /// Creates a <see cref="SourceFieldExpression"/>, given the source name and field name, type, and a value indicating
        ///     whether this is an internal name.
        /// </summary>
        /// <param name="sourceName">
        /// The name that represents the source.
        /// </param>
        /// <param name="fieldName">
        /// The name that represents the field.
        /// </param>
        /// <param name="useInternalName">
        /// <c>true</c> to use the internal name, <c>false</c> otherwise.
        /// </param>
        /// <param name="type">
        /// The type of the parameter.
        /// </param>
        /// <returns>
        /// The FieldExpression that results from calling the appropriate factory method.
        /// </returns>
        [NotNull]
        internal static SourceFieldExpression MakeSourceField(string sourceName, string fieldName, bool useInternalName, Type type)
        {
            return new SourceFieldExpression(sourceName, fieldName, useInternalName, type);
        }

        /// <summary>
        /// Creates a <see cref="SourceFieldExpression"/>, given the source name and field name and the type.
        /// </summary>
        /// <param name="sourceName">
        /// The name that represents the source.
        /// </param>
        /// <param name="fieldName">
        /// The name that represents the field.
        /// </param>
        /// <param name="type">
        /// The type of the parameter.
        /// </param>
        /// <returns>
        /// The FieldExpression that results from calling the appropriate factory method.
        /// </returns>
        [NotNull]
        internal static SourceFieldExpression MakeSourceField(string sourceName, string fieldName, Type type)
        {
            return new SourceFieldExpression(sourceName, fieldName, false, type);
        }
    }
}