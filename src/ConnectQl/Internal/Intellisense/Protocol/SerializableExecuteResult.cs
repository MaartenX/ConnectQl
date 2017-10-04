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

namespace ConnectQl.Internal.Intellisense.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    internal class SerializableExecuteResult : IExecuteResult
    {
        public SerializableExecuteResult()
        {
            
        }

        public SerializableExecuteResult(IExecuteResult executeResult)
        {
            this.Jobs = executeResult.Jobs.Select(job => new SerializableJob(job)).ToArray();
            this.QueryResults = executeResult.QueryResults.Select(result => new SerializableQueryResult(result)).ToArray();
            this.Warnings = executeResult.Warnings.Select(warning => new SerializableMessage(warning)).ToArray();
        }

        public SerializableJob[] Jobs { get; set; }

        public SerializableQueryResult[] QueryResults { get; set; }

        public SerializableMessage[] Warnings { get; set; }

        IReadOnlyList<IJob> IExecuteResult.Jobs => this.Jobs;

        IReadOnlyList<IQueryResult> IExecuteResult.QueryResults => this.QueryResults;

        IReadOnlyList<IMessage> IExecuteResult.Warnings => this.Warnings;
    }

    internal class SerializableQueryResult : IQueryResult
    {
        public SerializableQueryResult()
        {
            
        }

        public SerializableQueryResult(IQueryResult result)
        {
            this.AffectedRecords = result.AffectedRecords;
            this.Rows = new string[][] { };
        }

        public long AffectedRecords { get; set; }

        public string[][] Rows { get; set; }

        IAsyncEnumerable<Row> IQueryResult.Rows { get; }
    }

    internal class SerializableJob : IJob
    {
        public SerializableJob()
        {
            
        }

        public SerializableJob(IJob job)
        {
            this.Name = job.Name;
            this.Triggers = job.Triggers.Select(trigger => new SerializableJobTrigger(trigger)).ToArray();
        }

        public string Name { get; set; }

        public SerializableJobTrigger[] Triggers { get; set; }

        IReadOnlyList<IJobTrigger> IJob.Triggers => this.Triggers;

        public Task<IExecuteResult> RunAsync(IJobContext jobContext)
        {
            throw new NotSupportedException();
        }
    }

    internal class SerializableJobTrigger : IJobTrigger
    {
        public SerializableJobTrigger()
        {
            
        }

        public SerializableJobTrigger(IJobTrigger trigger)
        {
            this.Name = trigger.Name;
        }

        public string Name { get; set; }

        public void Disable(ITriggerContext context)
        {
            throw new NotSupportedException();
        }

        public void Enable(ITriggerContext context)
        {
            throw new NotSupportedException();
        }
    }
}