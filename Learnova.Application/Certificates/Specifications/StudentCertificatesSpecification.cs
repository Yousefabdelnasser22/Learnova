using Learnova.Domain.Enums;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Certificates.Specifications
{
    using CertificateEntity = Learnova.Domain.Entites.Certificate;

    public class StudentCertificatesSpecification : BaseSpecification<CertificateEntity>
    {
        public StudentCertificatesSpecification(string studentId, int pageNumber, int pageSize, string? search)
            : base(c =>
                c.StudentId == studentId &&
                c.Course.Enrollments.Any(e =>
                    !e.IsDeleted &&
                    e.StudentId == studentId &&
                    (e.Status == EnrollmentStatus.Active ||
                     e.Status == EnrollmentStatus.Completed)) &&
                (string.IsNullOrWhiteSpace(search) ||
                 c.CertificateNo.Contains(search) ||
                 c.Course.Title.Contains(search)))
        {
            AddInclude(c => c.Course);
            AddOrderByDescending(c => c.IssuedAt);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }
    }
}
