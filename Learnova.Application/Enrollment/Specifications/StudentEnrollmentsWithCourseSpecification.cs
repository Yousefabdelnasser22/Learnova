using Learnova.Domain.Specifications;

namespace Learnova.Application.Enrollment.Specifications
{
    using EnrollmentEntity = Learnova.Domain.Entites.Enrollment;

    public class StudentEnrollmentsWithCourseSpecification : BaseSpecification<EnrollmentEntity>
    {
        public StudentEnrollmentsWithCourseSpecification(string studentId)
            : base(e => e.StudentId == studentId)
        {
            AddInclude(e => e.Course);
            AddOrderByDescending(e => e.EnrolledAt);
        }

        public StudentEnrollmentsWithCourseSpecification(string studentId, int pageNumber, int pageSize, string? search)
            : base(e =>
                e.StudentId == studentId &&
                (string.IsNullOrWhiteSpace(search) ||
                 e.Course.Title.Contains(search) ||
                 (e.Course.Description != null && e.Course.Description.Contains(search))))
        {
            AddInclude(e => e.Course);
            AddOrderByDescending(e => e.EnrolledAt);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }
    }
}
