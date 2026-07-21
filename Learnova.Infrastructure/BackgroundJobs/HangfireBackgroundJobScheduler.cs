using Hangfire;
using Learnova.Application.Common.BackgroundJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.BackgroundJobs
{
    public sealed class HangfireBackgroundJobScheduler : IBackgroundJobScheduler
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HangfireBackgroundJobScheduler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall)
        {
            return _backgroundJobClient.Enqueue(methodCall);
        }
    }
}
