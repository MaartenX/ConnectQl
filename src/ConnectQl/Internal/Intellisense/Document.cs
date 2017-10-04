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
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Intellisense.Protocol;
    using ConnectQl.Internal.Validation;

    using JetBrains.Annotations;

    /// <summary>
    /// The document.
    /// </summary>
    internal class Document : IDocumentDescriptor
    {
        /// <summary>
        /// The session.
        /// </summary>
        private readonly IntellisenseSession session;

        /// <summary>
        /// The queue lock.
        /// </summary>
        private readonly object updateLock = new object();

        /// <summary>
        /// True if the document is currently updating.
        /// </summary>
        private bool updating;

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public Document(IntellisenseSession session, string filename)
        {
            this.session = session;
            this.Descriptor = new SerializableDocumentDescriptor { Filename = filename };
        }

        /// <summary>
        /// Gets the contents.
        /// </summary>
        public string Contents { get; private set; }

        /// <summary>
        /// Gets the version of the document.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename => this.Descriptor.Filename;

        /// <summary>
        /// Gets the functions.
        /// </summary>
        public IReadOnlyList<IFunctionDescriptor> Functions => this.Descriptor.Functions;

        /// <summary>
        /// Gets the tokens.
        /// </summary>
        public IReadOnlyList<IClassifiedToken> Tokens => this.Descriptor.Tokens;

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public IReadOnlyList<IMessage> Messages => this.Descriptor.Messages;

        /// <summary>
        /// Gets the variables.
        /// </summary>
        public IReadOnlyList<IVariableDescriptorRange> Variables => this.Descriptor.Variables;

        /// <summary>
        /// Gets the sources.
        /// </summary>
        public IReadOnlyList<IDataSourceDescriptorRange> Sources => this.Descriptor.Sources;

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        public IReadOnlyList<string> Plugins => this.Descriptor.Plugins;

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <value>
        /// The descriptor.
        /// </value>
        internal SerializableDocumentDescriptor Descriptor { get; }

        /// <summary>
        /// Updates the document.
        /// </summary>
        /// <param name="contents">The new contents of the document.</param>
        /// <param name="documentVersion">The new version of the document.</param>
        public void Update(string contents, int documentVersion)
        {
            this.Contents = contents;
            this.Version = documentVersion;
            var shouldUpdate = false;

            lock (this.updateLock)
            {
                if (!this.updating)
                {
                    this.updating = true;
                    shouldUpdate = true;
                }
            }

            if (shouldUpdate)
            {
                this.Update(this.Contents);
            }
        }

        /// <summary>
        /// Parses this document.
        /// </summary>
        /// <param name="documentText">
        /// The document Text.
        /// </param>
        private void Update(string documentText)
        {
            Action updateIntellisenseData = null;

            updateIntellisenseData = () =>
                {
                    try
                    {
                        var descriptor = new SerializableDocumentDescriptor();
                        var parsedDocument = this.ParseContent(documentText, descriptor);
                        var delta = this.GetChanges(descriptor);

                        if (delta != null)
                        {
                            this.session.OnDocumentChanged(delta);
                        }

                        if (this.Contents == documentText)
                        {
                            this.ValidateDocument(parsedDocument, descriptor);

                            delta = this.GetChanges(descriptor);

                            if (delta != null)
                            {
                                this.session.OnDocumentChanged(delta);
                            }
                        }

                        if (this.Contents == documentText)
                        {
                            var data = Evaluator.GetIntellisenseData(parsedDocument, descriptor.Tokens);

                            delta = this.GetChanges(new SerializableDocumentDescriptor
                                                        {
                                                            Variables = data.Variables.ToArray(),
                                                            Sources = data.Sources.ToArray(),
                                                        });

                            if (delta != null)
                            {
                                this.session.OnDocumentChanged(delta);
                            }
                        }

                        var shouldUpdate = false;

                        lock (this.updateLock)
                        {
                            if (this.Contents == documentText)
                            {
                                this.updating = false;
                            }
                            else
                            {
                                shouldUpdate = true;
                            }
                        }

                        if (shouldUpdate)
                        {
                            documentText = this.Contents;

                            Task.Run(updateIntellisenseData);
                        }
                    }
                    catch
                    {
                        lock (this.updateLock)
                        {
                            this.updating = false;
                        }
                    }
                };

            Task.Run(updateIntellisenseData);
        }

        /// <summary>
        /// Gets a serializable document containing the delta between the current document and the previous version.
        /// </summary>
        /// <param name="currentDocument">
        /// The current document.
        /// </param>
        /// <returns>
        /// The <see cref="SerializableDocumentDescriptor"/>.
        /// </returns>
        [CanBeNull]
        private SerializableDocumentDescriptor GetChanges(SerializableDocumentDescriptor currentDocument)
        {
            var result = new SerializableDocumentDescriptor { Filename = this.Filename, Version = this.Version };

            var updated = this.TryUpdate(currentDocument, result, d => d.Tokens) |
                          this.TryUpdate(currentDocument, result, d => d.Functions) |
                          this.TryUpdate(currentDocument, result, d => d.Messages) |
                          this.TryUpdate(currentDocument, result, d => d.Sources) |
                          this.TryUpdate(currentDocument, result, d => d.Variables) |
                          this.TryUpdate(currentDocument, result, d => d.Plugins);

            return updated ? result : null;
        }

        /// <summary>
        /// Parses the content into a document and stores the tokens into the descriptor.
        /// </summary>
        /// <param name="content">The content to parse.</param>
        /// <param name="descriptorToUpdate">The document to update.</param>
        /// <returns>The parsed document.</returns>
        [NotNull]
        private ParsedDocument ParseContent(string content, [NotNull] SerializableDocumentDescriptor descriptorToUpdate)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var tokens = new List<Token>();
                var context = new ExecutionContextImplementation(this.session.Context, this.Filename);
                var root = ConnectQlContext.Parse(stream, context.NodeData, context.Messages, true, tokens);

                descriptorToUpdate.Tokens = Classifier.Classify(tokens).Select(token => new SerializableToken(token)).ToArray();

                return new ParsedDocument(context, root);
            }
        }

        /// <summary>
        /// Validates the document and updates the descriptor.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="descriptorToUpdate">
        /// The descriptor to update.
        /// </param>
        private void ValidateDocument([NotNull] ParsedDocument document, [NotNull] SerializableDocumentDescriptor descriptorToUpdate)
        {
            document.Root = Validator.Validate(document.Context, document.Root, out var functionDefinitions);
            var messages = document.Context.Messages.Select(message => new SerializableMessage(message)).ToArray();
            var functions = functionDefinitions.SelectMany(lookup => lookup.Select(function => new SerializableFunctionDescriptor(lookup.Key, function))).ToArray();

            descriptorToUpdate.Messages = messages;
            descriptorToUpdate.Functions = functions;
            descriptorToUpdate.Plugins = document.Context.GetPlugins().Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Tries to update the property that the selector uses.
        /// </summary>
        /// <param name="newDocument">
        /// The new version of the document. Only non-null items are updated in the current document.
        /// </param>
        /// <param name="delta">
        /// The delta document. Has all changes that differ from the document.
        /// </param>
        /// <param name="selector">
        /// Returns the <see cref="IEnumerable{T}"/> to update.
        /// </param>
        /// <typeparam name="T">
        /// The type of the items in the selector.
        /// </typeparam>
        /// <returns>
        /// True if the document was updated, false otherwise.
        /// </returns>
        private bool TryUpdate<T>(SerializableDocumentDescriptor newDocument, SerializableDocumentDescriptor delta, [NotNull] Expression<Func<SerializableDocumentDescriptor, IReadOnlyList<T>>> selector)
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)selector.Body).Member;
            var getValue = selector.Compile();
            var newValue = getValue(newDocument);
            var existingValue = getValue(this.Descriptor);

            if (newValue == null || existingValue != null && EnumerableComparer.Equals(newValue, existingValue))
            {
                return false;
            }

            propertyInfo.SetValue(delta, newValue);
            propertyInfo.SetValue(this.Descriptor, newValue);

            return true;
        }
    }
}