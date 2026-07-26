using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Lesson.Specifications
{
    public class CompletedLessonProgressByModuleSpecification : BaseSpecification<LessonProgress>
    {
        public CompletedLessonProgressByModuleSpecification(string studentId, int moduleId)
            : base(lp => lp.Lesson.ModuleId == moduleId
                        && lp.StudentId == studentId
                        && lp.IsCompleted
                        && !lp.Lesson.IsDeleted
                        && !lp.Lesson.Module.IsDeleted)
        {
        }
    }
}
