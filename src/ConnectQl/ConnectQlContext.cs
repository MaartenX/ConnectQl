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

using System.Resources;
using System.Runtime.CompilerServices;

[assembly: NeutralResourcesLanguage("en")]

#if NOT_SIGNED
[assembly: InternalsVisibleTo("ConnectQl.Tests")]
#else 
[assembly: InternalsVisibleTo("ConnectQl.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100794156f7f392b0e582199123489c3194624082324f16629f2ad262202f1f17de02812b3fc6903b40e1586e2176ce45befc6af57537ff38408702a9383d35658724915eb4427c5404aef2b142cbddd22e1156319c76de32b1cee6266f4e3116ce9a2da22ec6c27d567e921e04de2bf840139f46ebbe0118f2335d100782558bc8")]
#endif

namespace ConnectQl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables.Policies;
    using ConnectQl.DataSources;
    using ConnectQl.Intellisense;
    using ConnectQl.Intellisense.Protocol;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal;
    using ConnectQl.Parser;
    using ConnectQl.Parser.Ast.Statements;
    using ConnectQl.Query;
    using ConnectQl.Results;
    using ConnectQl.Validation;

    using JetBrains.Annotations;
    
    /// <summary>
    ///     The ConnectQl context.
    /// </summary>
    public class ConnectQlContext : IDisposable, IConnectQlContext
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
        /// Stores the loggers.
        /// </summary>
        private readonly LoggerCollection loggers = new LoggerCollection();

        /// <summary>
        /// <c>true</c> if we are disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        ///     Stores the materialization policy.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IMaterializationPolicy materializationPolicy;

        /// <summary>
        /// The URI resolver.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IUriResolver uriResolver;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConnectQlContext" /> class.
        /// </summary>
        public ConnectQlContext()
            : this(ConnectQlContext.DefaultPluginResolver)
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
        [PublicAPI]
        public static IPluginResolver DefaultPluginResolver
        {
            get => ConnectQlContext.defaultPluginResolver ?? ReflectionLoader.PluginResolver.Value;
            set => ConnectQlContext.defaultPluginResolver = value;
        }

        /// <summary>
        ///     Gets or sets the job runner.
        /// </summary>
        [PublicAPI]
        public IJobRunner JobRunner { get; set; }

        /// <summary>
        /// Gets the loggers for the context.
        /// </summary>
        [PublicAPI]
        public ICollection<ILogger> Loggers => this.loggers;

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        [PublicAPI]
        public ILogger Logger => this.loggers;

        /// <summary>
        ///     Gets or sets the materialization policy.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IMaterializationPolicy MaterializationPolicy
        {
            get => this.materializationPolicy ?? (this.materializationPolicy = new InMemoryPolicy());
            set => this.materializationPolicy = value;
        }

        /// <summary>
        ///     Gets the plugin resolver.
        /// </summary>
        [PublicAPI]
        public IPluginResolver PluginResolver { get; }

        /// <summary>
        ///     Gets or sets a lambda that opens the file at the specified path and returns the stream.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public IUriResolver UriResolver
        {
            get
            {
                var result = this.uriResolver ?? ReflectionLoader.UriResolver.Value;

                if (result == null)
                {
                    this.Logger.Verbose("No uri resolver could be created, please specify one directly.");
                }

                return result;
            }

            set => this.uriResolver = value;
        }

        /// <summary>
        ///     Gets or sets the write progress interval. When this value is anything other than 0, progress is reported after this
        ///     number of records.
        /// </summary>
        [PublicAPI]
        public long WriteProgressInterval { get; set; }

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
        [PublicAPI]
        public async Task<IExecuteResult> ExecuteAsync(string filename, [CanBeNull] Stream stream)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            var script = this.GetParsedScript(filename, stream ?? await this.ResolveStream(filename), false);

            if (script == null)
            {
                return null;
            }

            var context = script.Context;
            var plan = QueryPlanBuilder.Build(context.Messages, context.NodeData, script.Root);

            if (!this.HandleErrors(context, script.Root))
            {
                return null;
            }

            var result = await plan.Invoke(context);

            return result;
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The execute result.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public async Task<IExecuteResult> ExecuteAsync([NotNull] string query)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(query)))
            {
                return await this.ExecuteAsync("inline query", ms);
            }
        }

        /// <summary>
        /// Executes the queries in the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The execute result.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public Task<IExecuteResult> ExecuteAsync([NotNull] Stream stream)
        {
            return this.ExecuteAsync("inline query", stream);
        }

        /// <summary>
        /// Executes the queries in the stream.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The execute result.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public Task<IExecuteResult> ExecuteFileAsync([NotNull] string filename)
        {
            return this.ExecuteAsync(filename, null);
        }

        /// <summary>
        /// Executes the queries in the stream and serializes the result to a byte array.
        /// </summary>
        /// <param name="filename">
        /// The file name.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The result, serialized as a byte array.
        /// </returns>
        async Task<byte[]> IConnectQlContext.ExecuteToByteArrayAsync(string filename, Stream stream)
        {
            return ProtocolSerializer.Serialize(await SerializableExecuteResult.CreateAync(await this.ExecuteAsync(filename, stream)));
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
        internal static Block Parse(Stream content, INodeDataProvider data, IMessageWriter messages, bool parseForIntellisense, [CanBeNull] List<Token> tokens = null)
        {
            var scanner = new ConnectQlScanner(content)
                              {
                                  EmitComments = parseForIntellisense,
                              };

            var parser = new ConnectQlParser(scanner, data, messages);

            var ctx = parser.Mark();

            parser.Parse();

            if (parseForIntellisense)
            {
                while (scanner.Scan().Kind != ConnectQlParser.EOFSymbol)
                {
                }
            }

            tokens?.AddRange(parser.Tokens);

            return parser.SetContext(new Block(new ReadOnlyCollection<StatementBase>(parser.Statements)), ctx);
        }

        /// <summary>
        /// Resolves a filename to a stream.
        /// </summary>
        /// <param name="filename">The filename to resolve.</param>
        /// <returns>The stream.</returns>
        private async Task<Stream> ResolveStream(string filename)
        {
            if (this.UriResolver == null)
            {
                throw new InvalidOperationException("No URI resolver registered.");
            }

            filename = this.UriResolver.GetFullPath(filename);

            return await this.UriResolver.ResolveToStream(filename, UriResolveMode.Read);
        }

        /// <summary>
        /// Disposes the context.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> if we were called from <see cref="Dispose" />.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.functions.Clear();
                    this.loggers.Dispose();
                }

                this.disposed = true;
            }
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
        [CanBeNull]
        private ParsedDocument GetParsedScript(string filename, Stream content, bool emitComments)
        {
            var context = new ExecutionContextImplementation(this, filename);
            var script = ConnectQlContext.Parse(content, context.NodeData, context.Messages, emitComments);

            if (!this.HandleErrors(context, script))
            {
                return null;
            }

            script = Validator.Validate(context, script);

            return this.HandleErrors(context, script) ? new ParsedDocument(context, script) : null;
        }

        /// <summary>
        ///     Handle errors.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="script">
        /// The script to handle errors for.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        /// <exception cref="Exception">
        ///     Thrown when an error was found.
        /// </exception>
        private bool HandleErrors([NotNull] ExecutionContextImplementation context, Block script)
        {
            if (context.Messages.HasErrors)
            {
                foreach (var message in context.Messages)
                {
                    switch (message.Type)
                    {
                        case ResultMessageType.Error:
                            this.Logger.Error(message.ToString());
                            break;
                        case ResultMessageType.Warning:
                            this.Logger.Warning(message.ToString());
                            break;
                        case ResultMessageType.Information:
                            this.Logger.Information(message.ToString());
                            break;
                    }
                }

                var errorMessage = (context.Messages.Count(e => e.Type == ResultMessageType.Error) == 1 ? "An error occurred" : "Errors occurred") + " while parsing:\n";

                // Load the log plugins before we exit, this allows displaying the log messages using imported log plugins.
                PluginLoader.LoadLogPlugins(script, this.loggers, this.PluginResolver);

                throw new ExecutionException(errorMessage, context.Messages);
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
            ///     Gets the classification.
            /// </summary>
            public Classification Classification { get; }

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
