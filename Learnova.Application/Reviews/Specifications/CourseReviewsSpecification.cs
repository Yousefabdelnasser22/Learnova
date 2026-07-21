using Learnova.Domain.Enums;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Reviews.Specifications
{
    using ReviewEntity = Learnova.Domain.Entites.Review;

    public class CourseReviewsSpecification : BaseSpecification<ReviewEntity>
    {
        public CourseReviewsSpecification(int courseId, int pageNumber, int pageSize, string? search)
            : base(r =>
                r.CourseId == courseId &&
                r.Course.Enrollments.Any(e =>
                    !e.IsDeleted &&
                    e.StudentId == r.StudentId &&
                    (e.Status == EnrollmentStatus.Active ||
                     e.Status == EnrollmentStatus.Completed)) &&
                (string.IsNullOrWhiteSpace(search) ||
                 (r.Comment != null && r.Comment.Contains(search)) ||
                 (r.Student.Email != null && r.Student.Email.Contains(search)) ||
                 (r.Student.UserName != null && r.Student.UserName.Contains(search))))
        {
            AddInclude(r => r.Student);
            AddOrderByDescending(r => r.CreatedAt);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }
    }
}
