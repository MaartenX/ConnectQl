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

namespace ConnectQl.Internal
{
    using ConnectQl.Interfaces;

    /// <summary>
    /// The function registration.
    /// </summary>
    internal class FunctionRegistration : IFunctionRegistration, IFunctionRegistration1, IFunctionRegistration2, IFunctionRegistration3, IFunctionRegistration4, IFunctionRegistration5, IFunctionRegistration6, IFunctionRegistration7, IFunctionRegistration8, IFunctionRegistration9, IFunctionRegistration10
    {
        /// <summary>
        /// The descriptor.
        /// </summary>
        private readonly FunctionDescriptor descriptor;

        /// <summary>
        /// The functions.
        /// </summary>
        private readonly IConnectQlFunctions functions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionRegistration"/> class.
        /// </summary>
        /// <param name="descriptor">
        /// The descriptor.
        /// </param>
        /// <param name="functions">
        /// The functions.
        /// </param>
        internal FunctionRegistration(FunctionDescriptor descriptor, IConnectQlFunctions functions)
        {
            this.descriptor = descriptor;
            this.functions = functions;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger => this.functions.Logger;

        /// <summary>
        /// The set description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description)
        {
            this.descriptor.SetDescription(description);

            return this;
        }

        /// <summary>
        /// The set description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);

            return this;
        }

        /// <summary>
        /// The set description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1, string argument2)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);
            this.descriptor.SetArgumentDescription(1, argument2);

            return this;
        }

        /// <summary>
        /// The set description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <param name="argument3">
        /// The argument 3.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1, string argument2, string argument3)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);
            this.descriptor.SetArgumentDescription(1, argument2);
            this.descriptor.SetArgumentDescription(2, argument3);

