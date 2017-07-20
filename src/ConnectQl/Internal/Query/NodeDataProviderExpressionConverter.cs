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

namespace ConnectQl.Internal.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Sources;
    using ConnectQl.Internal.Ast.Visitors;
    using ConnectQl.Internal.Expressions;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Validation;
    using ConnectQl.Internal.Validation.Operators;
    using ConnectQl.Results;

    /// <summary>
    /// Adds an expression converter to the <see cref="INodeDataProvider"/>.
    /// </summary>
    internal static class NodeDataProviderExpressionConverter
    {
        /// <summary>
        /// Converts the <paramref name="expression"/> to an <see cref="Expression"/>.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="expression">
        /// The expression to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression ConvertToLinqExpression(this INodeDataProvider dataProvider, SqlExpressionBase expression)
        {
            if (expression == null)
            {
                return null;
            }

            new Evaluator(dataProvider).Visit(expression);

            return CleanExpression(dataProvider.GetExpression(expression));
        }

        /// <summary>
        /// The has functions with side effects.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="hasVariableSideEffects">
        /// The has Variable Side Effects.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasSideEffects(this INodeDataProvider data, SqlExpressionBase expression, Func<string, bool> hasVariableSideEffects)
        {
            var checker = new SideEffectsChecker(data, hasVariableSideEffects);

            checker.Visit(expression);

            return checker.HasSideEffects;
        }

        /// <summary>
        /// The has functions with side effects.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="source">
        /// The expression.
        /// </param>
        /// <param name="hasVariableSideEffects">
        /// The has Variable Side Effects.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasSideEffects(this INodeDataProvider data, SourceBase source, Func<string, bool> hasVariableSideEffects)
        {
            var checker = new SideEffectsChecker(data, hasVariableSideEffects);

            checker.Visit(source);

            return checker.HasSideEffects;
        }

        /// <summary>
        /// Cleans the expression by merging <see cref="UnaryExpression"/> and <see cref="SourceFieldExpression"/>, and
        ///     replacing <see cref="BinaryExpression"/>s with <see cref="CompareExpression"/>s.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private static Expression CleanExpression(Expression expression)
        {
            return new GenericVisitor
                       {
                           (GenericVisitor v, BinaryExpression e) =>
                               {
                                   var left = v.Visit(e.Left);
                                   var right = v.Visit(e.Right);

                                   if (!ReferenceEquals(left, e.Left) || !ReferenceEquals(right, e.Right))
                                   {
                                       e = Expression.MakeBinary(e.NodeType, left, right);
                                   }

                                   return ReplaceBinaryCompares(e);
                               },
                           (GenericVisitor v, UnaryExpression e) =>
                               {
                                   var operand = v.Visit(e.Operand);

                                   if (!ReferenceEquals(operand, e.Operand))
                                   {
                                       e = Expression.MakeUnary(e.NodeType, e.Operand, e.Type);
                                   }

                                   if (e.Operand is SourceFieldExpression sourceFieldExpression && e.NodeType == ExpressionType.Convert)
                                   {
                                       return CustomExpression.MakeSourceField(sourceFieldExpression.SourceName, sourceFieldExpression.FieldName, e.Type);
                                   }

                                   return e;
                               },
                       }.Visit(expression);
        }

        /// <summary>
        /// Replaces a binary compare-expression with a <see cref="CompareExpression"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression to check.
        /// </param>
        /// <returns>
        /// The <paramref name="expression"/> or a new <see cref="CompareExpression"/> when the binary expression was a
        ///     comparison.
        /// </returns>
        private static Expression ReplaceBinaryCompares(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return CustomExpression.MakeCompare(expression.NodeType, expression.Left, expression.Right);
            }

            return expression;
        }

        /// <summary>
        /// The evaluator.
        /// </summary>
        private class Evaluator : NodeVisitor
        {
            /// <summary>
            /// The function that marks function results.
            /// </summary>
            private static readonly MethodInfo MarkFunctionResultWithNameMethod = typeof(Evaluator).GetGenericMethod(nameof(MarkFunctionResultWithName), typeof(IExecutionContext), typeof(string), typeof(string), null);

            /// <summary>
            /// The variable counter.
            /// </summary>
            private static int varCounter = 0;

            /// <summary>
            /// The data.
            /// </summary>
            private readonly INodeDataProvider data;

            /// <summary>
            /// Initializes a new instance of the <see cref="Evaluator"/> class.
            /// </summary>
            /// <param name="data">
            /// The data.
            /// </param>
            public Evaluator(INodeDataProvider data)
            {
                this.data = data;
            }

            /// <summary>
            /// Visits a <see cref="BinarySqlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitBinarySqlExpression(BinarySqlExpression node)
            {
                var result = base.VisitBinarySqlExpression(node);

                this.data.SetExpression(node, BinaryOperator.GenerateExpression(this.data.GetExpression(node.First), node.Op, this.data.GetExpression(node.Second)));

                return result;
            }

            /// <summary>
            /// Visits a <see cref="ConstSqlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitConstSqlExpression(ConstSqlExpression node)
            {
                this.data.SetExpression(node, Expression.Constant(node.Value));

                return node;
            }

            /// <summary>
            /// Visits a <see cref="FieldReferenceSqlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitFieldReferenceSqlExpression(FieldReferenceSqlExpression node)
            {
                var result = base.VisitFieldReferenceSqlExpression(node);

                this.data.SetExpression(node, CustomExpression.MakeSourceField(node.Source, node.Name));

                return result;
            }

            /// <summary>
            /// Visits a <see cref="FunctionCallSqlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitFunctionCallSqlExpression(FunctionCallSqlExpression node)
            {
                var function = this.data.GetFunction(node).GetExpression();
                var expression = function.Body;
                var result = base.VisitFunctionCallSqlExpression(node);

                var variables = new ParameterExpression[function.Parameters.Count];
                var statements = new List<Expression>();

                for (var i = 0; i < function.Parameters.Count; i++)
                {
                    var expr = this.data.GetExpression(node.Arguments[i]);

                    if (function.Parameters[i].Type == typeof(IAsyncEnumerable<Row>) &&
                        typeof(IDataSource).GetTypeInfo().IsAssignableFrom(expr.Type.GetTypeInfo()))
                    {
                        expr = this.data.ConvertToRows(expr);
                    }

                    variables[i] = Expression.Parameter(function.Parameters[i].Type, $"var{++varCounter}");
                    statements.Add(Expression.Assign(variables[i], Converter.Convert(expr, function.Parameters[i].Type)));
                    expression = expression.ReplaceParameter(function.Parameters[i], variables[i]);
                }

                if (expression.Type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDataAccess)))
                {
                    Expression<Func<string, object[], string>> getDisplayName = (name, args) =>
                        $"{name.ToUpperInvariant()}({string.Join(", ", args.Select(a => a is IAsyncEnumerable ? ((IAsyncEnumerable)a).GetElementType().Name + "[]" : a is string ? $"'{a}'" : a ?? "NULL"))})";

                    var displayName = getDisplayName.Body.ReplaceParameter(getDisplayName.Parameters[0], Expression.Constant(node.Name)).ReplaceParameter(getDisplayName.Parameters[1], Expression.NewArrayInit(typeof(object), variables.Select(v => Expression.Convert(v, typeof(object))).ToArray<Expression>()));

                    expression = Expression.Call(MarkFunctionResultWithNameMethod.MakeGenericMethod(expression.Type), CustomExpression.ExecutionContext(), Expression.Constant(node.Name), displayName, expression);
                }

                statements.Add(expression);

                Expression block = Expression.Block(variables, statements);

                if (block.Type.IsConstructedGenericType && block.Type.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    block = TaskExpression.Task(block);
                }

                this.data.SetExpression(node, block);

                return result;
            }

            /// <summary>
            /// Visits a <see cref="UnarySqlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitUnarySqlExpression(UnarySqlExpression node)
            {
                var result = base.VisitUnarySqlExpression(node);

                this.data.SetExpression(node, UnaryOperator.GenerateExpression(node.Op, this.data.GetExpression(node.Expression)));

                return result;
            }

            /// <summary>
            /// Visits a <see cref="VariableSqlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitVariableSqlExpression(VariableSqlExpression node)
            {
                var result = base.VisitVariableSqlExpression(node);
                var getVariable = typeof(IExecutionContext).GetRuntimeMethod("GetVariable", new[] { typeof(string), }).MakeGenericMethod(this.data.GetType(node).SimplifiedType);

                this.data.SetExpression(node, Expression.Call(CustomExpression.ExecutionContext(), getVariable, Expression.Constant(node.Name)));

                return result;
            }

            /// <summary>
            /// Marks the function result with the specified name name.
            /// </summary>
            /// <param name="context">
            /// The context.
            /// </param>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <param name="displayName">
            /// The display name.
            /// </param>
            /// <param name="function">
            /// The function.
            /// </param>
            /// <typeparam name="TFunctionResult">
            /// The type of the function result.
            /// </typeparam>
            /// <returns>
            /// The <typeparamref name="TFunctionResult"/>.
            /// </returns>
            private static TFunctionResult MarkFunctionResultWithName<TFunctionResult>(IExecutionContext context, string name, string displayName, TFunctionResult function)
                where TFunctionResult : IDataAccess
            {
                ((IInternalExecutionContext)context).SetFunctionName(function, name);
                ((IInternalExecutionContext)context).SetDisplayName(function, displayName);

                return function;
            }
        }

        /// <summary>
        /// The side effects checker.
        /// </summary>
        private class SideEffectsChecker : NodeVisitor
        {
            /// <summary>
            /// The data.
            /// </summary>
            private readonly INodeDataProvider data;

            /// <summary>
            /// The has variable side effects.
            /// </summary>
            private readonly Func<string, bool> hasVariableSideEffects;

            /// <summary>
            /// Initializes a new instance of the <see cref="SideEffectsChecker"/> class.
            /// </summary>
            /// <param name="data">
            /// The data.
            /// </param>
            /// <param name="hasVariableSideEffects">
            /// Checks if the variable has side effects.
            /// </param>
            public SideEffectsChecker(INodeDataProvider data, Func<string, bool> hasVariableSideEffects)
            {
                this.data = data;
                this.hasVariableSideEffects = hasVariableSideEffects;
            }

            /// <summary>
            /// Gets a value indicating whether the expression has side effects.
            /// </summary>
            public bool HasSideEffects { get; private set; }

            /// <summary>
            /// Visits a <see cref="FunctionCallSqlExpression"/> expression.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitFunctionCallSqlExpression(FunctionCallSqlExpression node)
            {
                this.HasSideEffects |= this.data.GetFunction(node)?.HasSideEffects ?? true;

                return this.HasSideEffects ? node : base.VisitFunctionCallSqlExpression(node);
            }

            /// <summary>
            /// Visits a <see cref="VariableSqlExpression"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitVariableSqlExpression(VariableSqlExpression node)
            {
                this.HasSideEffects |= this.hasVariableSideEffects(node.Name);

                return base.VisitVariableSqlExpression(node);
            }

            /// <summary>
            /// Visits a <see cref="VariableSource"/>.
            /// </summary>
            /// <param name="node">
            /// The node.
            /// </param>
            /// <returns>
            /// The node, or a new version of the node.
            /// </returns>
            protected internal override Node VisitVariableSource(VariableSource node)
            {
                this.HasSideEffects |= this.hasVariableSideEffects(node.Variable);

                return base.VisitVariableSource(node);
            }
        }
    }
}