using Learnova.Domain.Specifications;

namespace Learnova.Application.Modules.Specifications
{
    using ModuleEntity = Learnova.Domain.Entities.Module;

    public class ModulesByCourseSpecification : BaseSpecification<ModuleEntity>
    {
        public ModulesByCourseSpecification(int courseId)
            : base(m => m.CourseId == courseId)
        {
        }

        public ModulesByCourseSpecification(int courseId, int pageNumber, int pageSize, string? search)
            : base(m =>
                m.CourseId == courseId &&
                (string.IsNullOrWhiteSpace(search) ||
                 m.Title.Contains(search) ||
                 (m.Description != null && m.Description.Contains(search))))
        {
            AddInclude(m => m.Course);
            AddOrderBy(m => m.Position);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }
    }
}
