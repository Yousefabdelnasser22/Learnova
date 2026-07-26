using Learnova.Domain.Specifications;

namespace Learnova.Application.Certificates.Specifications
{
    using CertificateEntity = Learnova.Domain.Entities.Certificate;

    public class CertificateByStudentAndCourseSpecification : BaseSpecification<CertificateEntity>
    {
        public CertificateByStudentAndCourseSpecification(string studentId, int courseId)
            : base(c => c.StudentId == studentId && c.CourseId == courseId)
        {
        }
    }
}
