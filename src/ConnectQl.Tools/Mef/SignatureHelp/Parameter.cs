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
    using ConnectQl.Interfaces;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// The parameter.
    /// </summary>
    internal class Parameter : IParameter
    {
        /// <summary>
        /// The argument.
        /// </summary>
        private readonly IArgumentDescriptor argument;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="signature">
        /// The signature.
        /// </param>
        /// <param name="argument">
        /// The argument.
        /// </param>
        public Parameter(ISignature signature, IArgumentDescriptor argument)
        {
            this.Signature = signature;
            this.argument = argument;
        }

        /// <summary>
        /// Gets the documentation.
        /// </summary>
        public string Documentation => this.argument.Description;

        /// <summary>
        /// Gets the locus.
        /// </summary>
        public Span Locus { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => this.argument.Name;

        /// <summary>
        /// Gets the pretty printed locus.
        /// </summary>
        public Span PrettyPrintedLocus { get; }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        public ISignature Signature { get; }
    }
}