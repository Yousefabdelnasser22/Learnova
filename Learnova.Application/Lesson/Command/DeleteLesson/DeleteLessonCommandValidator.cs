using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Lesson.Command.DeleteLesson
{
    public class DeleteLessonCommandValidator:AbstractValidator<DeleteLessonCommand>
    {
        public DeleteLessonCommandValidator()
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
