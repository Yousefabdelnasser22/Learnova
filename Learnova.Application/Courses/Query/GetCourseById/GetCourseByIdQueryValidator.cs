using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Courses.Query.GetCourseById
{
    public class GetCourseByIdQueryValidator:AbstractValidator<GetCourseByIdQuery>
    {
        public GetCourseByIdQueryValidator()
        {
            RuleFor(x => x.Id)
               .GreaterThan(0).WithMessage("Id must be more than 0");
        }
    }
}
