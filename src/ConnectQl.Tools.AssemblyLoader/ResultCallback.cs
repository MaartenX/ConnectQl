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

namespace ConnectQl.Tools.AssemblyLoader
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Used to set a result from a task in a remote domain.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result.
    /// </typeparam>
    public class ResultCallback<T> : MarshalByRefObject
    {
        /// <summary>
        /// Stores the completion source.
        /// </summary>
        private readonly TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();

        /// <summary>
        /// Gets the task for the remote execution.
        /// </summary>
        public Task<T> Task => this.completionSource.Task;

        /// <summary>
        /// Sets the result of the remote execution.
        /// </summary>
        /// <param name="result">
        /// The result to set.
        /// </param>
        public void SetResult(T result)
        {
            this.completionSource.SetResult(result);
        }

        /// <summary>
        /// Sets the exception of the remote execution if it failed.
        /// </summary>
        /// <param name="exception">
        /// The exception to set.
        /// </param>
        public void SetException([NotNull] Exception exception)
        {
            this.completionSource.SetException(exception);
        }
    }
}