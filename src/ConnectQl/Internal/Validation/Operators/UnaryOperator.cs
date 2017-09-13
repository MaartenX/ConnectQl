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

    /// <summary>
    /// The unary operator.
    /// </summary>
    internal class UnaryOperator : Operator
    {
        /// <summary>
        /// The operators.
        /// </summary>
        private static readonly Dictionary<string, Func<Expression, Expression>> Operators
            = new Dictionary<string, Func<Expression, Expression>>(
                  StringComparer.OrdinalIgnoreCase)
                  {
                      {
                          "+", UnaryOperator.GeneratePlus
                      },
                      {
                          "-", UnaryOperator.GenerateMinus
                      },
                      {
                          "!", UnaryOperator.GenerateNot
                      },
                      {
                          "NOT", UnaryOperator.GenerateNot
                      },
                  };

        /// <summary>
        /// Generates an <see cref="Expression"/> for the unary  operator.
        /// </summary>
        /// <param name="op">
        /// The operator.
        /// </param>
        /// <param name="operand">
        /// The operand of the unary expression.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression GenerateExpression(string op, Expression operand)
        {
            if (!UnaryOperator.Operators.TryGetValue(op, out Func<Expression, Expression> generator))
            {
                throw new InvalidOperationException($"Unknown operator '{op}' with type '{operand}'.");
            }

            try
            {
                return generator(operand);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Operator '{op}' is not supported for type '{operand}'.", e);
            }
        }

        /// <summary>
        /// Infers the type of the unary expression.
        /// </summary>
        /// <param name="op">
        /// The operator.
        /// </param>
        /// <param name="operand">
        /// The operand of the unary expression.
        /// </param>
        /// ///
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public static Type InferType(string op, Type operand)
        {
            return UnaryOperator.GenerateExpression(op, Expression.Parameter(operand)).Type;
        }

        /// <summary>
        /// Generates an expression for the '-' operator.
        /// </summary>
        /// <param name="operand">
        /// The argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateMinus(Expression operand)
        {
            return Expression.Negate(operand);
        }

        /// <summary>
        /// Generates an expression for the '!' and 'NOT' operator.
        /// </summary>
        /// <param name="operand">
        /// The argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GenerateNot(Expression operand)
        {
            return Expression.Not(operand);
        }

        /// <summary>
        /// Generates an expression for the '+' operator.
        /// </summary>
        /// <param name="operand">
        /// The argument.
        /// </param>
        /// <returns>
        /// The generated expression.
        /// </returns>
        private static Expression GeneratePlus(Expression operand)
        {
            return Expression.UnaryPlus(operand);
        }
    }
}