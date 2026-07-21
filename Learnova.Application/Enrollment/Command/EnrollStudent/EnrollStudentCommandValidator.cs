using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Command.EnrollStudent
{
    public class EnrollStudentCommandValidator:AbstractValidator<EnrollStudentCommand>
    {
        public EnrollStudentCommandValidator()
        {
            RuleFor(x => x.CourseId)
            .GreaterThan(0);

        }
    }
}
