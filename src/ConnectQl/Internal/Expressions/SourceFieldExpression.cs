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

namespace ConnectQl.Internal.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// Represents an expression that is a reference to a field.
    /// </summary>
    internal class SourceFieldExpression : CustomExpression
    {
        /// <summary>
        /// The row get by internal name method.
        /// </summary>
        private static readonly MethodInfo RowGetByInternalNameMethod = typeof(Row).GetGenericMethod(nameof(Row.GetByInternalName), typeof(string));

        /// <summary>
        /// The <see cref="Row.Get{T}"/> method.
        /// </summary>
        private static readonly MethodInfo RowGetMethod = typeof(Row).GetGenericMethod(nameof(Row.Get), typeof(string));

        /// <summary>
        /// The select method for an async readonly collection.
        /// </summary>
        private static readonly MethodInfo SelectMethod = ((MethodCallExpression)((Expression<Func<IAsyncReadOnlyCollection<Row>, object>>)(rows => rows.Select(row => (object)null))).Body).Method;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceFieldExpression"/> class.
        /// </summary>
        /// <param name="sourceName">
        /// The name of the source.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field.
        /// </param>
        /// <param name="useInternalName">
        /// <c>true</c> to use the internal name of the field, <c>false</c> otherwise.
        /// </param>
        /// <param name="type">
        /// The type of the field.
        /// </param>
        protected internal SourceFieldExpression(string sourceName, string fieldName, bool useInternalName, Type type)
            : base(type)
        {
            this.SourceName = sourceName;
            this.FieldName = fieldName;
            this.UseInternalName = useInternalName;
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// Gets the name of the source.
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// Gets a value indicating whether to use the internal name.
        /// </summary>
        public bool UseInternalName { get; }

        /// <summary>
        /// Creates a method call that gets the value from the specified parameter.
        /// </summary>
        /// <param name="row">
        /// The parameter to get the field from.
        /// </param>
        /// <param name="type">
        /// The type to return (when omitted, the node's type will be returned).
        /// </param>
        /// <returns>
        /// The <see cref="MethodCallExpression"/>.
        /// </returns>
        public MethodCallExpression CreateGetter(ParameterExpression row, [CanBeNull] Type type = null)
            => Expression.Call(row, (this.UseInternalName ? SourceFieldExpression.RowGetByInternalNameMethod : SourceFieldExpression.RowGetMethod).MakeGenericMethod(type ?? this.Type), Expression.Constant(this.SourceName == null ? this.FieldName : $"{this.SourceName}.{this.FieldName}"));

        /// <summary>
        /// Creates a method call that gets the values from the specified parameter for grouping.
        /// </summary>
        /// <param name="rows">
        /// The parameter to get the fields from.
        /// </param>
        /// <returns>
        /// The <see cref="MethodCallExpression"/>.
        /// </returns>
        public MethodCallExpression CreateGroupGetter(ParameterExpression rows)
        {
            var row = Expression.Parameter(typeof(Row), "row");
            var getField = Expression.Lambda<Func<Row, object>>(
                Expression.Call(row, (this.UseInternalName ? SourceFieldExpression.RowGetByInternalNameMethod : SourceFieldExpression.RowGetMethod).MakeGenericMethod(typeof(object)), Expression.Constant(this.SourceName == null ? this.FieldName : $"{this.SourceName}.{this.FieldName}")),
                row);

            return Expression.Call(SourceFieldExpression.SelectMethod, rows, getField);
        }

        /// <summary>
        /// Returns a textual representation of the <see cref="T:System.Linq.Expressions.Expression"/>.
        /// </summary>
        /// <returns>
        /// A textual representation of the <see cref="T:System.Linq.Expressions.Expression"/>.
        /// </returns>
        [NotNull]
        public override string ToString()
        {
            return $"[{this.SourceName}].[{this.FieldName}]";
        }

        /// <summary>
        /// Reduces the node and then calls the visitor delegate on the reduced expression. The method throws an exception
        ///     if the node is not reducible.
        /// </summary>
        /// <returns>
        /// The expression being visited, or an expression which should replace it in the tree.
        /// </returns>
        /// <param name="visitor">
        /// An instance of <see cref="T:System.Func`2"/>.
        /// </param>
        [NotNull]
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }
    }
}