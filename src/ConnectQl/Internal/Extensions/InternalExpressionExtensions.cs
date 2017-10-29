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

namespace ConnectQl.Internal.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.DataSources;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// Internal extensions for expressions.
    /// </summary>
    internal static class InternalExpressionExtensions
    {
        /// <summary>
        /// Gets the fields of the <paramref name="dataSource"/> that are used in the expression.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="dataSource">
        /// The data source.
        /// </param>
        /// <returns>
        /// An enumerable of fields that were used.
        /// </returns>
        [NotNull]
        public static IEnumerable<IField> GetDataSourceFields(this Expression expression, DataSource dataSource)
        {
            return expression.GetFields().Where(field => dataSource.Aliases.Contains(field.SourceAlias));
        }

        /// <summary>
        /// Gets a function that takes two rows and returns true if the rows should be joined.
        /// </summary>
        /// <param name="expression">
        /// The expression to create the function from.
        /// </param>
        /// <param name="leftSource">
        /// The source that will be the first argument in the returned function.
        /// </param>
        /// <returns>
        /// A function that takes two rows and returns true if the rows should be joined.
        /// </returns>
        public static Func<Row, Row, bool> GetJoinFunction([CanBeNull] this Expression expression, DataSource leftSource)
        {
            if (expression == null)
            {
                return null;
            }

            var aliases = leftSource.Aliases;
            var leftRow = Expression.Parameter(typeof(Row));
            var rightRow = Expression.Parameter(typeof(Row));

            var filterExpression = new GenericVisitor
                                       {
                                           (SourceFieldExpression node) => node.CreateGetter(aliases.Contains(node.SourceName) ? leftRow : rightRow),
                                       }.Visit(expression);

            return Expression.Lambda<Func<Row, Row, bool>>(filterExpression, leftRow, rightRow).Compile();
        }

        /// <summary>
        /// Moves field expressions in comparisons to the left.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the expression.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="source">
        /// The source for which the fields should be moved to the left.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static T MoveFieldsToTheLeft<T>(this T expression, DataSource source)
            where T : Expression
        {
            return (T)new GenericVisitor
                          {
                              (BinaryExpression node) => !node.Right.ContainsField(source) ? null : node.SwapOperandsForComparison(),
                          }.Visit(expression);
        }

        /// <summary>
        /// Creates a new <see cref="Expression"/> that only contains comparisons with fields from the specified source.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="source">
        /// The source to filter by.
        /// </param>
        /// <returns>
        /// A new <see cref="Expression"/> without the parts containing fields that are not in the source.
        /// </returns>
        [CanBeNull]
        public static Expression RemoveAllPartsThatAreNotInSource(this Expression expression, [NotNull] DataSource source)
        {
            var hasNonSourceFields = false;
            var aliases = source.Aliases;

            var filter = new GenericVisitor
                             {
                                 (SourceFieldExpression node) =>
                                     {
                                         //// Check if the field is in the aliases of the source.
                                         hasNonSourceFields |= !aliases.Contains(node.SourceName);

                                         //// We stop looking for fields, there is nothing below a field node.
                                         return node;
                                     },
                                 (GenericVisitor visitor, BinaryExpression node) =>
                                     {
                                         if (node.NodeType != ExpressionType.And && node.NodeType != ExpressionType.AndAlso)
                                         {
                                             return null;
                                         }

                                         hasNonSourceFields = false;
                                         var left = visitor.Visit(node.Left);
                                         var leftHasNonSourceFields = hasNonSourceFields;

                                         hasNonSourceFields = false;
                                         var right = visitor.Visit(node.Right);
                                         var rightHasNonSourceFields = hasNonSourceFields;

                                         hasNonSourceFields = false;

                                         return leftHasNonSourceFields
                                                    ? rightHasNonSourceFields
                                                          ? Expression.Constant(true)
                                                          : right
                                                    : rightHasNonSourceFields
                                                        ? left
                                                        : node.Left == left && node.Right == right
                                                            ? node
                                                            : left is ConstantExpression
                                                                ? (right is ConstantExpression
                                                                       ? Expression.Constant(true)
                                                                       : right)
                                                                : (right is ConstantExpression
                                                                       ? left
                                                                       : Expression.MakeBinary(node.NodeType, left, right));
                                     },
                             }.Visit(expression);

            return hasNonSourceFields ? null : filter?.NodeType == ExpressionType.Constant ? null : filter;
        }
    }
}
