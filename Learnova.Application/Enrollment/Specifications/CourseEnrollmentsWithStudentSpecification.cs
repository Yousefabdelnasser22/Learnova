using Learnova.Domain.Specifications;

namespace Learnova.Application.Enrollment.Specifications
{
    using EnrollmentEntity = Learnova.Domain.Entities.Enrollment;

    public class CourseEnrollmentsWithStudentSpecification : BaseSpecification<EnrollmentEntity>
    {
        public CourseEnrollmentsWithStudentSpecification(int courseId)
            : base(e => e.CourseId == courseId)
        {
            AddInclude(e => e.Student);
            AddOrderByDescending(e => e.EnrolledAt);
        }

        public CourseEnrollmentsWithStudentSpecification(int courseId, int pageNumber, int pageSize, string? search)
            : base(e =>
                e.CourseId == courseId &&
                (string.IsNullOrWhiteSpace(search) ||
                 e.StudentId.Contains(search) ||
                 (e.Student.Email != null && e.Student.Email.Contains(search)) ||
                 (e.Student.UserName != null && e.Student.UserName.Contains(search))))
        {
            AddInclude(e => e.Student);
            AddOrderByDescending(e => e.EnrolledAt);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }
    }
}
