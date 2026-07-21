using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Common.BackgroundJobs
{
    public interface ICourseIndexingJob
    {
        Task IndexCourseAsync(int courseId);
    }
}
