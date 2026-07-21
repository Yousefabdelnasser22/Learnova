using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Command.UnenrollStudent
{
    public class UnenrollStudentCommandValidator:AbstractValidator<UnenrollStudentCommand>
    {
        public UnenrollStudentCommandValidator()
        {
            RuleFor(x => x.CourseId)
            .GreaterThan(0);

        }
    }
}
