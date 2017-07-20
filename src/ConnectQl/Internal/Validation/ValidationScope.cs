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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Sources;
    using ConnectQl.Internal.Interfaces;

    /// <summary>
    /// The validation scope.
    /// </summary>
    internal class ValidationScope : IPluginContext
    {
        /// <summary>
        /// The aliases.
        /// </summary>
        private readonly HashSet<string> aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The context.
        /// </summary>
        private readonly IValidationContext context;

        /// <summary>
        /// The functions.
        /// </summary>
        private readonly Dictionary<string, IFunctionDescriptor> functions = new Dictionary<string, IFunctionDescriptor>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The groupings.
        /// </summary>
        private readonly List<SqlExpressionBase> groupings = new List<SqlExpressionBase>();

        /// <summary>
        /// The registered plugins.
        /// </summary>
        private readonly HashSet<IConnectQlPlugin> registeredPlugins = new HashSet<IConnectQlPlugin>();

        /// <summary>
        /// The functions.
        /// </summary>
        private readonly Dictionary<string, SourceBase> sources = new Dictionary<string, SourceBase>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The variables.
        /// </summary>
        private readonly Dictionary<string, ITypeDescriptor> variables = new Dictionary<string, ITypeDescriptor>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationScope"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public ValidationScope(IValidationContext context)
        {
            this.context = context;
            this.Functions = new ConnectQlFunctions(this.functions, null);
            this.FileFormats = context.FileFormats;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationScope"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        private ValidationScope(ValidationScope parent)
            : this(parent.context)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the file formats.
        /// </summary>
        public IFileFormats FileFormats { get; }

        /// <summary>
        /// Gets the functions.
        /// </summary>
        public IConnectQlFunctions Functions { get; }

        /// <summary>
        /// Gets the parent validation scope.
        /// </summary>
        public ValidationScope Parent { get; }

        /// <summary>
        /// Adds an alias for a field to the scope. When an alias already exists in the current scope, it is appended by a
        ///     number.
        /// </summary>
        /// <param name="alias">
        /// The alias to add. When this is <c>null</c>, a default value will be supplied.
        /// </param>
        /// <returns>
        /// The alias for the node.
        /// </returns>
        public string AddAlias(string alias)
        {
            alias = alias ?? "Expr";

            var suffix = string.Empty;
            var counter = 0;

            while (!this.aliases.Add(alias + suffix))
            {
                suffix = (++counter).ToString();
            }

            return alias + suffix;
        }

        /// <summary>
        /// Adds groupings to the current scope.
        /// </summary>
        /// <param name="groupingsToAdd">
        /// The groupings to add.
        /// </param>
        public void AddGroupings(IEnumerable<SqlExpressionBase> groupingsToAdd)
        {
            this.groupings.AddRange(groupingsToAdd);
        }

        /// <summary>
        /// Adds a source to the current scope.
        /// </summary>
        /// <param name="sourceAlias">
        /// The source alias.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public void AddSource(string sourceAlias, SourceBase source)
        {
            if (sourceAlias != null)
            {
                this.sources[sourceAlias] = source;
            }
        }

        /// <summary>
        /// Adds a variable to the scope.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public void AddVariable(string name, ITypeDescriptor type)
        {
            this.variables[name] = type;
        }

        /// <summary>
        /// Creates a sub-scope.
        /// </summary>
        /// <returns>
        /// The <see cref="ValidationScope"/>.
        /// </returns>
        public ValidationScope CreateSubScope()
        {
            return new ValidationScope(this);
        }

        /// <summary>
        /// The enable plugin.
        /// </summary>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        public void EnablePlugin(IConnectQlPlugin plugin)
        {
            if (this.IsPluginEnabled(plugin.Name))
            {
                return;
            }

            this.registeredPlugins.Add(plugin);

            plugin.RegisterPlugin(this);
        }

        /// <summary>
        /// The enable plugin.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool EnablePlugin(string name)
        {
            var plugin = this.context.GetPlugins().FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

            if (plugin == null)
            {
                return false;
            }

            this.EnablePlugin(plugin);

            return true;
        }

        /// <summary>
        /// The get available plugins.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<string> GetAvailablePlugins()
        {
            return this.context.GetPlugins().Select(p => p.Name);
        }

        /// <summary>
        /// Gets the function by name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="IFunctionDescriptor"/>.
        /// </returns>
        public IFunctionDescriptor GetFunction(string name, ReadOnlyCollection<SqlExpressionBase> arguments)
        {
            var scope = this;

            while (scope != null)
            {
                if (scope.functions.TryGetValue($"{name}'{arguments.Count}", out IFunctionDescriptor function))
                {
                    return function;
                }

                scope = scope.Parent;
            }

            return null;
        }

        /// <summary>
        /// Gets the source with the specified alias.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="SourceBase"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the source is not found.
        /// </exception>
        public SourceBase GetSource(string alias)
        {
            var scope = this;
            while (scope != null && alias != null)
            {
                if (scope.sources.TryGetValue(alias, out SourceBase source))
                {
                    return source;
                }

                scope = scope.Parent;
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        /// <param name="variable">
        /// The variable.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> or <c>null</c> when the variable was not declared in this or the parent scopes.
        /// </returns>
        public ITypeDescriptor GetVariableType(string variable)
        {
            var scope = this;
            while (scope != null)
            {
                if (scope.variables.TryGetValue(variable, out ITypeDescriptor type))
                {
                    return type;
                }

                scope = scope.Parent;
            }

            return null;
        }

        /// <summary>
        /// Checks if a node is used in a group-by expression.
        /// </summary>
        /// <param name="node">
        /// The node to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the node is in a group-by expression, <c>false</c> otherwise.
        /// </returns>
        public bool IsGroupByExpression(SqlExpressionBase node)
        {
            var scope = this;

            while (scope != null)
            {
                if (scope.groupings.Contains(node))
                {
                    return true;
                }

                scope = scope.Parent;
            }

            return false;
        }

        /// <summary>
        /// The is plugin enabled.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsPluginEnabled(string name)
        {
            var plugin = this.context.GetPlugins()?.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            var scope = this;

            while (scope != null)
            {
                if (scope.registeredPlugins.Contains(plugin))
                {
                    return true;
                }

                scope = scope.Parent;
            }

            return false;
        }
    }
}