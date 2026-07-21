using FluentValidation;

namespace Learnova.Application.Quizzes.Command.DeleteQuestion
{
    public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
    {
        public DeleteQuestionCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be more than 0");
        }
    }
}
