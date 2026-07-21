using FluentValidation;

namespace Learnova.Application.User.Command.UpdateUserDetail
{
    public class UpdateUserDetailCommandValidator : AbstractValidator<UpdateUserDetailCommand>
    {
        public UpdateUserDetailCommandValidator()
        {
            RuleFor(x => x.City)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.City));

            RuleFor(x => x.Age)
                .InclusiveBetween(0, 120)
                .When(x => x.Age.HasValue);
        }
    }
}
