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

namespace ConnectQl.Triggers
{
    using System;

    using ConnectQl.Interfaces;

    /// <summary>
    /// The after job trigger.
    /// </summary>
    internal class AfterJobTrigger : ITrigger
    {
        /// <summary>
        /// The job name.
        /// </summary>
        private readonly string jobName;

        /// <summary>
        /// Stores the event handler.
        /// </summary>
        private EventHandler<JobExecutedArgs> handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterJobTrigger"/> class.
        /// </summary>
        /// <param name="jobName">
        /// The job name.
        /// </param>
        public AfterJobTrigger(string jobName)
        {
            this.jobName = jobName;
        }

        /// <summary>
        /// Disables the trigger.
        /// </summary>
        /// <param name="context">
        /// The trigger context.
        /// </param>
        public void Disable(ITriggerContext context)
        {
            if (this.handler == null)
            {
                return;
            }

            context.JobExecuted -= this.handler;

            this.handler = null;
        }

        /// <summary>
        /// Enables the trigger.
        /// </summary>
        /// <param name="context">
        /// The trigger context.
        /// </param>
        public void Enable(ITriggerContext context)
        {
            if (this.handler != null)
            {
                this.Disable(context);
            }

            this.handler = (o, e) =>
                {
                    if (string.Equals(this.jobName, e.JobName, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Activate();
                    }
                };

            context.JobExecuted += this.handler;
        }
    }
}