using Learnova.Domain.Specifications;

namespace Learnova.Application.Certificates.Specifications
{
    using CertificateEntity = Learnova.Domain.Entities.Certificate;

    public class CertificateByNumberSpecification : BaseSpecification<CertificateEntity>
    {
        public CertificateByNumberSpecification(string certificateNo)
            : base(c => c.CertificateNo == certificateNo)
        {
        }
    }
}
