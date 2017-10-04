﻿// MIT License
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

namespace ConnectQl.Internal.Query.Plans
{
    using System;
    using System.Threading.Tasks;

    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Interfaces;
    using ConnectQl.Internal.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The use default query plan.
    /// </summary>
    internal class UseDefaultQueryPlan : IQueryPlan
    {
        /// <summary>
        /// The function name.
        /// </summary>
        private readonly string functionName;

        /// <summary>
        /// The setting.
        /// </summary>
        private readonly string setting;

        /// <summary>
        /// The value factory.
        /// </summary>
        private readonly Func<IExecutionContext, object> valueFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UseDefaultQueryPlan"/> class.
        /// </summary>
        /// <param name="setting">
        /// The setting.
        /// </param>
        /// <param name="functionName">
        /// The function name.
        /// </param>
        /// <param name="valueFactory">
        /// The value factory.
        /// </param>
        public UseDefaultQueryPlan(string setting, string functionName, Func<IExecutionContext, object> valueFactory)
        {
            this.setting = setting;
            this.functionName = functionName;
            this.valueFactory = valueFactory;
        }

        /// <summary>
        /// Executes the plan.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ExecuteResult"/>.
        /// </returns>
        public Task<ExecuteResult> ExecuteAsync([NotNull] IInternalExecutionContext context)
        {
            context.RegisterDefault(this.setting, this.functionName, this.valueFactory(context));

            return Task.FromResult(new ExecuteResult());
        }
    }
}