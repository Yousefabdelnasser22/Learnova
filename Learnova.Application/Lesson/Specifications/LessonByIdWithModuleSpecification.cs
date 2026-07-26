using Learnova.Domain.Specifications;

namespace Learnova.Application.Lesson.Specifications
{
    using LessonEntity = Learnova.Domain.Entities.Lesson;

    public class LessonByIdWithModuleSpecification : BaseSpecification<LessonEntity>
    {
        public LessonByIdWithModuleSpecification(int lessonId)
            : base(lesson => lesson.Id == lessonId)
        {
            AddInclude(lesson => lesson.Module);
        }
    }
}
