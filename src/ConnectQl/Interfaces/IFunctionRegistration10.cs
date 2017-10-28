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

namespace ConnectQl.Interfaces
{
    using JetBrains.Annotations;

    /// <summary>
    /// A function registration with 10 arguments.
    /// </summary>
    [LocalizationRequired]
    public interface IFunctionRegistration10 : IConnectQlFunctions
    {
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
        IConnectQlFunctions SetDescription(string description, string argument1, string argument2, string argument3, string argument4, string argument5, string argument6, string argument7, string argument8, string argument9, string argument10);
    }
}