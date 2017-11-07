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

    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The serializable job trigger.
    /// </summary>
    internal class SerializableJobTrigger : IJobTrigger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableJobTrigger"/> class.
        /// </summary>
        [UsedImplicitly]
        public SerializableJobTrigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableJobTrigger"/> class.
        /// </summary>
        /// <param name="trigger">
        /// The trigger.
        /// </param>
        public SerializableJobTrigger([NotNull] IJobTrigger trigger)
        {
            this.Name = trigger.Name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; [UsedImplicitly] set; }

        /// <summary>
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Always thrown.
        /// </exception>
        public void Disable(ITriggerContext context)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Always thrown.
        /// </exception>
        public void Enable(ITriggerContext context)
        {
            throw new NotSupportedException();
        }
    }
}