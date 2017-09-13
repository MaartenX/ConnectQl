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
namespace ConnectQl.Internal.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Ast.Statements;
    using ConnectQl.Internal.Intellisense.Protocol;
    using ConnectQl.Internal.Interfaces;

    /// <summary>
    ///     The statement info.
    /// </summary>
    internal class EvaluationResult : IInternalExecutionContext, IEvaluationResult
    {
        /// <summary>
        ///     The context.
        /// </summary>
        private readonly ExecutionContextImplementation context;

        /// <summary>
        ///     The sources.
        /// </summary>
        private readonly List<SerializableDataSourceDescriptorRange> sources = new List<SerializableDataSourceDescriptorRange>();

        /// <summary>
        ///     The statementVariables.
        /// </summary>
        private readonly List<StatementBase> statements = new List<StatementBase>();

        /// <summary>
        ///     The tokens.
        /// </summary>
        private readonly IReadOnlyList<IClassifiedToken> tokens;

        /// <summary>
        ///     The ranges.
        /// </summary>
        private readonly List<SerializableVariableDescriptorRange> variables = new List<SerializableVariableDescriptorRange>();

        /// <summary>
        ///     The variables values.
        /// </summary>
        private readonly IDictionary<string, object> variablesValues = new Dictionary<string, object>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="EvaluationResult" /> class.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="tokens">
        ///     The tokens.
        /// </param>
        public EvaluationResult(ExecutionContextImplementation context, IReadOnlyList<IClassifiedToken> tokens)
        {
            this.context = context;
            this.tokens = tokens;
        }

        /// <summary>
        ///     Gets the maximum chunk size.
        /// </summary>
        public long MaximumChunkSize => this.context.MaximumChunkSize;

        /// <summary>
        ///     Gets the maximum rows to scan when determining the columns in a source.
        /// </summary>
        public int MaxRowsToScan => this.context.MaxRowsToScan;

        /// <summary>
        /// Gets the write progress interval.
        /// </summary>
        public long WriteProgressInterval => this.context.WriteProgressInterval;

        /// <summary>
        ///     Gets the sources.
        /// </summary>
        public IReadOnlyList<SerializableDataSourceDescriptorRange> Sources => this.sources;

        /// <summary>
        ///     Gets the variables.
        /// </summary>
        public IReadOnlyList<SerializableVariableDescriptorRange> Variables => this.variables;

        /// <summary>
        ///     Gets the available file formats.
        /// </summary>
        IEnumerable<IFileAccess> IExecutionContext.FileFormats => ((IInternalExecutionContext)this.context).FileFormats;

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        ILogger IExecutionContext.Logger => this.context.Logger;

        /// <summary>
        ///     Gets the display name for the specified access.
        /// </summary>
        /// <param name="access">
        ///     The access.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetDisplayName(IDataAccess access)
        {
            return this.context.GetDisplayName(access);
        }

        /// <summary>
        ///     Checks if the variable has side effects.
        /// </summary>
        /// <param name="variable">
        ///     The variable.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the variable has side effects, <c>false</c> otherwise.
        /// </returns>
        public bool HasVariableSideEffects(string variable)
        {
            var lastDeclaration = this.variables.Where(v => v.Variable.Name.Equals(variable, StringComparison.OrdinalIgnoreCase)).OrderByDescending(v => v.Start).FirstOrDefault();

            return !lastDeclaration?.Variable.WasEvaluated ?? true;
        }

        /// <summary>
        ///     Updates the current statement.
        /// </summary>
        /// <param name="statement">
        ///     The statement.
        /// </param>
        public void SetActiveStatement(StatementBase statement)
        {
            this.statements.Add(statement);
        }

        /// <summary>
        ///     Sets the display name.
        /// </summary>
        /// <param name="access">
        ///     The access.
        /// </param>
        /// <param name="displayName">
        ///     The display name.
        /// </param>
        public void SetDisplayName(IDataAccess access, string displayName)
        {
            this.context.SetDisplayName(access, displayName);
        }

        /// <summary>
        ///     Sets the source descriptor for this statement.
        /// </summary>
        /// <param name="sourceAlias">
        ///     The source alias.
        /// </param>
        /// <param name="descriptor">
        ///     The descriptor.
        /// </param>
        public void SetSource(string sourceAlias, IDataSourceDescriptor descriptor)
        {
            if (this.context.NodeData.TryGet(this.statements.Last(), "Context", out IParserContext nodeContext))
            {
                this.sources.Add(
                    new SerializableDataSourceDescriptorRange
                    {
                        Start = this.tokens[nodeContext.Start.TokenIndex].Start,
                        End = this.tokens[nodeContext.End.TokenIndex].End,
                        DataSource = new SerializableDataSourceDescriptor(descriptor),
                    });
            }
        }

        /// <summary>
        ///     Creates a builder that can be used to create an <see cref="IAsyncEnumerable{T}" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IAsyncEnumerableBuilder{T}" />.
        /// </returns>
        IAsyncEnumerableBuilder<T> IMaterializationPolicy.CreateBuilder<T>()
        {
            return this.context.CreateBuilder<T>();
        }

        /// <summary>
        ///     Gets the default setting for a data source. A 'USE DEFAULT' statement can be used to set a default value for a
        ///     function.
        /// </summary>
        /// <param name="setting">
        ///     The default setting get the value for.
        /// </param>
        /// <param name="source">
        ///     The data source to get the value for.
        /// </param>
        /// <param name="throwOnError">
        ///     <c>true</c>to throw an exception when an error occurs.
        /// </param>
        /// <returns>
        ///     The value for the function for the specified source.
        /// </returns>
        object IExecutionContext.GetDefault(string setting, IDataAccess source, bool throwOnError)
        {
            return this.context.GetDefault(setting, source, throwOnError);
        }

        /// <summary>
        ///     Gets the value for the specified variable.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the variable.
        /// </typeparam>
        /// <param name="variable">
        ///     The name of the variable, including the '@'.
        /// </param>
        /// <returns>
        ///     The value of the variable.
        /// </returns>
        T IExecutionContext.GetVariable<T>(string variable)
        {
            try
            {
                if (this.variablesValues.TryGetValue(variable, out object value))
                {
                    return typeof(T) == typeof(object) || typeof(T).GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo())
                        ? (T)value
                        : (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch
            {
                // Ignore.
            }

            return default(T);
        }

        /// <summary>
        ///     Opens a file.
        /// </summary>
        /// <param name="uri">
        ///     The uri of the file.
        /// </param>
        /// <param name="uriResolveMode">
        ///     The file mode.
        /// </param>
        /// <returns>
        ///     The stream containing the data of the file.
        /// </returns>
        Task<Stream> IExecutionContext.OpenStreamAsync(string uri, UriResolveMode uriResolveMode)
        {
            return this.context.OpenStreamAsync(uri, uriResolveMode);
        }

        /// <summary>
        ///     Registers a default value for the specified target function and function name.
        /// </summary>
        /// <param name="setting">
        ///     The setting to register.
        /// </param>
        /// <param name="functionName">
        ///     The function name to register the setting for.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        void IInternalExecutionContext.RegisterDefault(string setting, string functionName, object value)
        {
            this.context.RegisterDefault(setting, functionName, value);
        }

        /// <summary>
        ///     Sets the function name for the specified data source.
        /// </summary>
        /// <param name="access">
        ///     The data access to set the name for.
        /// </param>
        /// <param name="functionName">
        ///     The function name.
        /// </param>
        void IInternalExecutionContext.SetFunctionName(IDataAccess access, string functionName)
        {
            this.context.SetFunctionName(access, functionName);
        }

        /// <summary>
        ///     Sets the variable to the specified value.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the variable.
        /// </typeparam>
        /// <param name="variable">
        ///     The name of the variable, including the '@'.
        /// </param>
        /// <param name="value">
        ///     The value of the variable.
        /// </param>
        void IInternalExecutionContext.SetVariable<T>(string variable, T value)
        {
            this.SetVariable(variable, value, true);
        }

        /// <summary>
        ///     Creates a new <see cref="IAsyncReadOnlyCollection{T}" /> that contains the sorted elements of the
        ///     <see cref="IAsyncEnumerable{T}" />.
        /// </summary>
        /// <param name="source">
        ///     The <see cref="IAsyncEnumerable{T}" /> to sort.
        /// </param>
        /// <param name="comparison">
        ///     The comparison to use while sorting.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the items.
        /// </typeparam>
        /// <returns>
        ///     The sorted <see cref="IAsyncReadOnlyCollection{T}" />.
        /// </returns>
        Task<IAsyncReadOnlyCollection<T>> IMaterializationPolicy.SortAsync<T>(IAsyncEnumerable<T> source, Comparison<T> comparison)
        {
            return ((IInternalExecutionContext)this.context).SortAsync(source, comparison);
        }

        /// <summary>
        ///     Sets the variable to the specified value.
        /// </summary>
        /// <param name="variable">
        ///     The variable.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="wasEvaluated">
        ///     <c>true</c> if the variable was evaluated, <c>false</c> if it had side effects.
        /// </param>
        internal void SetVariable(string variable, object value, bool wasEvaluated)
        {
            this.variablesValues[variable] = value;

            var nodeContext = this.context.NodeData.Get<IParserContext>(this.statements.Last(), "Context");

            this.variables.Add(
                new SerializableVariableDescriptorRange
                {
                    Start = this.tokens[nodeContext.Start.TokenIndex].Start,
                    End = this.tokens[nodeContext.End.TokenIndex].End,
                    Variable = new SerializableVariableDescriptor
                    {
                        Name = variable,
                        Type = new SerializableTypeDescriptor(new TypeDescriptor(value?.GetType() ?? typeof(object))),
                        Value = value is string ? $"'{((string)value).Replace("'", "''")}'" : value?.ToString() ?? "null",
                        WasEvaluated = wasEvaluated
                    },
                });
        }
    }
}