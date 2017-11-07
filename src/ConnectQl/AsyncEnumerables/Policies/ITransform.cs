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

namespace ConnectQl.AsyncEnumerables.Policies
{
    using System;

    /// <summary>
    /// Allows transforming non-serializable objects to something that can be serialized.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items to transform.
    /// </typeparam>
    public interface ITransform<T>
    {
        /// <summary>
        ///     Gets the target type values will be transformed to.
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        ///     Creates a transformation context.
        ///     This context will be used in all calls to Serialize and Deserialize.
        /// </summary>
        /// <returns>
        ///     The context that will be disposed when the transformation is no longer needed.
        /// </returns>
        IDisposable CreateContext();

        /// <summary>
        ///     Transforms the value to the serializable object.
        /// </summary>
        /// <param name="context">
        ///     The context in which this value is transformed.
        /// </param>
        /// <param name="value">
        ///     The value to transform.
        /// </param>
        /// <returns>
        ///     A serializable version of the value.
        /// </returns>
        object Serialize(IDisposable context, T value);

        /// <summary>
        ///     Transforms a serializable object to a value.
        /// </summary>
        /// <param name="context">
        ///     The context in which this serializable object is transformed.
        /// </param>
        /// <param name="value">
        ///     The serializable object to transform.
        /// </param>
        /// <returns>
        ///     The value.
        /// </returns>
        T Deserialize(IDisposable context, object value);
    }
}