            return this;
        }

        /// <summary>
        /// The set description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <param name="argument3">
        /// The argument 3.
        /// </param>
        /// <param name="argument4">
        /// The argument 4.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1, string argument2, string argument3, string argument4)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);
            this.descriptor.SetArgumentDescription(1, argument2);
            this.descriptor.SetArgumentDescription(2, argument3);
            this.descriptor.SetArgumentDescription(3, argument4);

            return this;
        }

        /// <summary>
        /// The set description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <param name="argument3">
        /// The argument 3.
        /// </param>
        /// <param name="argument4">
        /// The argument 4.
        /// </param>
        /// <param name="argument5">
        /// The argument 5.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1, string argument2, string argument3, string argument4, string argument5)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);
            this.descriptor.SetArgumentDescription(1, argument2);
            this.descriptor.SetArgumentDescription(2, argument3);
            this.descriptor.SetArgumentDescription(3, argument4);
            this.descriptor.SetArgumentDescription(4, argument5);

            return this;
        }

        /// <summary>
        /// The set description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <param name="argument3">
        /// The argument 3.
        /// </param>
        /// <param name="argument4">
        /// The argument 4.
        /// </param>
        /// <param name="argument5">
        /// The argument 5.
        /// </param>
        /// <param name="argument6">
        /// The argument 6.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1, string argument2, string argument3, string argument4, string argument5, string argument6)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);
            this.descriptor.SetArgumentDescription(1, argument2);
            this.descriptor.SetArgumentDescription(2, argument3);
            this.descriptor.SetArgumentDescription(3, argument4);
            this.descriptor.SetArgumentDescription(4, argument5);
            this.descriptor.SetArgumentDescription(5, argument6);

            return this;
        }

        /// <summary>
        /// Sets the description for this registration.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <param name="argument3">
        /// The argument 3.
        /// </param>
        /// <param name="argument4">
        /// The argument 4.
        /// </param>
        /// <param name="argument5">
        /// The argument 5.
        /// </param>
        /// <param name="argument6">
        /// The argument 6.
        /// </param>
        /// <param name="argument7">
        /// The argument 7.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1, string argument2, string argument3, string argument4, string argument5, string argument6, string argument7)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);
            this.descriptor.SetArgumentDescription(1, argument2);
            this.descriptor.SetArgumentDescription(2, argument3);
            this.descriptor.SetArgumentDescription(3, argument4);
            this.descriptor.SetArgumentDescription(4, argument5);
            this.descriptor.SetArgumentDescription(5, argument6);
            this.descriptor.SetArgumentDescription(5, argument7);

            return this;
        }

        /// <summary>
        /// Sets the description for this registration.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <param name="argument3">
        /// The argument 3.
        /// </param>
        /// <param name="argument4">
        /// The argument 4.
        /// </param>
        /// <param name="argument5">
        /// The argument 5.
        /// </param>
        /// <param name="argument6">
        /// The argument 6.
        /// </param>
        /// <param name="argument7">
        /// The argument 7.
        /// </param>
        /// <param name="argument8">
        /// The argument 8.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1, string argument2, string argument3, string argument4, string argument5, string argument6, string argument7, string argument8)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);
            this.descriptor.SetArgumentDescription(1, argument2);
            this.descriptor.SetArgumentDescription(2, argument3);
            this.descriptor.SetArgumentDescription(3, argument4);
            this.descriptor.SetArgumentDescription(4, argument5);
            this.descriptor.SetArgumentDescription(5, argument6);
            this.descriptor.SetArgumentDescription(5, argument7);
            this.descriptor.SetArgumentDescription(5, argument8);

            return this;
        }

        /// <summary>
        /// Sets the description for this registration.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <param name="argument3">
        /// The argument 3.
        /// </param>
        /// <param name="argument4">
        /// The argument 4.
        /// </param>
        /// <param name="argument5">
        /// The argument 5.
        /// </param>
        /// <param name="argument6">
        /// The argument 6.
        /// </param>
        /// <param name="argument7">
        /// The argument 7.
        /// </param>
        /// <param name="argument8">
        /// The argument 8.
        /// </param>
        /// <param name="argument9">
        /// The argument 9.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1, string argument2, string argument3, string argument4, string argument5, string argument6, string argument7, string argument8, string argument9)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);
            this.descriptor.SetArgumentDescription(1, argument2);
            this.descriptor.SetArgumentDescription(2, argument3);
            this.descriptor.SetArgumentDescription(3, argument4);
            this.descriptor.SetArgumentDescription(4, argument5);
            this.descriptor.SetArgumentDescription(5, argument6);
            this.descriptor.SetArgumentDescription(5, argument7);
            this.descriptor.SetArgumentDescription(5, argument8);
            this.descriptor.SetArgumentDescription(5, argument9);

            return this;
        }

        /// <summary>
        /// Sets the description for this registration.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <param name="argument3">
        /// The argument 3.
        /// </param>
        /// <param name="argument4">
        /// The argument 4.
        /// </param>
        /// <param name="argument5">
        /// The argument 5.
        /// </param>
        /// <param name="argument6">
        /// The argument 6.
        /// </param>
        /// <param name="argument7">
        /// The argument 7.
        /// </param>
        /// <param name="argument8">
        /// The argument 8.
        /// </param>
        /// <param name="argument9">
        /// The argument 9.
        /// </param>
        /// <param name="argument10">
        /// The argument 10.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions SetDescription(string description, string argument1, string argument2, string argument3, string argument4, string argument5, string argument6, string argument7, string argument8, string argument9, string argument10)
        {
            this.descriptor.SetDescription(description);
            this.descriptor.SetArgumentDescription(0, argument1);
            this.descriptor.SetArgumentDescription(1, argument2);
            this.descriptor.SetArgumentDescription(2, argument3);
            this.descriptor.SetArgumentDescription(3, argument4);
            this.descriptor.SetArgumentDescription(4, argument5);
            this.descriptor.SetArgumentDescription(5, argument6);
            this.descriptor.SetArgumentDescription(5, argument7);
            this.descriptor.SetArgumentDescription(5, argument8);
            this.descriptor.SetArgumentDescription(5, argument9);
            this.descriptor.SetArgumentDescription(5, argument10);

            return this;
        }

        /// <summary>
        /// Adds a key/value pair of key'1 =&gt; function to the dictionary.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when a lambda with the specified number of parameters is already in the dictionary.
        /// </exception>
        /// <returns>
        /// The <see cref="IConnectQlFunctions"/>.
        /// </returns>
        public IConnectQlFunctions AddFunction(string name, IFunctionDescriptor function)
        {
            return this.functions.AddFunction(name, function);
        }
    }
}