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
    using System;

    /// <summary>
    /// The TriggerContext interface.
    /// </summary>
    public interface ITriggerContext
    {
        /// <summary>
        /// Raised when a job was executed.
        /// </summary>
        event EventHandler<JobExecutedArgs> JobExecuted;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILog Log { get; }

        /// <summary>
        /// Activates the trigger.
        /// </summary>
        void Activate();

        /// <summary>
        /// Gets the date and time this job was executed the last time.
        /// </summary>
        /// <param name="jobName">
        /// The name of the job.
        /// </param>
        /// <returns>
        /// The date and time this job was executed, or <c>null</c> if the job was never executed.
        /// </returns>
        DateTime? GetLastExecutionTime(string jobName);
    }
}