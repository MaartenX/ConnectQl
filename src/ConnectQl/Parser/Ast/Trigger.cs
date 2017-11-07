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

namespace ConnectQl.Parser.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Expressions;

    using JetBrains.Annotations;

    using Visitors;

    /// <summary>
    /// The trigger.
    /// </summary>
    internal class Trigger : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> class.
        /// </summary>
        /// <param name="after">
        /// The after.
        /// </param>
        public Trigger(string after)
        {
            var arguments = new ConnectQlExpressionBase[]
                                {
                                    new ConstConnectQlExpression(after),
                                };

            this.Function = new FunctionCallConnectQlExpression("triggerafter", new ReadOnlyCollection<ConnectQlExpressionBase>(arguments));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> class.
        /// </summary>
        /// <param name="interval">
        /// The interval.
        /// </param>
        public Trigger(TimeSpan interval)
        {
            var arguments = new ConnectQlExpressionBase[]
                                {
                                    new ConstConnectQlExpression(interval),
                                };

            this.Function = new FunctionCallConnectQlExpression("triggerevery", new ReadOnlyCollection<ConnectQlExpressionBase>(arguments));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> class.
        /// </summary>
        /// <param name="function">
        /// The function.
        /// </param>
        public Trigger(FunctionCallConnectQlExpression function)
        {
            this.Function = function;
        }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public override IEnumerable<Node> Children
        {
            get
            {
                yield return this.Function;
            }
        }

        /// <summary>
        /// Gets the function.
        /// </summary>
        public FunctionCallConnectQlExpression Function { get; }

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
            return visitor.VisitTrigger(this);
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
            var function = visitor.Visit(this.Function);

            return !object.ReferenceEquals(this.Function, function)
                       ? new Trigger(function)
                       : this;
        }
    }
}