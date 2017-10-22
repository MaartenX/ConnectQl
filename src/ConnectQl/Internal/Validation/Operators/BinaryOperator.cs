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

namespace ConnectQl.Internal.Validation.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using ConnectQl.Expressions;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The binary operator.
    /// </summary>
    internal class BinaryOperator : Operator
    {
        /// <summary>
        /// The operators.
        /// </summary>
        private static readonly Dictionary<string, Func<Expression, Expression, Expression>> Operators
            = new Dictionary<string, Func<Expression, Expression, Expression>>(
                  StringComparer.OrdinalIgnoreCase)
                  {
                      {
                          "+", BinaryOperator.GenerateAdd
                      },
                      {
                          "-", BinaryOperator.GenerateSubtract
                      },
                      {
                          "/", BinaryOperator.GenerateDivide
                      },
                      {
                          "*", BinaryOperator.GenerateMultiply
                      },
                      {
                          "%", BinaryOperator.GenerateModulo
                      },
                      {
                          "^", BinaryOperator.GeneratePower
                      },
                      {
                          ">", BinaryOperator.GenerateGreaterThan
                      },
                      {
                          ">=", BinaryOperator.GenerateGreaterThanOrEqual
                      },
                      {
                          "=", BinaryOperator.GenerateEqual
                      },
                      {
                          "<>", BinaryOperator.GenerateNotEqual
                      },
                      {
                          "<=", BinaryOperator.GenerateLessThanOrEqual
                      },
                      {
                          "<", BinaryOperator.GenerateLessThan
                      },
                      {
                          "AND", BinaryOperator.GenerateAnd
                      },
                      {
                          "OR", BinaryOperator.GenerateOr
                      },
                  };

        /// <summary>
        /// Generates an <see cref="Expression"/> for the binary operator.
        /// </summary>
        /// <param name="first">
        /// The first operand of the binary expression.
        /// </param>
        /// <param name="op">
        /// The operator.
        /// </param>
        /// <param name="second">
        /// The second operand of the binary expression.
        /// </param>
        /// <param name="onError">
        /// Called when errors occur.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression GenerateExpression(Expression first, string op, Expression second, [CanBeNull] Action<string> onError = null)
        {
            if (!BinaryOperator.Operators.TryGetValue(op, out Func<Expression, Expression, Expression> generator))
            {
                var message = $"Unknown operator '{op}' with types '{first.Type}' and '{second.Type}'.";

                onError?.Invoke(message);

                return Expression.Constant(new Error(new KeyNotFoundException(message)));
            }

            try
            {
                return generator(first, second);
            }
            catch (Exception e)
            {
                var message = $"Operator '{op}' is not supported for types '{first.Type}' and '{second.Type}'.";

                onError?.Invoke(message);

                return Expression.Constant(new Error(new InvalidOperationException(message, e)));
            }
        }

        /// <summary>
        /// Infers the type of the binary expression.
        /// </summary>
        /// <param name="first">
        /// The first operand of the binary expression.
        /// </param>
        /// <param name="op">
        /// The operator.
        /// </param>
        /// <param name="second">
        /// The second operand of the binary expression.
        /// </param>
        /// <param name="errorCallback">
        /// Called when errors occur.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public static Type InferType(Type first, string op, Type second, Action<string> errorCallback)
        {
            return BinaryOperator.GenerateExpression(Expression.Parameter(first), op, Expression.Parameter(second), errorCallback).Type;
        }

        /// <summary>
        /// Applies the expression function to the arguments after converting to a common type.
        /// </summary>
        /// <param name="first">
        /// The first expression.
        /// </param>
        /// <param name="second">
        /// The second expression.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private static Expression DoConversion(Expression first, Expression second, [NotNull] Func<Expression, Expression, Expression> expression)
        {
            if (first.Type != second.Type)
            {
                // Try to cast the arguments to a common type, starting with the highest precision.
                foreach (var type in Operator.CommonTypeOrder)
                {
                    if (first.Type == type || second.Type == type)
                    {
                        first = Operator.ToType(first, type);
                        second = Operator.ToType(second, type);
                        break;
                    }
                }
            }

            return expression(first, second);
        }

        /// <summary>
        /// Generates an expression for the '+' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateAdd([NotNull] Expression first, Expression second)
        {
            return first.Type == typeof(string) || second.Type == typeof(string)
                       ? Expression.Call(typeof(string).GetRuntimeMethod("Concat", new[] { typeof(string), typeof(string) }), Operator.ToString(first), Operator.ToString(second))
                       : BinaryOperator.DoConversion(first, second, Expression.Add);
        }

        /// <summary>
        /// Generates an expression for the 'AND' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateAnd(Expression first, Expression second)
        {
            return BinaryOperator.DoConversion(first, second, Expression.And);
        }

        /// <summary>
        /// Generates an expression for the '/' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateDivide(Expression first, Expression second)
        {
            return BinaryOperator.DoConversion(first, second, Expression.Divide);
        }

        /// <summary>
        /// Generates an expression for the '=' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateEqual(Expression first, Expression second)
        {
            return CustomExpression.MakeCompare(ExpressionType.Equal, first, second);
        }

        /// <summary>
        /// Generates an expression for the '&gt;' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateGreaterThan(Expression first, Expression second)
        {
            return CustomExpression.MakeCompare(ExpressionType.GreaterThan, first, second);
        }

        /// <summary>
        /// Generates an expression for the '&gt;=' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateGreaterThanOrEqual(Expression first, Expression second)
        {
            return CustomExpression.MakeCompare(ExpressionType.GreaterThanOrEqual, first, second);
        }

        /// <summary>
        /// Generates an expression for the '&lt;' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateLessThan(Expression first, Expression second)
        {
            return CustomExpression.MakeCompare(ExpressionType.LessThan, first, second);
        }

        /// <summary>
        /// Generates an expression for the '&lt;=' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateLessThanOrEqual(Expression first, Expression second)
        {
            return CustomExpression.MakeCompare(ExpressionType.LessThanOrEqual, first, second);
        }

        /// <summary>
        /// Generates an expression for the '%' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateModulo(Expression first, Expression second)
        {
            return BinaryOperator.DoConversion(first, second, Expression.Modulo);
        }

        /// <summary>
        /// Generates an expression for the '*' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateMultiply(Expression first, Expression second)
        {
            return BinaryOperator.DoConversion(first, second, Expression.Multiply);
        }

        /// <summary>
        /// Generates an expression for the '&lt;&gt;' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateNotEqual(Expression first, Expression second)
        {
            return CustomExpression.MakeCompare(ExpressionType.NotEqual, first, second);
        }

        /// <summary>
        /// Generates an expression for the 'OR' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateOr(Expression first, Expression second)
        {
            return BinaryOperator.DoConversion(first, second, Expression.Or);
        }

        /// <summary>
        /// Generates an expression for the '^' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GeneratePower(Expression first, Expression second)
        {
            return BinaryOperator.DoConversion(first, second, Expression.Power);
        }

        /// <summary>
        /// Generates an expression for the '-' operator.
        /// </summary>
        /// <param name="first">
        /// The first argument.
        /// </param>
        /// <param name="second">
        /// The second argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateSubtract(Expression first, Expression second)
        {
            return BinaryOperator.DoConversion(first, second, Expression.Subtract);
        }
    }
}