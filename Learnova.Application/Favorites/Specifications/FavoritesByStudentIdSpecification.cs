using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Favorites.Specifications
{
    public class FavoritesByStudentIdSpecification : BaseSpecification<FavoriteList>
    {
        public FavoritesByStudentIdSpecification(string studentId)
            : base(x => x.StudentId == studentId && !x.Course.IsDeleted && x.Course.Status == CourseStatus.Published)
        {
            AddInclude(x => x.Course);
            AddOrderByDescending(x => x.AddedAt);
        }
    }
}
