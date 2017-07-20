﻿// MIT License
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

namespace ConnectQl.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Intellisense;
    using ConnectQl.Internal.Intellisense.Protocol;

    /// <summary>
    /// The <c>Intellisense</c> session.
    /// </summary>
    public class IntellisenseSession
    {
        /// <summary>
        /// The documents.
        /// </summary>
        private readonly Dictionary<string, Document> documents = new Dictionary<string, Document>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="IntellisenseSession"/> class.
        /// </summary>
        /// <param name="pluginResolver">
        /// The plugin resolver.
        /// </param>
        public IntellisenseSession(IPluginResolver pluginResolver)
        {
            this.Plugins = pluginResolver?.EnumerateAvailablePlugins()?.ToArray() ?? new IConnectQlPlugin[0];

            this.Context = new ConnectQlContext(pluginResolver);
        }

        /// <summary>
        /// The classification changed.
        /// </summary>
        public event EventHandler<Tuple<string, byte[]>> ClassificationChanged;

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        public IConnectQlPlugin[] Plugins { get; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        internal ConnectQlContext Context { get; }

        /// <summary>
        /// Deserializes a serialized document.
        /// </summary>
        /// <param name="serializedDocument">
        /// The serialized document.
        /// </param>
        /// <returns>
        /// The <see cref="IDocumentDescriptor"/>.
        /// </returns>
        public static IDocumentDescriptor Deserialize(byte[] serializedDocument)
        {
            return ProtocolSerializer.Deserialize<SerializableDocumentDescriptor>(serializedDocument);
        }

        /// <summary>
        /// Gets the document by its path.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public string GetDocument(string filename)
        {
            return this.documents.ContainsKey(filename) ? filename : null;
        }

        /// <summary>
        /// Removes the document.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void RemoveDocument(string filename)
        {
            this.documents.Remove(filename);
        }

        /// <summary>
        /// Updates the document.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="contents">
        /// The contents.
        /// </param>
        public void UpdateDocument(string filename, string contents)
        {
            if (this.documents.TryGetValue(filename, out Document doc))
            {
                doc.Contents = contents;
            }
            else
            {
                this.documents[filename] = new Document(this, filename, contents);
            }
        }

        /// <summary>
        /// Updates a span in the document.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="endIndex">
        /// The end index.
        /// </param>
        /// <param name="span">
        /// The span.
        /// </param>
        public void UpdateDocumentSpan(string filename, int startIndex, int endIndex, string span)
        {
            if (!this.documents.TryGetValue(filename, out Document doc))
            {
                return;
            }

            var contents = doc.Contents;

            doc.Contents = contents.Substring(0, startIndex) + span + contents.Substring(endIndex);
        }

        /// <summary>
        /// Sends the classification changed event.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="document">
        /// The serialized document.
        /// </param>
        internal void OnClassificationChanged(string filename, SerializableDocumentDescriptor document)
        {
            this.ClassificationChanged?.Invoke(this, Tuple.Create(filename, ProtocolSerializer.Serialize(document)));
        }
    }
}