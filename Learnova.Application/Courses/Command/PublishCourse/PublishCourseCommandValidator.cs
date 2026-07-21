using FluentValidation;

namespace Learnova.Application.Courses.Command.PublishCourse
{
    public class PublishCourseCommandValidator : AbstractValidator<PublishCourseCommand>
    {
        public PublishCourseCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be more than 0");
        }
    }
}
