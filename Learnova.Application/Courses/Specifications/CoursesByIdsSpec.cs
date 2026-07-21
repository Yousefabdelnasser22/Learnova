using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Courses.Specifications
{
    public class CoursesByIdsSpec : BaseSpecification<Course>
    {
        public CoursesByIdsSpec(IEnumerable<int> courseIds)
            : base(c => courseIds.Contains(c.Id) && c.Status == CourseStatus.Published)
        {
            AddInclude(c => c.Instructor);
            AddInclude(c => c.SubCategory);
            AddInclude(c => c.SubCategory.Category);
        }
    }
}
