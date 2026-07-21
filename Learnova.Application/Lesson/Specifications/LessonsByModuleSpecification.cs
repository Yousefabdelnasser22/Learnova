using Learnova.Domain.Specifications;

namespace Learnova.Application.Lesson.Specifications
{
    using LessonEntity = Learnova.Domain.Entites.Lesson;

    public class LessonsByModuleSpecification : BaseSpecification<LessonEntity>
    {
        public LessonsByModuleSpecification(int moduleId)
            : base(l => l.ModuleId == moduleId)
        {
        }

        public LessonsByModuleSpecification(int moduleId, int pageNumber, int pageSize, string? search)
            : base(l =>
                l.ModuleId == moduleId &&
                (string.IsNullOrWhiteSpace(search) ||
                 l.Title.Contains(search) ||
                 (l.Description != null && l.Description.Contains(search)) ||
                 (l.TextContent != null && l.TextContent.Contains(search)) ||
                 l.Module.Title.Contains(search)))
        {
            AddInclude(l => l.Module);
            AddOrderBy(l => l.Position);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }
    }
}
