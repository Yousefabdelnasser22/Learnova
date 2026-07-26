using Learnova.Domain.Specifications;

namespace Learnova.Application.Enrollment.Specifications
{
    using EnrollmentEntity = Learnova.Domain.Entities.Enrollment;

    public class EnrollmentByStudentAndCourseSpecification : BaseSpecification<EnrollmentEntity>
    {
        public EnrollmentByStudentAndCourseSpecification(string studentId, int courseId)
            : base(e => e.StudentId == studentId && e.CourseId == courseId)
        {
        }
    }
}
