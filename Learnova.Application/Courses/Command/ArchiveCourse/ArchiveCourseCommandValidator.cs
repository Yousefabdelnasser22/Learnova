using FluentValidation;

namespace Learnova.Application.Courses.Command.ArchiveCourse
{
    public class ArchiveCourseCommandValidator : AbstractValidator<ArchiveCourseCommand>
    {
        public ArchiveCourseCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be more than 0");
        }
    }
}
