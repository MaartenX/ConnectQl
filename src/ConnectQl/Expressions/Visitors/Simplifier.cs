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

namespace ConnectQl.Expressions.Visitors
{
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Simplifies expressions by evaluating constant nodes.
    /// </summary>
    internal class Simplifier : ExpressionVisitor
    {
        /// <summary>
        /// The visit binary.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var result = base.VisitBinary(node);

            node = result as BinaryExpression;

            if (node?.Left is ConstantExpression && node.Right is ConstantExpression)
            {
                return Evaluate(node);
            }

            if (node?.NodeType == ExpressionType.And || node?.NodeType == ExpressionType.AndAlso)
            {
                if (node.Left is ConstantExpression left)
                {
                    return Equals(left.Value, true) ? node.Right : Expression.Constant(false);
                }

                if (node.Right is ConstantExpression right)
                {
                    return Equals(right.Value, true) ? node.Left : Expression.Constant(false);
                }
            }

            if (node?.NodeType == ExpressionType.Or || node?.NodeType == ExpressionType.OrElse)
            {
                if (node.Left is ConstantExpression left)
                {
                    return Equals(left.Value, true) ? Expression.Constant(true) : node.Right;
                }

                if (node.Right is ConstantExpression right)
                {
                    return Equals(right.Value, true) ? Expression.Constant(true) : node.Left;
                }
            }

            return result;
        }

        /// <summary>
        /// The visit conditional.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            var result = base.VisitConditional(node);

            node = result as ConditionalExpression;

            return node?.Test is ConstantExpression && node.IfTrue is ConstantExpression && node.IfFalse is ConstantExpression ? Evaluate(node) : result;
        }

        /// <summary>
        /// The visit index.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected override Expression VisitIndex(IndexExpression node)
        {
            var result = base.VisitIndex(node);

            node = result as IndexExpression;

            return node != null && (node.Object == null || node.Object is ConstantExpression) && node.Arguments.All(arg => arg is ConstantExpression) ? Evaluate(node) : result;
        }

        /// <summary>
        /// The visit member.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            var result = base.VisitMember(node);

            node = result as MemberExpression;

            return node != null && (node.Expression == null || node.Expression is ConstantExpression) ? Evaluate(node) : result;
        }

        /// <summary>
        /// The visit method call.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var result = base.VisitMethodCall(node);

            node = result as MethodCallExpression;

            return node != null && (node.Object == null || node.Object is ConstantExpression) && node.Arguments.All(arg => arg is ConstantExpression) ? Expression.Constant(node.Method.Invoke((node.Object as ConstantExpression)?.Value, node.Arguments.Cast<ConstantExpression>().Select(c => c.Value).ToArray())) : result;
        }

        /// <summary>
        /// The visit unary.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            var result = base.VisitUnary(node);

            node = result as UnaryExpression;

            return node?.Operand is ConstantExpression ? Evaluate(node) : result;
        }

        /// <summary>
        /// The evaluate.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="ConstantExpression"/>.
        /// </returns>
        private static ConstantExpression Evaluate([NotNull] Expression expression)
        {
            return Expression.Constant(Expression.Lambda(expression).Compile().DynamicInvoke(), expression.Type);
        }
    }
}