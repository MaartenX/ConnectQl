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
    /// <summary>
    /// The InternalExecutionContext interface.
    /// </summary>
    internal interface IInternalExecutionContext : IExecutionContext
    {
        /// <summary>
        /// Sets the variable to the specified value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the variable.
        /// </typeparam>
        /// <param name="variable">
        /// The name of the variable, including the '@'.
        /// </param>
        /// <param name="value">
        /// The value of the variable.
        /// </param>
        void SetVariable<T>(string variable, T value);

        /// <summary>
        /// Sets the function name for the specified data access.
        /// </summary>
        /// <param name="access">
        /// The data access to set the name for.
        /// </param>
        /// <param name="functionName">
        /// The function name.
        /// </param>
        void SetFunctionName(IDataAccess access, string functionName);

        /// <summary>
        /// Sets the display name.
        /// </summary>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        void SetDisplayName(IDataAccess access, string displayName);

        /// <summary>
        /// Registers a default value for the specified target function and function name.
        /// </summary>
        /// <param name="setting">
        /// The setting to register.
        /// </param>
        /// <param name="functionName">
        /// The function name to register the setting for.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        void RegisterDefault(string setting, string functionName, object value);

        /// <summary>
        /// Gets the display name for the specified access.
        /// </summary>
        /// <param name="access">
        /// The access.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetDisplayName(IDataAccess access);
    }
}