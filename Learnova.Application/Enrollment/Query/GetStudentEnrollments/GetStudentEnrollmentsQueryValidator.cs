using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Query.GetStudentEnrollments
{
    public class GetStudentEnrollmentsQueryValidator : AbstractValidator<GetStudentEnrollmentsQuery>
    {
        public GetStudentEnrollmentsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(100);
        }
    }
}
