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

namespace ConnectQl.Internal.Validation
{
    using System.Linq.Expressions;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Interfaces;

    /// <summary>
    /// The node data provider extensions.
    /// </summary>
    internal static class NodeDataProviderExtensions
    {
        /// <summary>
        /// Gets the alias connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The alias or <c>null</c> when no alias is available.
        /// </returns>
        public static string GetAlias(this INodeDataProvider dataProvider, Node node)
        {
            return dataProvider.TryGet(node, "Alias", out string result) ? result : null;
        }

        /// <summary>
        /// Gets the expression connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        public static Expression GetExpression(this INodeDataProvider dataProvider, SqlExpressionBase node)
        {
            return dataProvider.Get<Expression>(node, "Expression");
        }

        /// <summary>
        /// Gets the source factory .
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The source factory.
        /// </returns>
        public static Expression GetFactoryExpression(this INodeDataProvider dataProvider, Node node)
        {
            return dataProvider.Get<Expression>(node, "FactoryExpression");
        }

        /// <summary>
        /// Gets the expression that will replace a <see cref="FieldReferenceSqlExpression"/> in an ORDER BY clause.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="SqlExpressionBase"/>.
        /// </returns>
        public static SqlExpressionBase GetFieldReplacer(this INodeDataProvider dataProvider, FieldReferenceSqlExpression field)
        {
            return dataProvider.TryGet(field, "FieldReplacer", out SqlExpressionBase result) ? result : null;
        }

        /// <summary>
        /// Gets the function connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The function.
        /// </returns>
        public static IFunctionDescriptor GetFunction(this INodeDataProvider dataProvider, Node node)
        {
            return dataProvider.Get<IFunctionDescriptor>(node, "Function");
        }

        /// <summary>
        /// Gets the scope connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The scope.
        /// </returns>
        public static NodeScope GetScope(this INodeDataProvider dataProvider, Node node)
        {
            return dataProvider.TryGet(node, "Scope", out NodeScope result) ? result : NodeScope.Initial;
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="ITypeDescriptor"/>.
        /// </returns>
        public static ITypeDescriptor GetType(this INodeDataProvider dataProvider, Node node)
        {
            return node != null && dataProvider.TryGet(node, "Type", out ITypeDescriptor result) ? result ?? new TypeDescriptor(typeof(object)) : new TypeDescriptor(typeof(object));
        }

        /// <summary>
        /// Checks if the node is a group function.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// <c>true</c> if the node is a group function, <c>false</c> otherwise.
        /// </returns>
        public static bool IsGroupFunction(this INodeDataProvider dataProvider, Node node)
        {
            return dataProvider.TryGet(node, "IsGroupFunction", out object isGroupFunction) && (bool)isGroupFunction;
        }

        /// <summary>
        /// Marks the node as a group function.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        public static void MarkAsGroupFunction(this INodeDataProvider dataProvider, Node node)
        {
            dataProvider.Set(node, "IsGroupFunction", true);
        }

        /// <summary>
        /// Sets the alias connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        public static void SetAlias(this INodeDataProvider dataProvider, Node node, string alias)
        {
            dataProvider.Set(node, "Alias", alias);
        }

        /// <summary>
        /// Sets the expression connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public static void SetExpression(this INodeDataProvider dataProvider, SqlExpressionBase node, Expression expression)
        {
            dataProvider.Set(node, "Expression", expression);
        }

        /// <summary>
        /// Sets the source factory .
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        public static void SetFactoryExpression(this INodeDataProvider dataProvider, Node node, Expression factory)
        {
            dataProvider.Set(node, "FactoryExpression", factory);
        }

        /// <summary>
        /// Sets the expression that will replace a <see cref="FieldReferenceSqlExpression"/> in an ORDER BY clause.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public static void SetFieldReplacer(this INodeDataProvider dataProvider, FieldReferenceSqlExpression field, SqlExpressionBase expression)
        {
            dataProvider.Set(field, "FieldReplacer", expression);
        }

        /// <summary>
        /// Sets the function connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        public static void SetFunction(this INodeDataProvider dataProvider, Node node, IFunctionDescriptor function)
        {
            dataProvider.Set(node, "Function", function);
        }

        /// <summary>
        /// Sets the scope connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        public static void SetScope(this INodeDataProvider dataProvider, Node node, NodeScope scope)
        {
            dataProvider.Set(node, "Scope", scope);
        }

        /// <summary>
        /// Sets the type of the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public static void SetType(this INodeDataProvider dataProvider, Node node, ITypeDescriptor type)
        {
            dataProvider.Set(node, "Type", type);
        }

        /// <summary>
        /// Gets the query connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="IQuery"/>.
        /// </returns>
        internal static IQueryPlan GetQueryPlan(this INodeDataProvider dataProvider, Node node)
        {
            return dataProvider.TryGet(node, "Query", out IQueryPlan result) ? result : null;
        }

        /// <summary>
        /// Sets the query connected to the node.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        internal static void SetQueryPlan(this INodeDataProvider dataProvider, Node node, IQueryPlan query)
        {
            dataProvider.Set(node, "Query", query);
        }
    }
}