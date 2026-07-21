using Learnova.Domain.Entites;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Lesson.Specifications
{
    public class CompletedLessonProgressByCourseSpecification : BaseSpecification<LessonProgress>
    {
        public CompletedLessonProgressByCourseSpecification(string studentId, int courseId)
            : base(lp => lp.Lesson.Module.CourseId == courseId
                        && lp.StudentId == studentId
                        && lp.IsCompleted
                        && !lp.Lesson.IsDeleted
                        && !lp.Lesson.Module.IsDeleted)
        {
        }
    }
}
