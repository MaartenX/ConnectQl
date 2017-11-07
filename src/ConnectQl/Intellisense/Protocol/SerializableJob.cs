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

namespace ConnectQl.Intellisense.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The serializable job.
    /// </summary>
    internal class SerializableJob : IJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableJob"/> class.
        /// </summary>
        [UsedImplicitly]
        public SerializableJob()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableJob"/> class.
        /// </summary>
        /// <param name="job">
        /// The job.
        /// </param>
        public SerializableJob(IJob job)
        {
            this.Name = job.Name;
            this.Triggers = job.Triggers.Select(trigger => new SerializableJobTrigger(trigger)).ToArray();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; [UsedImplicitly] set; }

        /// <summary>
        /// Gets or sets the triggers.
        /// </summary>
        [UsedImplicitly]
        public SerializableJobTrigger[] Triggers { get; set; }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        IReadOnlyList<IJobTrigger> IJob.Triggers => this.Triggers;

        /// <summary>
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="jobContext">
        /// The job context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Always thrown.
        /// </exception>
        public Task<IExecuteResult> RunAsync(IJobContext jobContext)
        {
            throw new NotSupportedException();
        }
    }
}