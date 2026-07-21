using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Lesson.Query.GetLessonById
{
    public class GetLessonByIdQueryValidator:AbstractValidator<GetLessonByIdQuery>
    {
        public GetLessonByIdQueryValidator()
        {

            RuleFor(x => x.Id)
            .GreaterThan(0);

            RuleFor(x => x.CourseId)
            .GreaterThan(0);

            RuleFor(x => x.ModuleId)
            .GreaterThan(0);


        }
    }
}
