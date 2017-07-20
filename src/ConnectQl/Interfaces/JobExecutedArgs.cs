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
    /// The job executed args.
    /// </summary>
    public class JobExecutedArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobExecutedArgs"/> class.
        /// </summary>
        /// <param name="jobName">
        /// The job name.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        public JobExecutedArgs(string jobName, DateTime start, DateTime end)
        {
            this.JobName = jobName;
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Gets the execution time.
        /// </summary>
        public TimeSpan Duration => this.Start - this.End;

        /// <summary>
        /// Gets the end time.
        /// </summary>
        public DateTime End { get; }

        /// <summary>
        /// Gets the job name.
        /// </summary>
        public string JobName { get; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        public DateTime Start { get; }
    }
}