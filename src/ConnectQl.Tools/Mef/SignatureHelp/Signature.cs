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

namespace ConnectQl.Tools.Mef.SignatureHelp
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// The signature.
    /// </summary>
    internal class Signature : ISignature
    {
        /// <summary>
        /// The function.
        /// </summary>
        private readonly IFunctionDescriptor function;

        /// <summary>
        /// The current parameter.
        /// </summary>
        private IParameter currentParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <param name="trackingSpan">
        /// The tracking span.
        /// </param>
        public Signature([NotNull] ITextBuffer buffer, IFunctionDescriptor function, ITrackingSpan trackingSpan)
        {
            this.ApplicableToSpan = trackingSpan;
            this.function = function;
            this.Parameters = new ReadOnlyCollection<IParameter>(this.function.Arguments.Select(a => new Parameter(this, a)).ToArray<IParameter>());

            buffer.Changed += (o, e) => this.ComputeCurrentParameter();

            this.ComputeCurrentParameter();
        }

        /// <summary>
        /// The current parameter changed.
        /// </summary>
        public event EventHandler<CurrentParameterChangedEventArgs> CurrentParameterChanged;

        /// <summary>
        /// Gets the applicable to span.
        /// </summary>
        public ITrackingSpan ApplicableToSpan { get; }

        /// <summary>
        /// Getst the content.
        /// </summary>
        [NotNull]
        public string Content => $"{this.function.Name.ToUpperInvariant()} ({string.Join(", ", this.function.Arguments.Select(a => $" {a.Type.SimplifiedType.Name} {a.Name} "))})";

        /// <summary>
        /// Gets the current parameter.
        /// </summary>
        public IParameter CurrentParameter
        {
            get => this.currentParameter;

            private set
            {
                if (this.currentParameter == value)
                {
                    return;
                }

                this.CurrentParameterChanged?.Invoke(this, new CurrentParameterChangedEventArgs(this.currentParameter, value));
                this.currentParameter = value;
            }
        }

        /// <summary>
        /// Gets the documentation.
        /// </summary>
        public string Documentation => this.function.Description;

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public ReadOnlyCollection<IParameter> Parameters { get; }

        /// <summary>
        /// Gets the pretty printed content.
        /// </summary>
        public string PrettyPrintedContent => this.Content;

        /// <summary>
        /// The compute current parameter.
        /// </summary>
        private void ComputeCurrentParameter()
        {
            this.CurrentParameter = this.Parameters.Count == 0 ? null : this.Parameters[0];
        }
    }
}