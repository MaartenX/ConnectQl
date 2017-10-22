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

namespace ConnectQl.Internal.Query
{
    using System.Collections.Generic;

    using ConnectQl.Internal.Ast;
    using ConnectQl.Internal.Ast.Sources;
    using ConnectQl.Internal.Ast.Visitors;

    using JetBrains.Annotations;

    /// <summary>
    /// Retrieves all source aliases in a node.
    /// </summary>
    internal class SourceAliasesRetriever : NodeVisitor
    {
        /// <summary>
        /// The aliases.
        /// </summary>
        private readonly HashSet<string> aliases = new HashSet<string>();

        /// <summary>
        /// Prevents a default instance of the <see cref="SourceAliasesRetriever"/> class from being created.
        /// </summary>
        private SourceAliasesRetriever()
        {
        }

        /// <summary>
        /// Gets all source aliases.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The aliases.
        /// </returns>
        public static IEnumerable<string> GetAllSources(Node node)
        {
            var retriever = new SourceAliasesRetriever();

            retriever.Visit(node);

            return retriever.aliases;
        }

        /// <summary>
        /// Visits a <see cref="FunctionSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitFunctionSource([NotNull] FunctionSource node)
        {
            this.aliases.Add(node.Alias);

            return base.VisitFunctionSource(node);
        }

        /// <summary>
        /// Visits a <see cref="SelectSource"/>.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The node, or a new version of the node.
        /// </returns>
        protected internal override Node VisitSelectSource([NotNull] SelectSource node)
        {
            this.aliases.Add(node.Alias);

            return base.VisitSelectSource(node);
        }
    }
}