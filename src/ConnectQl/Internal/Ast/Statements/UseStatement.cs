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

namespace ConnectQl.Internal.Ast.Statements
{
    using System.Collections.Generic;

    using ConnectQl.Internal.Ast.Expressions;
    using ConnectQl.Internal.Ast.Visitors;

    using JetBrains.Annotations;

    /// <summary>
    /// The use statement.
    /// </summary>
    internal class UseStatement : StatementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseStatement"/> class.
        /// </summary>
        /// <param name="settingFunction">
        /// The expression.
        /// </param>
        /// <param name="functionName">
        /// The function Uri.
        /// </param>
        public UseStatement(FunctionCallSqlExpression settingFunction, string functionName)
        {
            this.SettingFunction = settingFunction;
            this.FunctionName = functionName;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public override IEnumerable<Node> Children
        {
            get
            {
                yield return this.SettingFunction;
            }
        }

        /// <summary>
        /// Gets the function name.
        /// </summary>
        public string FunctionName { get; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public FunctionCallSqlExpression SettingFunction { get; }

        /// <summary>
        /// Dispatches the visitor to the correct visit-method.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        protected internal override Node Accept([NotNull] NodeVisitor visitor)
        {
            return visitor.VisitUseStatement(this);
        }

        /// <summary>
        /// Visits the children of this node.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        [NotNull]
        protected internal override Node VisitChildren([NotNull] NodeVisitor visitor)
        {
            var settingFunction = visitor.Visit(this.SettingFunction);

            return !object.ReferenceEquals(settingFunction, this.SettingFunction) ? new UseStatement(settingFunction, this.FunctionName) : this;
        }
    }
}