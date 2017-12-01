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
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// A expr that creates a lambda parameter.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the parameter.
    /// </typeparam>
    public class ParameterExpr<T> : Expr<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterExpr{T}"/> class.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        public ParameterExpr([NotNull] ParameterExpression p) 
            : base(p)
        {
        }

        /// <summary>
        /// Implicitly converts a expr to a parameter expression.
        /// </summary>
        /// <param name="expr">
        /// The expr.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>
        /// </returns>
        [NotNull]
        public static implicit operator ParameterExpression([NotNull] ParameterExpr<T> expr)
        {
            return (ParameterExpression)expr.Expression;
        }

        public Expr Index(string s)
        {
            throw new System.NotImplementedException();
        }
    }
}