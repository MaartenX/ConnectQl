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

namespace ConnectQl.Internal.Interfaces
{
    using ConnectQl.Internal.Ast;
    using ConnectQl.Results;

    /// <summary>
    /// The ParseResult interface.
    /// </summary>
    internal interface IResultBuilder : IResult
    {
        /// <summary>
        /// Adds data to the specified node.
        /// </summary>
        /// <param name="node">
        /// The node to add data to.
        /// </param>
        /// <param name="type">
        /// The type of the data.
        /// </param>
        /// <param name="data">
        /// The data to add to the node.
        /// </param>
        /// <typeparam name="T">
        /// The type of the data to write.
        /// </typeparam>
        /// <returns>
        /// The data that was added.
        /// </returns>
        T AddNodeData<T>(Node node, string type, T data);

        /// <summary>
        /// Removes the data from the node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="type">
        /// The type of the data to remove.
        /// </param>
        /// <returns>
        /// <c>true</c> if the node was removed, <c>false</c> otherwise.
        /// </returns>
        bool RemoveNodeData(Node node, string type);

        /// <summary>
        /// Transfers the data from one node to another.
        /// </summary>
        /// <param name="from">
        /// The node to transfer all data from.
        /// </param>
        /// <param name="to">
        /// The node to transfer all data to.
        /// </param>
        /// <param name="overwrite">
        /// When <c>true</c>, overwrites duplicate values in <paramref name="to"/>, when <c>false</c>, keeps the original
        ///     values.
        /// </param>
        /// <returns>
        /// <c>true</c> if data was moved, <c>false</c> otherwise.
        /// </returns>
        bool TransferData(Node from, Node to, bool overwrite);

        /// <summary>
        /// Adds an error to the result.
        /// </summary>
        /// <param name="start">
        /// The start position where the error was found.
        /// </param>
        /// <param name="end">
        /// The end position where the error was found.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void AddError(Position start, Position end, string message);

        /// <summary>
        /// Adds a warning to the result.
        /// </summary>
        /// <param name="start">
        /// The start position where the warning was found.
        /// </param>
        /// <param name="end">
        /// The end position where the warning was found.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void AddWarning(Position start, Position end, string message);

        /// <summary>
        /// Adds an information message to the result.
        /// </summary>
        /// <param name="start">
        /// The start position where the information message was found.
        /// </param>
        /// <param name="end">
        /// The end position where the information message was found.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void AddInformation(Position start, Position end, string message);
    }
}