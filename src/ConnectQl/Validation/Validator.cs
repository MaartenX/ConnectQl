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
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Plugins;
    using ConnectQl.Results;
    using ConnectQl.ExtensionMethods;

    using JetBrains.Annotations;
    using ConnectQl.Parser.Ast;
    using ConnectQl.Parser.Ast.Expressions;
    using ConnectQl.Parser.Ast.Sources;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Parser.Ast.Targets;
    using ConnectQl.Parser.Ast.Visitors;

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
        /// Initializes a new instance of the <see cref="Validator"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private Validator([NotNull] IValidationContext context)
        {
            this.context = context;
            this.Scope = new ValidationScope(context);
            this.Scope.EnablePlugin(Validator.DefaultFunctions);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        private INodeDataProvider Data => this.context.NodeData;

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        private ValidationScope Scope { get; set; }

        /// <summary>
        /// Validates the specified <see cref="ConnectQl.Parser.Ast.Node"/>.
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
        /// The <see cref="ConnectQl.Parser.Ast.Node"/>.
        /// </returns>
        [CanBeNull]
        public static T Validate<T>([NotNull] IValidationContext context, T node)
            where T : Node
        {
            var validator = new Validator(context);

            return validator.Visit(node);
        }

        /// <summary>
        /// Validates the specified <see cref="ConnectQl.Parser.Ast.Node"/>.
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
        /// The <see cref="ConnectQl.Parser.Ast.Node"/>.
        /// </returns>
        [CanBeNull]
        internal static T Validate<T>([NotNull] IValidationContext context, T node, [NotNull] out ILookup<string, IFunctionDescriptor> functions)
            where T : Node
        {
            var validator = new Validator(context);
            var result = validator.Visit(node);

            functions = ((IFunctionDictionary)validator.Scope.Functions).Dictionary.ToLookup(d => d.Key.Split('\'')[0].ToLowerInvariant(), d => d.Value);

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

            var alias = this.Scope.AddAlias(node.Alias ?? (node.Expression as FieldReferenceConnectQlExpression)?.Name);

            if (node.Alias != alias)
            {
                node = this.Data.CopyValues(node, new AliasedConnectQlExpression(node.Expression, alias));
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

            this.Data.SetType(node, new TypeDescriptor(OperatorHelper.InferType(node.Op, this.Data.GetType(node.First).SimplifiedType, this.Data.GetType(node.Second).SimplifiedType, error => this.AddError(node, error))));

            if (this.Scope.IsGroupByExpression(node))
            {
                this.Data.SetScope(node, NodeScope.Group);
            }

            return node;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.ConstConnectQlExpression"/> expression.
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
            this.Data.SetType(node, new TypeDescriptor(node.Value?.GetType() ?? typeof(object)));
            this.Data.SetScope(node, this.Scope.IsGroupByExpression(node) ? NodeScope.Group : NodeScope.Constant);

            return node;
        }

        /// <summary>
        /// Visits a <see cref="ConnectQl.Parser.Ast.Expressions.FieldReferenceConnectQlExpression"/> expression.
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
            var replacer = this.Data.GetFieldReplacer(node);

            if (replacer != null)
            {
                return this.Visit(replacer);
            }

            this.Data.SetType(node, new TypeDescriptor(typeof(object)));
            this.Data.SetScope(node, this.Scope.IsGroupByExpression(node) ? NodeScope.Group : NodeScope.Row);

            if (node.Source == null)
            {
                this.AddError(node, $"Field {node.Name} must have a source.");

                return node;
            }

            var source = this.Scope.GetSource(node.Source);

            if (source == null)
            {
                this.AddError(node, $"Source {node.Source} is referenced before it is declared.");
            }

            this.Data.Set(node, "Source", source);

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
            var function = this.Scope.GetFunction(node.Name, node.Arguments);

            if (function == null)
            {
                this.AddError(node, $"Cannot find function {NodeExtensions.GetDisplay(node)}.");

                this.Data.SetType(node, new TypeDescriptor(typeof(object)));
                this.Data.SetFunction(node, this.Scope.GetFunction(node.Name, node.Arguments));
                this.Data.SetScope(node, NodeScope.Constant);

                return node;
            }

            this.Data.SetType(node, function.ReturnType);
            this.Data.SetFunction(node, this.Scope.GetFunction(node.Name, node.Arguments));

            var result = this.ValidateChildren(this.ReplaceEnumArguments(node));

            for (var i = 0; i < function.Arguments.Count; i++)
            {
                ConversionHelper.ValidateConversion(node.Arguments[i], this.Data.GetType(node.Arguments[i]).SimplifiedType, function.Arguments[i].Type.SimplifiedType);
            }

            this.Data.SetScope(node, this.Scope.IsGroupByExpression(node) || (node.Arguments.All(a => this.Data.GetScope(a) != NodeScope.Row) && node.Arguments.Any(a => this.Data.GetScope(a) == NodeScope.Group)) ? NodeScope.Group : NodeScope.Row);

            if (function.Arguments.Count > 0 && function.Arguments.Any(argument => argument.Type.Interfaces.Any(i => i.HasInterface(typeof(IAsyncEnumerable<>)))))
            {
                this.Data.SetScope(result, NodeScope.Group);
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
                if (this.Scope.GetSource(node.Alias) != null)
                {
                    this.AddError(node, $"There is already a source with alias {node.Alias}.");
                }
                else
                {
                    this.Scope.AddSource(node.Alias, node);
                }
            }

            var result = this.ValidateChildren(node);
            var function = this.Data.GetFunction(result.Function);

            if (!function?.ReturnType.Interfaces.Contains(typeof(IDataSource)) ?? false)
            {
                this.AddError(node, $"Function {node.Function.Name.ToUpperInvariant()} is not a data source.");
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
            var function = this.Data.GetFunction(result.Function);

            if (!function?.ReturnType.Interfaces.Contains(typeof(IDataTarget)) ?? false)
            {
                this.AddError(node, $"Function {node.Function.Name.ToUpperInvariant()} is not a data target.");
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
            if (this.Scope.IsPluginEnabled(node.Plugin))
            {
                this.AddWarning(node, $"Plugin {node.Plugin} was already enabled.");
            }

            if (!this.Scope.EnablePlugin((string)node.Plugin) && !this.Scope.IsLoadingPlugins)
            {
                var plugins = this.Scope.GetAvailablePlugins().ToArray();
                var availablePlugins = plugins.Length == 0 ? string.Empty : $" Available plugins: {string.Join(", ", plugins.Where(p => !string.Equals(p, "DefaultFunctions")))}";
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
                this.Data.CopyValues(node, result);
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

                this.Scope.AddGroupings(groupings);

                var having = this.Visit(node.Having);
                var expressions = this.Visit(node.Expressions);

                var aliasOrders = Enumerable.Join(expressions, node.Orders.Select(o => o.Expression as FieldReferenceConnectQlExpression).Where(fr => fr != null && fr.Source == null),
                        e => e.Alias,
                        fr => fr.Name,
                        (expression, field) => new
                                                   {
                                                       expression,
                                                       field,
                                                   });

                foreach (var aliasOrder in aliasOrders)
                {
                    this.Data.SetFieldReplacer(aliasOrder.field, aliasOrder.expression.Expression);
                }

                var orders = this.Visit(node.Orders);

                foreach (var invalidOrderBy in orders.Select(o => o.Expression as FieldReferenceConnectQlExpression).Where(fr => fr != null && fr.Source == null))
                {
                    this.AddError(invalidOrderBy, $"Alias '{invalidOrderBy.Name}' is not defined.");
                }

                var hasGroupings = groupings.Any();

                if (hasGroupings)
                {
                    if (having != null && this.Data.GetScope(having) == NodeScope.Row)
                    {
                        this.AddError(having, $"Missing aggregate function for '{node.Having}'.");
                    }

                    foreach (var e in expressions.Where(e => this.Data.GetScope(e) == NodeScope.Row))
                    {
                        this.AddError(e, $"Missing aggregate function for '{e}'.");
                    }
                }
                else if (having != null)
                {
                    this.AddError(having, "HAVING is only allowed when GROUP BY is used.");
                }

                return expressions != node.Expressions || source != node.Source || where != node.Where ||
                       groupings != node.Groupings || having != node.Having || orders != node.Orders
                           ? this.Data.CopyValues(node, new SelectFromStatement(expressions, source, where, groupings, having, orders))
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
            this.Scope.AddSource(node.Alias, node);

            return base.VisitSelectSource(node);
        }

        /// <summary>
        /// The visit trigger.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="ConnectQl.Parser.Ast.Node"/>.
        /// </returns>
        [NotNull]
        protected internal override Node VisitTrigger(Trigger node)
        {
            var result = (Trigger)node.VisitChildren(this);
            var function = this.Data.GetFunction(result.Function);

            if (!function?.ReturnType.Interfaces.Contains(typeof(ITrigger)) ?? false)
            {
                this.AddError(node, $"Function {node.Function.Name.ToUpperInvariant()} is not a trigger.");
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

            this.Data.SetType(node, new TypeDescriptor(OperatorHelper.InferType(node.Op, this.Data.GetType(node.Expression).SimplifiedType)));

            if (this.Scope.IsGroupByExpression(node))
            {
                this.Data.SetScope(node, NodeScope.Group);
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

            this.Scope.AddVariable(node.Name, node.Expression == null ? new TypeDescriptor(typeof(object)) : this.Data.GetType(node.Expression));

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

            var variableType = this.Scope.GetVariableType(node.Variable);

            if (variableType == null)
            {
                this.AddError(node, $"Undeclared variable '{node.Variable}'.");
            }
            else if (!variableType.Interfaces.Contains(typeof(IDataSource)))
            {
                this.AddError(node, $"Variable {node.Variable} is not a data source.");
            }

            if (this.Scope.GetSource(node.Alias) != null)
            {
                this.AddError(node, $"There is already a source with alias {node.Alias}.");
            }
            else
            {
                this.Scope.AddSource(node.Alias, node);
            }

            this.Data.SetType(node, variableType);
            this.Data.SetScope(node, NodeScope.Constant);

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

            var variableType = this.Scope.GetVariableType(node.Name);

            if (variableType == null)
            {
                this.AddError(node, $"Undeclared variable '{node.Name}'.");
            }

            this.Data.SetType(node, variableType);
            this.Data.SetScope(node, NodeScope.Constant);

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

            var variableType = this.Scope.GetVariableType(node.Variable);

            if (variableType == null)
            {
                this.AddError(node, $"Undeclared variable '{node.Variable}'.");
            }
            else if (!variableType.Interfaces.Contains(typeof(IDataTarget)))
            {
                this.AddError(node, $"Variable {node.Variable} is not a data target.");
            }

            this.Data.SetType(node, variableType);
            this.Data.SetScope(node, NodeScope.Constant);

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
                var source = this.Scope.GetSource(node.Source);

                if (source == null)
                {
                    this.AddError(node, $"Source '{node.Source}' was referenced before it was declared.");
                }

                this.Data.Set(node, "Source", source);
            }

            this.Data.SetScope(node, NodeScope.Row);

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
            var scope = this.Scope;
            this.Scope = this.Scope.CreateSubScope();

            return new ActionOnDispose(() => this.Scope = scope);
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
            this.Data.TryGet(node, "Context", out IParserContext parserContext);
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
            this.Data.TryGet(node, "Context", out IParserContext parserContext);
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
            var function = this.Data.GetFunction(node);

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
                            [i] = this.Data.CopyValues(arguments[i], enumExpr),
                        });
            }

            return arguments != node.Arguments ? this.Data.CopyValues(node, new FunctionCallConnectQlExpression(node.Name, arguments)) : node;
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

            var scopes = Enumerable.Select(result.Children.OfType<ConnectQlExpressionBase>(), expression =>
                    new
                        {
                            Expression = expression,
                            Scope = this.Data.GetScope(expression),
                        })
                .ToArray();

            if (Enumerable.Any(scopes, s => s.Expression == null || s.Scope == NodeScope.Error || s.Scope == NodeScope.Initial))
            {
                this.AddError(node, "Invalid scope.");
            }

            if (Enumerable.Any(scopes, s => s.Scope == NodeScope.Group))
            {
                if (Enumerable.Any(scopes, s => s.Scope == NodeScope.Row))
                {
                    foreach (var child in Enumerable.Where(scopes, s => s.Scope == NodeScope.Row))
                    {
                        this.AddError(child.Expression, $"Missing aggregate function for '{child.Expression}'.");
                    }
                }
                else
                {
                    this.Data.SetScope(baseExpression, NodeScope.Group);
                }
            }
            else if (Enumerable.Any(scopes, s => s.Scope == NodeScope.Row))
            {
                this.Data.SetScope(baseExpression, NodeScope.Row);
            }
            else
            {
                this.Data.SetScope(baseExpression, NodeScope.Constant);
            }

            return result;
        }
    }
}