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

    /// <summary>
    /// The document.
    /// </summary>
    internal class Document
    {
        /// <summary>
        /// The document that was sent last.
        /// </summary>
        private readonly SerializableDocumentDescriptor document;

        /// <summary>
        /// The session.
        /// </summary>
        private readonly IntellisenseSession session;

        /// <summary>
        /// The queue lock.
        /// </summary>
        private readonly object updateLock = new object();

        /// <summary>
        /// The contents.
        /// </summary>
        private string contents;

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
        /// <param name="contents">
        /// The contents.
        /// </param>
        public Document(IntellisenseSession session, string filename, string contents)
        {
            this.session = session;
            this.Filename = filename;
            this.Contents = contents;

            this.document = new SerializableDocumentDescriptor();
        }

        /// <summary>
        /// Gets or sets the contents.
        /// </summary>
        public string Contents
        {
            get
            {
                return this.contents;
            }

            set
            {
                this.contents = value;

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
                    this.Update(this.contents);
                }
            }
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Parses this document.
        /// </summary>
        /// <param name="documentText">
        /// The document Text.
        /// </param>
        public void Update(string documentText)
        {
            Action updateIntellisenseData = null;

            updateIntellisenseData = () =>
                {
                    try
                    {
                        var delta = this.GetChanges(this.ParseDocument(documentText, out ParsedScript parsedScript));

                        if (delta != null)
                        {
                            this.session.OnClassificationChanged(this.Filename, delta);
                        }

                        if (this.contents == documentText)
                        {
                            var data = Evaluator.GetIntellisenseData(parsedScript, this.document.Tokens);

                            delta = this.GetChanges(new SerializableDocumentDescriptor
                                                        {
                                                            Variables = data.Variables.ToArray(),
                                                            Sources = data.Sources.ToArray(),
                                                        });

                            if (delta != null)
                            {
                                this.session.OnClassificationChanged(this.Filename, delta);
                            }
                        }

                        var shouldUpdate = false;

                        lock (this.updateLock)
                        {
                            if (this.contents == documentText)
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
                            documentText = this.contents;
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
        private SerializableDocumentDescriptor GetChanges(SerializableDocumentDescriptor currentDocument)
        {
            var result = new SerializableDocumentDescriptor();

            var updated = this.TryUpdate(currentDocument, result, d => d.Tokens) |
                          this.TryUpdate(currentDocument, result, d => d.Functions) |
                          this.TryUpdate(currentDocument, result, d => d.Messages) |
                          this.TryUpdate(currentDocument, result, d => d.Sources) |
                          this.TryUpdate(currentDocument, result, d => d.Variables) |
                          this.TryUpdate(currentDocument, result, d => d.Plugins);

            return updated ? result : null;
        }

        /// <summary>
        /// Parses the content to a SerializableDocument.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        /// <returns>
        /// The <see cref="SerializableDocumentDescriptor"/>.
        /// </returns>
        private SerializableDocumentDescriptor ParseDocument(string content, out ParsedScript script)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var tokens = new List<Token>();
                var context = new ExecutionContextImplementation(this.session.Context, this.Filename);
                var root = this.session.Context.Parse(stream, context.NodeData, context.Messages, true, tokens);

                root = Validator.Validate(context, root, out ILookup<string, IFunctionDescriptor> functionDefinitions);

                var classifiedTokens = Classifier.Classify(context, root, tokens).Select(token => new SerializableToken(token)).ToArray();
                var messages = context.Messages.Select(message => new SerializableMessage(message)).ToArray();
                var functions = functionDefinitions.SelectMany(lookup => lookup.Select(function => new SerializableFunctionDescriptor(lookup.Key, function))).ToArray();

                script = new ParsedScript(context, root);

                return new SerializableDocumentDescriptor
                           {
                               Tokens = classifiedTokens,
                               Messages = messages,
                               Functions = functions,
                               Plugins = new List<string>(context.GetPlugins().Select(p => p.Name)),
                           };
            }
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
        private bool TryUpdate<T>(SerializableDocumentDescriptor newDocument, SerializableDocumentDescriptor delta, Expression<Func<SerializableDocumentDescriptor, IReadOnlyList<T>>> selector)
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)selector.Body).Member;
            var getValue = selector.Compile();
            var newValue = getValue(newDocument);
            var existingValue = getValue(this.document);

            if (newValue == null || existingValue != null && EnumerableComparer.Equals(newValue, existingValue))
            {
                return false;
            }

            propertyInfo.SetValue(delta, newValue);
            propertyInfo.SetValue(this.document, newValue);

            return true;
        }
    }
}