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

namespace ConnectQl.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.DataSources;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Interfaces;

    /// <summary>
    /// The execution context implementation.
    /// </summary>
    internal class ExecutionContextImplementation : IInternalExecutionContext, IValidationContext
    {
        /// <summary>
        /// The defaults.
        /// </summary>
        private readonly Dictionary<string, object> defaults = new Dictionary<string, object>();

        /// <summary>
        /// The file formats.
        /// </summary>
        private readonly FileFormatsImplementation fileFormats;

        /// <summary>
        /// The function names.
        /// </summary>
        private readonly Dictionary<object, string> functionNames = new Dictionary<object, string>();

        /// <summary>
        /// The function names.
        /// </summary>
        private readonly Dictionary<object, string> displayNames = new Dictionary<object, string>();

        /// <summary>
        /// The values.
        /// </summary>
        private readonly Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The plugins.
        /// </summary>
        private IConnectQlPlugin[] plugins;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContextImplementation"/> class.
        /// </summary>
        /// <param name="parentContext">
        /// The parent context.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public ExecutionContextImplementation(ConnectQlContext parentContext, string filename)
        {
            this.Filename = filename;
            this.ParentContext = parentContext;
            this.Messages = new MessageWriter(filename);
            this.fileFormats = new FileFormatsImplementation();
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Gets the maximum rows to scan when determining the columns in a source.
        /// </summary>
        public int MaxRowsToScan { get; } = 100;

        /// <summary>
        /// Gets the write progress interval.
        /// </summary>
        public long WriteProgressInterval => this.ParentContext.WriteProgressInterval;

        /// <summary>
        /// Gets the log.
        /// </summary>
        public ILog Log => this.ParentContext.Log;

        /// <summary>
        /// Gets the message writer.
        /// </summary>
        public IMessageWriter Messages { get; }

        /// <summary>
        /// Gets the node data.
        /// </summary>
        public INodeDataProvider NodeData { get; } = new NodeDataProvider();

        /// <summary>
        /// Gets the parent context.
        /// </summary>
        public ConnectQlContext ParentContext { get; }

        /// <summary>
        /// Gets the file formats.
        /// </summary>
        IFileFormats IValidationContext.FileFormats => this.fileFormats;

        /// <summary>
        /// Gets the file formats.
        /// </summary>
        IEnumerable<IFileAccess> IExecutionContext.FileFormats => this.fileFormats;

        /// <summary>
        /// Gets the maximum chunk size.
        /// </summary>
        public long MaximumChunkSize => this.ParentContext.MaterializationPolicy.MaximumChunkSize;

        /// <summary>
        /// Creates a builder that can be used to create an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the items in the <see cref="IAsyncEnumerable{T}"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IAsyncEnumerableBuilder{T}"/>.
        /// </returns>
        public IAsyncEnumerableBuilder<T> CreateBuilder<T>()
        {
            return this.ParentContext.MaterializationPolicy.CreateBuilder<T>();
        }

        /// <summary>
        /// Gets the default setting for a data source. A 'USE DEFAULT' statement can be used to set a default value for a
        ///     function.
        /// </summary>
        /// <param name="setting">
        /// The default setting get the value for.
        /// </param>
        /// <param name="source">
        /// The data source to get the value for.
        /// </param>
        /// <param name="throwOnError">
        /// <c>true</c>to throw an exception when an error occurs.
        /// </param>
        /// <returns>
        /// The value for the function for the specified source.
        /// </returns>
        public object GetDefault(string setting, IDataAccess source, bool throwOnError)
        {
            if (this.defaults.TryGetValue($"{setting}|{this.functionNames[source]}", out object result) && result != null)
            {
                return result;
            }

            if (!throwOnError)
            {
                return null;
            }

            throw new InvalidOperationException($"No default {setting.ToUpperInvariant()} found for {this.functionNames[source].ToUpperInvariant()}. Register by using 'USE DEFAULT {setting.ToUpperInvariant()}(...) FOR {this.functionNames[source].ToUpperInvariant()}'");
        }

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        /// <returns>
        /// The plugin, or <c>null</c> if it wasn't found.
        /// </returns>
        public IEnumerable<IConnectQlPlugin> GetPlugins()
        {
            return this.plugins ?? (this.plugins = this.ParentContext.PluginResolver?.EnumerateAvailablePlugins().ToArray()) ?? new IConnectQlPlugin[0];
        }

        /// <summary>
        /// Gets the value for the specified variable.
        /// </summary>
        /// <typeparam name="T">
        /// The type to retrieve.
        /// </typeparam>
        /// <param name="variable">
        /// The name of the variable, including the '@'.
        /// </param>
        /// <returns>
        /// The value of the variable.
        /// </returns>
        public T GetVariable<T>(string variable)
        {
            if (this.values.TryGetValue(variable, out object result))
            {
                if (typeof(T) == typeof(object))
                {
                    return (T)result;
                }

                return (T)Convert.ChangeType(result, typeof(T));
            }

            return default(T);
        }

        /// <summary>
        /// Opens a file.
        /// </summary>
        /// <param name="uri">
        /// The uri of the file.
        /// </param>
        /// <param name="mode">
        /// The file mode.
        /// </param>
        /// <returns>
        /// The stream containing the data of the file.
        /// </returns>
        public Task<Stream> OpenStreamAsync(string uri, UriResolveMode mode)
        {
            if (this.ParentContext.UriResolver == null)
            {
                throw new InvalidOperationException($"No URI resolver registered, cannot open '{uri}'.");
            }

            return this.ParentContext.UriResolver(uri, mode);
        }

        /// <summary>
        /// Sets the display name.
        /// </summary>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        public void SetDisplayName(IDataAccess access, string displayName)
        {
            this.displayNames[access] = displayName;
        }

        /// <summary>
        /// Registers a default value for the specified target function and function name.
        /// </summary>
        /// <param name="setting">
        /// The target function.
        /// </param>
        /// <param name="functionName">
        /// The function name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void RegisterDefault(string setting, string functionName, object value)
        {
            this.defaults[$"{setting}|{functionName}"] = value;
        }

        /// <summary>
        /// Gets the display name for the specified access.
        /// </summary>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetDisplayName(IDataAccess access)
        {
            return this.displayNames[access];
        }

        /// <summary>
        /// Sets the function name for the specified data target.
        /// </summary>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <param name="functionName">
        /// The function name.
        /// </param>
        public void SetFunctionName(IDataAccess access, string functionName)
        {
            this.functionNames[access] = functionName;
        }

        /// <summary>
        /// Sets the variable to the specified value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="variable">
        /// The name of the variable, including the '@'.
        /// </param>
        /// <param name="value">
        /// The value of the variable.
        /// </param>
        public void SetVariable<T>(string variable, T value)
        {
            this.values[variable] = value;
        }

        /// <summary>
        /// Sorts the elements in an <see cref="IAsyncReadOnlyCollection{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> to use when comparing elements.
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements of the <see cref="IAsyncEnumerable{T}"/>.
        /// </typeparam>
        /// <returns>
        /// A sorted and materialized <see cref="IAsyncReadOnlyCollection{T}"/>.
        /// </returns>
        Task<IAsyncReadOnlyCollection<T>> IMaterializationPolicy.SortAsync<T>(IAsyncEnumerable<T> source, Comparison<T> comparison)
        {
            return this.ParentContext.MaterializationPolicy.SortAsync(source, comparison);
        }
    }
}