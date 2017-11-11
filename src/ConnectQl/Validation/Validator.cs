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

namespace ConnectQl.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions.Helpers;
    using ConnectQl.ExtensionMethods;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Parser.Ast;
    using ConnectQl.Parser.Ast.Expressions;
    using ConnectQl.Parser.Ast.Sources;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Parser.Ast.Targets;
    using ConnectQl.Parser.Ast.Visitors;
    using ConnectQl.Plugins;
    using ConnectQl.Resources;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The validator.
    /// </summary>
    internal class Validator : NodeVisitor
    {
        /// <summary>
        /// The default functions.
        /// </summary>
        private static readonly DefaultFunctions DefaultFunctions = new DefaultFunctions();

        /// <summary>
        /// The context.
        /// </summary>
        private readonly IValidationContext context;

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        private ValidationScope scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private Validator([NotNull] IValidationContext context)
        {
            this.context = context;
            this.scope = new ValidationScope(context);
            this.scope.EnablePlugin(DefaultFunctions);
        }

        /// <summary>
        /// Validates the specified <see cref="Node"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the node to validate.
        /// </typeparam>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="node">
        /// The node to validate.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        [CanBeNull]
        public static T Validate<T>([NotNull] IValidationContext context, T node)
            where T : Node
        {
            return new Validator(context).Visit(node);
        }

        /// <summary>
        /// Validates the specified <see cref="Node"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the node to validate.
        /// </typeparam>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="node">
        /// The node to validate.
        /// </param>
        /// <param name="functions">
        /// The functions.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        [CanBeNull]
        internal static T Validate<T>([NotNull] IValidationContext context, T node, [NotNull] out ILookup<string, IFunctionDescriptor> functions)
            where T : Node
        {
            var validator = new Validator(context);
            var result = validator.Visit(node);

            functions = ((IFunctionDictionary)validator.scope.Functions).Dictionary.ToLookup(d => d.Key.Split('\'')[0].ToLowerInvariant(), d => d.Value);

            return result;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.AliasedConnectQlExpression"/>.
        /// Adds the alias to the current scope (or a default alias if none exists). Returns a new node if an alias was created.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitAliasedSqlExpression(AliasedConnectQlExpression node)
        {
            if (node.Expression is WildcardConnectQlExpression)
            {
                return this.ValidateChildren(node);
            }

            var alias = this.scope.AddAlias(node.Alias ?? (node.Expression as FieldReferenceConnectQlExpression)?.Name);

            if (node.Alias != alias)
            {
                node = this.context.NodeData.CopyValues(node, new AliasedConnectQlExpression(node.Expression, alias));
            }

            return this.ValidateChildren(node);
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.BinaryConnectQlExpression"/> expression.
        /// Infers the type of the binary expression and stores it in the node data. Checks if children are in a grouping,
        /// if so, marks this node as a group too.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitBinarySqlExpression(BinaryConnectQlExpression node)
        {
            node = this.ValidateChildren(node);

            var resultType = new TypeDescriptor(OperatorHelper.InferType(node.Op, this.context.NodeData.GetType(node.First).SimplifiedType, this.context.NodeData.GetType(node.Second).SimplifiedType, error => this.AddError(node, error)));

            this.context.NodeData.SetType(node, resultType);

            if (this.scope.IsGroupByExpression(node))
            {
                this.context.NodeData.SetScope(node, NodeScope.Group);
            }

            return node;
        }

        /// <summary>
        /// Visits a <see cref="ConstConnectQlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitConstSqlExpression(ConstConnectQlExpression node)
        {
            this.context.NodeData.SetType(node, new TypeDescriptor(node.Value?.GetType() ?? typeof(object)));
            this.context.NodeData.SetScope(node, this.scope.IsGroupByExpression(node) ? NodeScope.Group : NodeScope.Constant);

            return node;
        }

        /// <summary>
        /// Visits a <see cref="FieldReferenceConnectQlExpression"/> expression.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [CanBeNull]
        protected internal override Node VisitFieldReferenceSqlExpression(FieldReferenceConnectQlExpression node)
        {
            var replacer = this.context.NodeData.GetFieldReplacer(node);

            if (replacer != null)
            {
                return this.Visit(replacer);
            }
            
            this.context.NodeData.SetType(node, new TypeDescriptor(typeof(object)));
            this.context.NodeData.SetScope(node, this.scope.IsGroupByExpression(node) ? NodeScope.Group : NodeScope.Row);

            if (node.Source == null)
            {
                this.AddError(node, string.Format(Messages.FieldMustHaveASource, node.Name));

                return node;
            }

            var source = this.scope.GetSource(node.Source);

            if (source == null)
            {
                this.AddError(node, string.Format(Messages.SourceReferencedBeforeDeclared, node.Source));
            }

            this.context.NodeData.Set(node, "Source", source);

            return node;
        }

        /// <summary>
        /// The visit function call async.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected internal override Node VisitFunctionCallSqlExpression(FunctionCallConnectQlExpression node)
        {
            var function = this.scope.GetFunction(node.Name, node.Arguments);

            if (function == null)
            {
                this.AddError(node, string.Format(Messages.FunctionNotFound, node.GetDisplay()));

                this.context.NodeData.SetType(node, new TypeDescriptor(typeof(object)));
                this.context.NodeData.SetFunction(node, this.scope.GetFunction(node.Name, node.Arguments));
                this.context.NodeData.SetScope(node, NodeScope.Constant);

                return node;
            }

            this.context.NodeData.SetType(node, function.ReturnType);
            this.context.NodeData.SetFunction(node, this.scope.GetFunction(node.Name, node.Arguments));

            var result = this.ValidateChildren(this.ReplaceEnumArguments(node));

            for (var i = 0; i < function.Arguments.Count; i++)
            {
                ConversionHelper.ValidateConversion(node.Arguments[i], this.context.NodeData.GetType(node.Arguments[i]).SimplifiedType, function.Arguments[i].Type.SimplifiedType);
            }

            this.context.NodeData.SetScope(node, this.scope.IsGroupByExpression(node) || (node.Arguments.All(a => this.context.NodeData.GetScope(a) != NodeScope.Row) && node.Arguments.Any(a => this.context.NodeData.GetScope(a) == NodeScope.Group)) ? NodeScope.Group : NodeScope.Row);

            if (function.Arguments.Count > 0 && function.Arguments.Any(argument => argument.Type.Interfaces.Any(i => i.HasInterface(typeof(IAsyncEnumerable<>)))))
            {
                this.context.NodeData.SetScope(result, NodeScope.Group);
            }

            return result;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Sources.FunctionSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitFunctionSource(FunctionSource node)
        {
            if (node.Alias != null)
            {
                if (this.scope.GetSource(node.Alias) != null)
                {
                    this.AddError(node, string.Format(Messages.AliasAlreadyUsed, node.Alias));
                }
                else
                {
                    this.scope.AddSource(node.Alias, node);
                }
            }

            var result = this.ValidateChildren(node);
            var function = this.context.NodeData.GetFunction(result.Function);

            if (!function?.ReturnType.Interfaces.Contains(typeof(IDataSource)) ?? false)
            {
                this.AddError(node, string.Format(Messages.FunctionIsNoDataSource, node.Function.Name.ToUpperInvariant()));
            }

            return result;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Targets.FunctionTarget"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitFunctionTarget(FunctionTarget node)
        {
            var result = this.ValidateChildren(node);
            var function = this.context.NodeData.GetFunction(result.Function);

            if (!function?.ReturnType.Interfaces.Contains(typeof(IDataTarget)) ?? false)
            {
                this.AddError(node, string.Format(Messages.FunctionIsNoDataTarget, node.Function.Name.ToUpperInvariant()));
            }

            return result;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.ImportPluginStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitImportPluginStatement(ImportPluginStatement node)
        {
            if (this.scope.IsPluginEnabled(node.Plugin))
            {
                this.AddWarning(node, string.Format(Messages.PluginAlreadyEnabled, node.Plugin));
            }

            if (!this.scope.EnablePlugin(node.Plugin) && !this.scope.IsLoadingPlugins)
            {
                var plugins = this.scope.GetAvailablePlugins().ToArray();
                var availablePlugins = plugins.Length == 0 ? string.Empty : " " + string.Format(Messages.AvailablePlugins, string.Join(", ", plugins.Where(p => !string.Equals(p, "DefaultFunctions"))));
                this.AddError(node, $"Plugin {node.Plugin} was not found.{availablePlugins}");
            }

            return base.VisitImportPluginStatement(node);
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Sources.JoinSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitJoinSource(JoinSource node)
        {
            return this.ValidateChildren(node);
        }

        /// <summary>
        /// Visits the node and ensures the result is of type <typeparamref name="T"/>. When node is <c>null</c>, returns
        ///     <c>null</c>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the result.
        /// </typeparam>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <typeparamref name="T"/>.
        /// </returns>
        protected internal override T Visit<T>(T node)
        {
            var result = base.Visit(node);

            if (!object.ReferenceEquals(result, node))
            {
                this.context.NodeData.CopyValues(node, result);
            }

            return result;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Statements.SelectFromStatement"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitSelectFromStatement(SelectFromStatement node)
        {
            using (this.EnterScope())
            {
                var source = this.Visit(node.Source);
                var where = this.Visit(node.Where);
                var groupings = this.Visit(node.Groupings);

                this.scope.AddGroupings(groupings);

                var having = this.Visit(node.Having);
                var expressions = this.Visit(node.Expressions);

                var aliasOrders = expressions.Join(
                    node.Orders.Select(
                        o => o.Expression as FieldReferenceConnectQlExpression).Where(fr => fr != null && fr.Source == null),
                        e => e.Alias,
                        fr => fr.Name,
                        (expression, field) => new
                                                   {
                                                       expression,
                                                       field,
                                                   });

                foreach (var aliasOrder in aliasOrders)
                {
                    this.context.NodeData.SetFieldReplacer(aliasOrder.field, aliasOrder.expression.Expression);
                }

                var orders = this.Visit(node.Orders);

                foreach (var invalidOrderBy in orders.Select(o => o.Expression as FieldReferenceConnectQlExpression).Where(fr => fr != null && fr.Source == null))
                {
                    this.AddError(invalidOrderBy, string.Format(Messages.AliasNotDefined, invalidOrderBy.Name));
                }

                var hasGroupings = groupings.Any();

                if (hasGroupings)
                {
                    if (having != null && this.context.NodeData.GetScope(having) == NodeScope.Row)
                    {
                        this.AddError(having, string.Format(Messages.MissingAggregateFunction, node.Having));
                    }

                    foreach (var e in expressions.Where(e => this.context.NodeData.GetScope(e) == NodeScope.Row))
                    {
                        this.AddError(e, string.Format(Messages.MissingAggregateFunction, e));
                    }
                }
                else if (having != null)
                {
                    this.AddError(having, Messages.HavingNotAllowed);
                }

                return expressions != node.Expressions || source != node.Source || where != node.Where ||
                       groupings != node.Groupings || having != node.Having || orders != node.Orders
                           ? this.context.NodeData.CopyValues(node, new SelectFromStatement(expressions, source, where, groupings, having, orders))
                           : node;
            }
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Sources.SelectSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitSelectSource(SelectSource node)
        {
            this.scope.AddSource(node.Alias, node);

            return base.VisitSelectSource(node);
        }

        /// <summary>
        /// The visit trigger.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        [NotNull]
        protected internal override Node VisitTrigger(Trigger node)
        {
            var result = (Trigger)node.VisitChildren(this);
            var function = this.context.NodeData.GetFunction(result.Function);

            if (!function?.ReturnType.Interfaces.Contains(typeof(ITrigger)) ?? false)
            {
                this.AddError(node, string.Format(Messages.FunctionIsNoTrigger, node.Function.Name.ToUpperInvariant()));
            }

            return result;
        }

        /// <summary>
        /// The visit unary async.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [NotNull]
        protected internal override Node VisitUnarySqlExpression(UnaryConnectQlExpression node)
        {
            node = this.ValidateChildren(node);

            this.context.NodeData.SetType(node, new TypeDescriptor(OperatorHelper.InferType(node.Op, this.context.NodeData.GetType(node.Expression).SimplifiedType)));

            if (this.scope.IsGroupByExpression(node))
            {
                this.context.NodeData.SetScope(node, NodeScope.Group);
            }

            return node;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.VariableDeclaration"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitVariableDeclaration(VariableDeclaration node)
        {
            node = this.ValidateChildren(node);

            this.scope.AddVariable(node.Name, node.Expression == null ? new TypeDescriptor(typeof(object)) : this.context.NodeData.GetType(node.Expression));

            return node;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Sources.VariableSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitVariableSource(VariableSource node)
        {
            node = this.ValidateChildren(node);

            var variableType = this.scope.GetVariableType(node.Variable);

            if (variableType == null)
            {
                this.AddError(node, string.Format(Messages.UndeclaredVariable, node.Variable));
            }
            else if (!variableType.Interfaces.Contains(typeof(IDataSource)))
            {
                this.AddError(node, string.Format(Messages.VariableIsNoDataSource, node.Variable));
            }

            if (this.scope.GetSource(node.Alias) != null)
            {
                this.AddError(node, string.Format(Messages.AliasAlreadyUsed, node.Alias));
            }
            else
            {
                this.scope.AddSource(node.Alias, node);
            }

            this.context.NodeData.SetType(node, variableType);
            this.context.NodeData.SetScope(node, NodeScope.Constant);

            return node;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.VariableConnectQlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitVariableSqlExpression(VariableConnectQlExpression node)
        {
            node = this.ValidateChildren(node);

            var variableType = this.scope.GetVariableType(node.Name);

            if (variableType == null)
            {
                this.AddError(node, string.Format(Messages.UndeclaredVariable, node.Name));
            }

            this.context.NodeData.SetType(node, variableType);
            this.context.NodeData.SetScope(node, NodeScope.Constant);

            return node;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Targets.FunctionTarget"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitVariableTarget(VariableTarget node)
        {
            node = this.ValidateChildren(node);

            var variableType = this.scope.GetVariableType(node.Variable);

            if (variableType == null)
            {
                this.AddError(node, string.Format(Messages.UndeclaredVariable, node.Variable));
            }
            else if (!variableType.Interfaces.Contains(typeof(IDataTarget)))
            {
                this.AddError(node, string.Format(Messages.VariableIsNoDataTarget, node.Variable));
            }

            this.context.NodeData.SetType(node, variableType);
            this.context.NodeData.SetScope(node, NodeScope.Constant);

            return node;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.WildcardConnectQlExpression"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        [NotNull]
        protected internal override Node VisitWildCardSqlExpression(WildcardConnectQlExpression node)
        {
            if (node.Source != null)
            {
                var source = this.scope.GetSource(node.Source);

                if (source == null)
                {
                    this.AddError(node, string.Format(Messages.SourceReferencedBeforeDeclared, node.Source));
                }

                this.context.NodeData.Set(node, "Source", source);
            }

            this.context.NodeData.SetScope(node, NodeScope.Row);

            return node;
        }

        /// <summary>
        /// Enters a new scope.
        /// </summary>
        /// <returns>
        /// A <see cref="IDisposable"/>, that will exit the scope when disposed.
        /// </returns>
        [NotNull]
        private IDisposable EnterScope()
        {
            var scope = this.scope;
            this.scope = this.scope.CreateSubScope();

            return new ActionOnDispose(() => this.scope = scope);
        }

        /// <summary>
        /// Adds an error for the specified node.
        /// </summary>
        /// <param name="node">
        /// The node connected to this error.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private void AddError(Node node, string message)
        {
            // ReSharper disable StyleCop.SA1126
            this.context.NodeData.TryGet(node, "Context", out IParserContext parserContext);
            this.context.Messages.AddError(parserContext?.Start ?? new Position(), parserContext?.End ?? new Position(), message);

            // ReSharper restore StyleCop.SA1126
        }

        /// <summary>
        /// Adds a warning for the specified node.
        /// </summary>
        /// <param name="node">
        /// The node connected to this error.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private void AddWarning(Node node, string message)
        {
            // ReSharper disable StyleCop.SA1126
            this.context.NodeData.TryGet(node, "Context", out IParserContext parserContext);
            this.context.Messages.AddWarning(parserContext?.Start ?? new Position(), parserContext?.End ?? new Position(), message);

            // ReSharper restore StyleCop.SA1126
        }

        /// <summary>
        /// Replaces strings and field references to enum values.
        /// </summary>
        /// <param name="node">
        /// The node to replace values with.
        /// </param>
        /// <returns>
        /// The node with replaced.
        /// </returns>
        private Node ReplaceEnumArguments([NotNull] FunctionCallConnectQlExpression node)
        {
            var arguments = node.Arguments;
            var function = this.context.NodeData.GetFunction(node);

            for (var i = 0; i < function.Arguments.Count; i++)
            {
                var arg = arguments[i];
                var param = function.Arguments[i].Type.SimplifiedType;

                if (!param.GetTypeInfo().IsEnum)
                {
                    continue;
                }

                var constExpr = arg as ConstConnectQlExpression;
                var fieldExpr = arg as FieldReferenceConnectQlExpression;

                ConstConnectQlExpression enumExpr;

                if (fieldExpr != null && fieldExpr.Source == null)
                {
                    enumExpr = new ConstConnectQlExpression(Enum.Parse(param, fieldExpr.Name, true));
                }
                else if (constExpr?.Value is int)
                {
                    enumExpr = new ConstConnectQlExpression(Enum.ToObject(param, constExpr.Value));
                }
                else if (constExpr?.Value is string)
                {
                    enumExpr = new ConstConnectQlExpression(Enum.Parse(param, (string)constExpr.Value, true));
                }
                else
                {
                    continue;
                }

                arguments = new ReadOnlyCollection<ConnectQlExpressionBase>(
                    new List<ConnectQlExpressionBase>(arguments)
                        {
                            [i] = this.context.NodeData.CopyValues(arguments[i], enumExpr),
                        });
            }

            return arguments != node.Arguments ? this.context.NodeData.CopyValues(node, new FunctionCallConnectQlExpression(node.Name, arguments)) : node;
        }

        /// <summary>
        /// The validate children.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <typeparam name="T">
        /// The type of the node.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private T ValidateChildren<T>([NotNull] T node)
            where T : Node
        {
            var result = (T)node.VisitChildren(this);
            var baseExpression = node as ConnectQlExpressionBase;

            if (baseExpression == null)
            {
                return result;
            }

            var scopes = result.Children.OfType<ConnectQlExpressionBase>().Select(expression =>
                    new
                        {
                            Expression = expression,
                            Scope = this.context.NodeData.GetScope(expression),
                        })
                .ToArray();

            if (scopes.Any(s => s.Expression == null || s.Scope == NodeScope.Error || s.Scope == NodeScope.Initial))
            {
                this.AddError(node, Messages.InvalidScope);
            }

            if (scopes.Any(s => s.Scope == NodeScope.Group))
            {
                if (scopes.Any(s => s.Scope == NodeScope.Row))
                {
                    foreach (var child in scopes.Where(s => s.Scope == NodeScope.Row))
                    {
                        this.AddError(child.Expression, string.Format(Messages.MissingAggregateFunction, child.Expression));
                    }
                }
                else
                {
                    this.context.NodeData.SetScope(baseExpression, NodeScope.Group);
                }
            }
            else if (scopes.Any(s => s.Scope == NodeScope.Row))
            {
                this.context.NodeData.SetScope(baseExpression, NodeScope.Row);
            }
            else
            {
                this.context.NodeData.SetScope(baseExpression, NodeScope.Constant);
            }

            return result;
        }
    }
}