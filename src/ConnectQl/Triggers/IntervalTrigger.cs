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
    using System.Threading;
    using System.Threading.Tasks;

    using ConnectQl.Interfaces;

    /// <summary>
    /// The interval trigger.
    /// </summary>
    internal class IntervalTrigger : ITrigger
    {
        /// <summary>
        /// The interval.
        /// </summary>
        private readonly TimeSpan interval;

        /// <summary>
        /// The token.
        /// </summary>
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalTrigger"/> class.
        /// </summary>
        /// <param name="interval">
        /// The interval.
        /// </param>
        public IntervalTrigger(TimeSpan interval)
        {
            this.interval = interval;
        }

        /// <summary>
        /// Disables the trigger.
        /// </summary>
        /// <param name="context">
        /// The trigger context.
        /// </param>
        public void Disable(ITriggerContext context)
        {
            this.tokenSource.Cancel();
            this.tokenSource = null;
        }

        /// <summary>
        /// Enables the trigger.
        /// </summary>
        /// <param name="context">
        /// The trigger context.
        /// </param>
        public void Enable(ITriggerContext context)
        {
            if (this.tokenSource != null)
            {
                return;
            }

            this.tokenSource = new CancellationTokenSource();

            Func<Task> wait = async () =>
                {
                    while (!this.tokenSource.IsCancellationRequested)
                    {
                        await Task.Delay(this.interval, this.tokenSource.Token).ConfigureAwait(false);

                        if (!this.tokenSource.IsCancellationRequested)
                        {
                            context.Activate();
                        }
                    }
                };

            wait();
        }
    }
}