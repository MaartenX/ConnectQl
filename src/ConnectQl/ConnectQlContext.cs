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

namespace ConnectQl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Internal.Ast.Statements;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Loggers;
    using ConnectQl.Internal.Query;
    using ConnectQl.Internal.Validation;
    using ConnectQl.Results;

    /// <summary>
    ///     The ConnectQl context.
    /// </summary>
    public class ConnectQlContext : IDisposable
    {
        /// <summary>
        /// The default plugin resolver.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static IPluginResolver defaultPluginResolver;

        /// <summary>
        ///     The functions.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, IFunctionDescriptor> functions = new Dictionary<string, IFunctionDescriptor>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// <c>true</c> if we are disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        ///     Stores the logger.
        /// </summary>
        private ILog log = new NullLogger();

        /// <summary>
        ///     Stores the materialization policy.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IMaterializationPolicy materializationPolicy;

        /// <summary>
        /// The URI resolver.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<string, UriResolveMode, Task<Stream>> uriResolver;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConnectQlContext" /> class.
        /// </summary>
        public ConnectQlContext()
            : this(DefaultPluginResolver)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConnectQlContext" /> class.
        /// </summary>
        /// <param name="resolver">
        ///     The resolver.
        /// </param>
        public ConnectQlContext(IPluginResolver resolver)
        {
            this.PluginResolver = resolver;
        }

        /// <summary>
        /// Gets or sets the default plugin resolver.
        /// </summary>
        public static IPluginResolver DefaultPluginResolver
        {
            get
            {
                return defaultPluginResolver ?? ReflectionLoader.PluginResolver.Value;
            }

            set
            {
                defaultPluginResolver = value;
            }
        }

        /// <summary>
        ///     Gets or sets the job runner.
        /// </summary>
        public IJobRunner JobRunner { get; set; }

        /// <summary>
        ///     Gets or sets the logger.
        /// </summary>
        public ILog Log
        {
            get
            {
                return this.log;
            }

            set
            {
                if (this.log is NullLogger logger && value != null)
                {
                    logger.ForwardMessages(value);
                }

                this.log = value;
            }
        }

        /// <summary>
        ///     Gets or sets the materialization policy.
        /// </summary>
        public IMaterializationPolicy MaterializationPolicy
        {
            get
            {
                return this.materializationPolicy ?? (this.materializationPolicy = new InMemoryPolicy());
            }

            set
            {
                this.materializationPolicy = value;
            }
        }

        /// <summary>
        ///     Gets or sets the plugin resolver.
        /// </summary>
        public IPluginResolver PluginResolver { get; set; }

        /// <summary>
        ///     Gets or sets a lambda that opens the file at the specified path and returns the stream.
        /// </summary>
        public Func<string, UriResolveMode, Task<Stream>> UriResolver
        {
            get
            {
                var result = this.uriResolver ?? ReflectionLoader.UriResolver.Value;

                if (result == null)
                {
                    this.Log.Verbose("No uri resolver could be created, please specify one directly.");
                }

                return result;
            }

            set
            {
                this.uriResolver = value;
            }
        }

        /// <summary>
        ///     Gets or sets the write progress interval. When this value is anything other than 0, progress is reported after this
        ///     number of records.
        /// </summary>
        public long WriteProgressInterval { get; set; }

        /// <summary>
        ///     Executes the query.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <returns>
        ///     The execute result.
        /// </returns>
        public Task<IExecuteResult> ExecuteAsync(string query)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(query)))
            {
                return this.ExecuteAsync("inline query", ms);
            }
        }

        /// <summary>
        ///     Executes the queries in the stream.
        /// </summary>
        /// <param name="stream">
        ///     The stream.
        /// </param>
        /// <returns>
        ///     The execute result.
        /// </returns>
        public Task<IExecuteResult> ExecuteAsync(Stream stream)
        {
            return this.ExecuteInternalAsync("inline query", stream);
        }

        /// <summary>
        ///     Executes the queries in the stream.
        /// </summary>
        /// <param name="filename">
        ///     The filename.
        /// </param>
        /// <param name="stream">
        ///     The stream.
        /// </param>
        /// <returns>
        ///     The execute result.
        /// </returns>
        public Task<IExecuteResult> ExecuteAsync(string filename, Stream stream)
        {
            return this.ExecuteInternalAsync(filename, stream);
        }

        /// <summary>
        ///     Executes the queries in the stream.
        /// </summary>
        /// <param name="filename">
        ///     The filename.
        /// </param>
        /// <returns>
        ///     The execute result.
        /// </returns>
        public async Task<IExecuteResult> ExecuteFileAsync(string filename)
        {
            if (this.UriResolver == null)
            {
                throw new InvalidOperationException("No URI resolver registered.");
            }

            return await this.ExecuteInternalAsync(filename, await this.UriResolver(filename, UriResolveMode.Read));
        }

        /// <summary>
        /// Disposes the context.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Parses the stream into an AST <see cref="Block" /> node.
        /// </summary>
        /// <param name="content">
        ///     The content to parse.
        /// </param>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <param name="messages">
        ///     The errors.
        /// </param>
        /// <param name="parseForIntellisense">
        ///     The parse For Intellisense.
        /// </param>
        /// <param name="tokens">
        ///     The tokens.
        /// </param>
        /// <returns>
        ///     The <see cref="Block" />.
        /// </returns>
        internal Block Parse(Stream content, INodeDataProvider data, IMessageWriter messages, bool parseForIntellisense, List<Token> tokens = null)
        {
            var scanner = new Scanner(content)
                              {
                                  EmitComments = parseForIntellisense,
                              };

            var parser = new Parser(scanner, data, messages);

            var ctx = parser.Mark();

            parser.Parse();

            if (parseForIntellisense)
            {
                while (scanner.Scan().Kind != Parser.EOFSymbol)
                {
                }
            }

            tokens?.AddRange(parser.Tokens);

            return parser.SetContext(new Block(new ReadOnlyCollection<StatementBase>(parser.Statements)), ctx);
        }

        /// <summary>
        /// Disposes the context.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> if we were called from <see cref="Dispose" />.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.functions.Clear();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        ///     Implementation of the execute.
        /// </summary>
        /// <param name="filename">
        ///     The filename.
        /// </param>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        private async Task<IExecuteResult> ExecuteInternalAsync(string filename, Stream content)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            var script = this.GetParsedScript(filename, content, false);

            if (script == null)
            {
                return null;
            }

            var context = script.Context;
            var plan = QueryPlanBuilder.Build(context.Messages, context.NodeData, script.Root);

            if (!this.HandleErrors(context))
            {
                return null;
            }

            var result = await plan.ExecuteAsync(context);

            return result;
        }

        /// <summary>
        ///     Parses and validates the content.
        /// </summary>
        /// <param name="filename">
        ///     The filename.
        /// </param>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <param name="emitComments">
        ///     The emit Comments.
        /// </param>
        /// <returns>
        ///     The parsed script, or <c>null</c> if errors occurred.
        /// </returns>
        private ParsedScript GetParsedScript(string filename, Stream content, bool emitComments)
        {
            var context = new ExecutionContextImplementation(this, filename);
            var script = this.Parse(content, context.NodeData, context.Messages, emitComments);

            if (!this.HandleErrors(context))
            {
                return null;
            }

            script = Validator.Validate(context, script);

            return this.HandleErrors(context) ? new ParsedScript(context, script) : null;
        }

        /// <summary>
        ///     Handle errors.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        /// <exception cref="Exception">
        ///     Thrown when an error was found.
        /// </exception>
        private bool HandleErrors(ExecutionContextImplementation context)
        {
            if (context.Messages.HasErrors)
            {
                foreach (var message in context.Messages)
                {
                    switch (message.Type)
                    {
                        case ResultMessageType.Error:
                            this.Log.Error(message.ToString());
                            break;
                        case ResultMessageType.Warning:
                            this.Log.Warning(message.ToString());
                            break;
                        case ResultMessageType.Information:
                            this.Log.Information(message.ToString());
                            break;
                    }
                }

                throw new Exception("Error");
            }

            return true;
        }

        /// <summary>
        ///     The classified token.
        /// </summary>
        internal class ClassifiedToken : IClassifiedToken
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="ClassifiedToken" /> class.
            /// </summary>
            /// <param name="start">
            ///     The start position of the token.
            /// </param>
            /// <param name="end">
            ///     The end position of the token.
            /// </param>
            /// <param name="classification">
            ///     The classification.
            /// </param>
            /// <param name="kind">
            ///     The kind.
            /// </param>
            /// <param name="value">
            ///     The value.
            /// </param>
            public ClassifiedToken(int start, int end, Classification classification, int kind, string value)
            {
                this.Start = start;
                this.Length = end - start;
                this.End = end;
                this.Classification = classification;
                this.Kind = kind;
                this.Value = value;
            }

            /// <summary>
            ///     Gets or sets the classification.
            /// </summary>
            public Classification Classification { get; set; }

            /// <summary>
            ///     Gets the end.
            /// </summary>
            public int End { get; }

            /// <summary>
            ///     Gets the token kind.
            /// </summary>
            public int Kind { get; }

            /// <summary>
            ///     Gets the length.
            /// </summary>
            public int Length { get; }

            /// <summary>
            ///     Gets or sets the scope.
            /// </summary>
            public ClassificationScope Scope { get; set; }

            /// <summary>
            ///     Gets the start.
            /// </summary>
            public int Start { get; }

            /// <summary>
            ///     Gets the value.
            /// </summary>
            public string Value { get; }
        }
    }
}