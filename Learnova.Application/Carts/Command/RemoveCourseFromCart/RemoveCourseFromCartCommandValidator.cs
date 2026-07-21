using FluentValidation;

namespace Learnova.Application.Carts.Command.RemoveCourseFromCart
{
    public sealed class RemoveCourseFromCartCommandValidator : AbstractValidator<RemoveCourseFromCartCommand>
    {
        public RemoveCourseFromCartCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than 0");
        }
    }
}
