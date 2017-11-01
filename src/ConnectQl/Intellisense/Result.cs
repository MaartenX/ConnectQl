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

namespace ConnectQl.Intellisense
{
    using ConnectQl.Internal.Intellisense.Protocol;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The execute result.
    /// </summary>
    [PublicAPI]
    public static class Result
    {
        /// <summary>
        /// Deserializes an execute result.
        /// </summary>
        /// <param name="bytes">
        /// The bytes to deserialize.
        /// </param>
        /// <returns>The execute result.</returns>
        public static IExecuteResult Deserialize(byte[] bytes)
        {
            return ProtocolSerializer.Deserialize<SerializableExecuteResult>(bytes);
        }

        /// <summary>
        /// Returns an empty execute result.
        /// </summary>
        /// <returns>The empty result.</returns>
        [NotNull]
        public static IExecuteResult Empty()
        {
            return new SerializableExecuteResult { Jobs = new SerializableJob[0], QueryResults = new SerializableQueryResult[0], Warnings = new SerializableMessage[0] };
        }
    }
}