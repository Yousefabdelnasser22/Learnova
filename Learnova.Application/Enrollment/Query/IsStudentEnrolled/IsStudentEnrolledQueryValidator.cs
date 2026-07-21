using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Query.IsStudentEnrolled
{
    public class IsStudentEnrolledQueryValidator:AbstractValidator<IsStudentEnrolledQuery>
    {
        public IsStudentEnrolledQueryValidator()
        {
            RuleFor(x => x.CourseId)
            .GreaterThan(0);
        }
    }
}
