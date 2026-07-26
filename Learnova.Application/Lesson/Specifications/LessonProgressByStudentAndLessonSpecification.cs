using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Lesson.Specifications
{
    public class LessonProgressByStudentAndLessonSpecification : BaseSpecification<LessonProgress>
    {
        public LessonProgressByStudentAndLessonSpecification(string studentId, int lessonId)
            : base(lp => lp.StudentId == studentId && lp.LessonId == lessonId)
        {
        }
    }
}
