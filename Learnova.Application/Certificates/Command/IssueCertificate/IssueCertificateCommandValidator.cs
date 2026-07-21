using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Certificates.Command.IssueCertificate
{
    public class IssueCertificateCommandValidator:AbstractValidator<IssueCertificateCommand>
        
    {
        public IssueCertificateCommandValidator()
        {

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be more than 0");
        }
    }
}
