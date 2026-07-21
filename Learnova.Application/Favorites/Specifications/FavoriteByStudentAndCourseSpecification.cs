using Learnova.Domain.Entites;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Favorites.Specifications
{
    public class FavoriteByStudentAndCourseSpecification : BaseSpecification<FavoriteList>
    {
        public FavoriteByStudentAndCourseSpecification(string studentId, int courseId)
            : base(x => x.StudentId == studentId && x.CourseId == courseId)
        {
            AddInclude(x => x.Course);
        }
    }
}
