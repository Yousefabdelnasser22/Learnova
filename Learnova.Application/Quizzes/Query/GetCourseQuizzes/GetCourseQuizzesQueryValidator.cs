using FluentValidation;
using Learnova.Application.Modules.Query.GetAllModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Query.GetCourseQuizzes
{
    public class GetCourseQuizzesQueryValidator : AbstractValidator<GetCourseQuizzesQuery>
    {
        public GetCourseQuizzesQueryValidator()
        {

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Id must be more than 0");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(100);
        }
    }
}
