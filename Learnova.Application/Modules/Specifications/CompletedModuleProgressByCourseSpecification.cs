using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Modules.Specifications
{
    public class CompletedModuleProgressByCourseSpecification : BaseSpecification<ModuleProgress>
    {
        public CompletedModuleProgressByCourseSpecification(string studentId, int courseId)
            : base(mp => mp.Module.CourseId == courseId
                        && mp.StudentId == studentId
                        && mp.IsCompleted
                        && !mp.Module.IsDeleted)
        {
        }
    }
}
