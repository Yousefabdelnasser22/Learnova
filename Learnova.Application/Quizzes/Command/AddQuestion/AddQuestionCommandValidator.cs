using FluentValidation;

namespace Learnova.Application.Quizzes.Command.AddQuestion
{
    public class AddQuestionCommandValidator : AbstractValidator<AddQuestionCommand>
    {
        public AddQuestionCommandValidator()
        {
            RuleFor(x => x.QuizId)
                .GreaterThan(0).WithMessage("Id must be more than 0");

            RuleFor(x => x.Question)
                .NotEmpty().WithMessage("Question text is required");

            RuleFor(x => x.Options)
                .Must(o => o.Count >= 2).WithMessage("Each question must have at least 2 options")
                .Must(o => o.Count <= 6).WithMessage("Max 6 options per question");

            RuleFor(x => x.CorrectAnswerIndex)
                .GreaterThanOrEqualTo(0)
                .WithMessage("CorrectAnswerIndex must be valid")
                .Must((dto, index) => index < dto.Options.Count)
                .WithMessage("CorrectAnswerIndex out of range");
        }
    }
}
