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

namespace ConnectQl.Internal.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.Expressions;
    using ConnectQl.Internal.Expressions;

    /// <summary>
    /// The expression comparer.
    /// </summary>
    internal class ExpressionComparer : IEqualityComparer<Expression>
    {
        /// <summary>
        /// The default.
        /// </summary>
        public static readonly ExpressionComparer Default = new ExpressionComparer();

        /// <summary>
        /// The ignore variable names.
        /// </summary>
        private readonly bool ignoreVariableNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionComparer"/> class.
        /// </summary>
        public ExpressionComparer()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionComparer"/> class.
        /// </summary>
        /// <param name="ignoreVariableNames">
        /// The ignore variable names.
        /// </param>
        private ExpressionComparer(bool ignoreVariableNames)
        {
            this.ignoreVariableNames = ignoreVariableNames;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(Expression x, Expression y)
        {
            return x == null && y == null || x != null && y != null && x.GetType() == y.GetType() &&
                   (

                       // Builtin expressions.
                       Compare<BinaryExpression>(x, y, (first, second) => this.Equals(first.Left, second.Left) && this.Equals(first.Right, second.Right)) ||
                       Compare<BlockExpression>(x, y, (first, second) => first.Expressions.SequenceEqual(second.Expressions, this) && this.Equals(first.Result, second.Result) && first.Variables.SequenceEqual(second.Variables, this)) ||
                       Compare<ConditionalExpression>(x, y, (first, second) => this.Equals(first.Test, second.Test) && this.Equals(first.IfTrue, second.IfTrue) && this.Equals(first.IfFalse, second.IfFalse)) ||
                       Compare<ConstantExpression>(x, y, (first, second) => Equals(first.Value, second.Value)) ||
                       Compare<DefaultExpression>(x, y, (first, second) => true) ||
                       Compare<GotoExpression>(x, y, (first, second) => Equals(first.Kind, second.Kind) && first.Target.Equals(second.Target) && this.Equals(first.Value, second.Value)) ||
                       Compare<IndexExpression>(x, y, (first, second) => this.Equals(first.Object, second.Object) && first.Indexer.Equals(second.Indexer) && first.Arguments.SequenceEqual(second.Arguments, this)) ||
                       Compare<InvocationExpression>(x, y, (first, second) => this.Equals(first.Expression, second.Expression) && first.Arguments.SequenceEqual(second.Arguments, this)) ||
                       Compare<LabelExpression>(x, y, (first, second) => this.Equals(first.DefaultValue, second.DefaultValue) && Equals(first.Target, second.Target)) ||
                       Compare<LambdaExpression>(x, y, (first, second) => this.Equals(first.Body, second.Body) && Equals(first.Name, second.Name) && first.Parameters.SequenceEqual(second.Parameters, this) && Equals(first.TailCall, second.TailCall) && Equals(first.ReturnType, second.ReturnType)) ||
                       Compare<ListInitExpression>(x, y, (first, second) => this.Equals(first.NewExpression, second.NewExpression) && first.Initializers.Cast<Expression>().SequenceEqual(second.Initializers.Cast<Expression>(), this)) ||
                       Compare<LoopExpression>(x, y, (first, second) => this.Equals(first.Body, second.Body) && Equals(first.BreakLabel, second.BreakLabel) && Equals(first.ContinueLabel, second.ContinueLabel)) ||
                       Compare<MemberExpression>(x, y, (first, second) => this.Equals(first.Expression, second.Expression) && first.Member.Equals(second.Member)) ||
                       Compare<MemberInitExpression>(x, y, (first, second) => this.Equals(first.NewExpression, second.NewExpression) && first.Bindings.SequenceEqual(second.Bindings)) ||
                       Compare<MethodCallExpression>(x, y, (first, second) => this.Equals(first.Object, second.Object) && Equals(first.Method, second.Method) && first.Arguments.SequenceEqual(second.Arguments, this)) ||
                       Compare<NewArrayExpression>(x, y, (first, second) => first.Expressions.SequenceEqual(second.Expressions, this)) ||
                       Compare<ParameterExpression>(x, y, (first, second) => Equals(first.IsByRef, second.IsByRef) && first.Type == second.Type && (this.ignoreVariableNames || Equals(first.Name, second.Name))) ||
                       Compare<RuntimeVariablesExpression>(x, y, (first, second) => first.Variables.SequenceEqual(second.Variables, this)) ||
                       Compare<TypeBinaryExpression>(x, y, (first, second) => this.Equals(first.Expression, second.Expression) && first.TypeOperand == second.TypeOperand) ||
                       Compare<UnaryExpression>(x, y, (first, second) => this.Equals(first.Operand, second.Operand)) ||

                       // Custom expressions.
                       Compare<TaskExpression>(x, y, (first, second) => this.Equals(first.Expression, second.Expression)) ||
                       Compare<CompareExpression>(x, y, (first, second) => this.Equals(first.Left, second.Left) && this.Equals(first.Right, second.Right) && first.CompareType == second.CompareType) ||
                       Compare<ExecutionContextExpression>(x, y, (first, second) => true) ||
                       Compare<SourceFieldExpression>(x, y, (first, second) => Equals(first.FieldName, second.FieldName) && Equals(first.SourceName, second.SourceName)) ||
                       Compare<RangeExpression>(x, y, (first, second) => Equals(first.Min, second.Min) & Equals(first.Max, second.Max)));
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetHashCode(Expression obj)
        {
            return obj?.GetType().GetHashCode() ?? 0;
        }

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items to compare.
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool Compare<T>(Expression x, Expression y, Func<T, T, bool> comparison)
            where T : Expression
        {
            var first = x as T;
            var second = y as T;

            return first != null && second != null && first.NodeType == second.NodeType && first.Type == second.Type && comparison(first, second);
        }
    }
}