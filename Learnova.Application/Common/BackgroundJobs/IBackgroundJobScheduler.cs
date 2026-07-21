using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Common.BackgroundJobs
{
    public interface IBackgroundJobScheduler
    {
        string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall);
    }
}
