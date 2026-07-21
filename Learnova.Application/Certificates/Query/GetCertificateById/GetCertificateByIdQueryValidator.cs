using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Certificates.Query.GetCertificateById
{
    public class GetCertificateByIdQueryValidator : AbstractValidator<GetCertificateByIdQuery>
    {
        public GetCertificateByIdQueryValidator()
        {
            RuleFor(x => x.CertificateId)
                .GreaterThan(0).WithMessage("CertificateId must be more than 0");
        }
    }
}
