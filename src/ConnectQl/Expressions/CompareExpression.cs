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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Internal.Extensions;

    /// <summary>
    /// The compare expression.
    /// </summary>
    public class CompareExpression : CustomExpression
    {
        /// <summary>
        /// The <see cref="Convert.ChangeType(object,System.Type)"/> method.
        /// </summary>
        private static readonly MethodInfo ChangeTypeMethod = typeof(Convert).GetMethod(nameof(System.Convert.ChangeType), typeof(object), typeof(Type));

        /// <summary>
        /// The <see cref="string.Compare(string,string)"/> method.
        /// </summary>
        private static readonly MethodInfo CompareMethod =
            typeof(string).GetMethod(nameof(string.Compare), typeof(string), typeof(string));

        /// <summary>
        /// The <see cref="CompareValues"/> method.
        /// </summary>
        private static readonly MethodInfo CompareValuesMethod = typeof(CompareExpression).GetTypeInfo().GetDeclaredMethods(nameof(CompareValues)).First();

        /// <summary>
        /// The ops.
        /// </summary>
        private static readonly Dictionary<ExpressionType, string> Ops = new Dictionary<ExpressionType, string>
                                                                             {
                                                                                 {
                                                                                     ExpressionType.Equal, "=="
                                                                                 },
                                                                                 {
                                                                                     ExpressionType.NotEqual, "!="
                                                                                 },
                                                                                 {
                                                                                     ExpressionType.GreaterThanOrEqual, ">="
                                                                                 },
                                                                                 {
                                                                                     ExpressionType.GreaterThan, ">"
                                                                                 },
                                                                                 {
                                                                                     ExpressionType.LessThanOrEqual, "<="
                                                                                 },
                                                                                 {
                                                                                     ExpressionType.LessThan, "<"
                                                                                 },
                                                                             };

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareExpression"/> class.
        /// </summary>
        /// <param name="compareType">
        /// The compare type.
        /// </param>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        protected internal CompareExpression(ExpressionType compareType, Expression left, Expression right)
            : base(typeof(bool))
        {
            this.CompareType = compareType;

            if (left.Type == typeof(object) && right.Type != typeof(object))
            {
                var field = left as SourceFieldExpression;
                left = field != null
                           ? (Expression)MakeSourceField(field.SourceName, field.FieldName, right.Type)
                           : Convert(Call(null, ChangeTypeMethod, left, Constant(right.Type)), right.Type);
            }

            if (left.Type != typeof(object) && right.Type == typeof(object))
            {
                var field = right as SourceFieldExpression;
                right = field != null
                            ? (Expression)MakeSourceField(field.SourceName, field.FieldName, left.Type)
                            : Convert(Call(null, ChangeTypeMethod, right, Constant(left.Type)), left.Type);
            }

            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Gets a value indicating whether the current node can be reduced.
        /// </summary>
        public override bool CanReduce => true;

        /// <summary>
        /// Gets the comparer.
        /// </summary>
        public IComparer<object> Comparer
        {
            get
            {
                if (this.Left.Type == typeof(object) || this.Right.Type == typeof(object))
                {
                    return Comparer<object>.Create((a, b) => string.Compare(a?.ToString() ?? string.Empty, b?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase));
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the compare type.
        /// </summary>
        public ExpressionType CompareType { get; }

        /// <summary>
        /// Gets the left.
        /// </summary>
        public Expression Left { get; }

        /// <summary>
        /// Gets the right.
        /// </summary>
        public Expression Right { get; }

        /// <summary>
        /// Creates a comparison expression for the specified expressions.
        /// </summary>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public Expression CreateComparer()
        {
            if (this.Left.Type == this.Right.Type)
            {
                if (this.Left.Type != typeof(object) && this.Left.Type != typeof(string))
                {
                    return MakeBinary(this.CompareType, this.Left, this.Right);
                }

                if (this.Left.Type == typeof(string))
                {
                    return MakeBinary(this.CompareType, Call(CompareMethod, this.Left, this.Right), Constant(0));
                }
            }

            return Call(null, CompareValuesMethod, Constant(this.CompareType), Convert(this.Left, typeof(object)), Convert(this.Right, typeof(object)));
        }

        /// <summary>
        /// Reduces the expression.
        /// </summary>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public override Expression Reduce()
        {
            return this.CreateComparer();
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Left} {Ops[this.CompareType]} {this.Right}";
        }

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <param name="type">
        /// The way to compare the two values.
        /// </param>
        /// <param name="first">
        /// The first value.
        /// </param>
        /// <param name="second">
        /// The second value.
        /// </param>
        /// <returns>
        /// True if the comparison is true, false otherwise.
        /// </returns>
        internal static bool CompareValues(ExpressionType type, object first, object second)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return Equals(first, second);
                case ExpressionType.NotEqual:
                    return !Equals(first, second);
            }

            if (first?.GetType() == second?.GetType())
            {
                var result = Comparer<object>.Default.Compare(first, second);

                switch (type)
                {
                    case ExpressionType.GreaterThan:
                        return result > 0;
                    case ExpressionType.GreaterThanOrEqual:
                        return result >= 0;
                    case ExpressionType.LessThan:
                        return result < 0;
                    case ExpressionType.LessThanOrEqual:
                        return result <= 0;
                }
            }

            if (first is string && second != null)
            {
                var result = Comparer<object>.Default.Compare(System.Convert.ChangeType(first, second.GetType()), second);

                switch (type)
                {
                    case ExpressionType.GreaterThan:
                        return result > 0;
                    case ExpressionType.GreaterThanOrEqual:
                        return result >= 0;
                    case ExpressionType.LessThan:
                        return result < 0;
                    case ExpressionType.LessThanOrEqual:
                        return result <= 0;
                }
            }

            if (second is string && first != null)
            {
                var result = Comparer<object>.Default.Compare(first, System.Convert.ChangeType(second, first.GetType()));

                switch (type)
                {
                    case ExpressionType.GreaterThan:
                        return result > 0;
                    case ExpressionType.GreaterThanOrEqual:
                        return result >= 0;
                    case ExpressionType.LessThan:
                        return result < 0;
                    case ExpressionType.LessThanOrEqual:
                        return result <= 0;
                }
            }

            return false;
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
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var left = visitor.Visit(this.Left);
            var right = visitor.Visit(this.Right);

            return ReferenceEquals(left, this.Left) && ReferenceEquals(right, this.Right)
                       ? this
                       : new CompareExpression(this.CompareType, left, right);
        }
    }
}