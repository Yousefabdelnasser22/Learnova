using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Lesson.Query.GetAllLesson
{
    public class GetAllLessonQueryValidator :AbstractValidator<GetAllLessonQuery>
    {
        public GetAllLessonQueryValidator()
        {
          RuleFor(x => x.CourseId)
         .GreaterThan(0);

            RuleFor(x => x.ModuleId)
            .GreaterThan(0);

            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(100);
        }

    }
}
