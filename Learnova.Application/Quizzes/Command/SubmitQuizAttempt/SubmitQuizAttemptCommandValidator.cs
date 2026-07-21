using FluentValidation;
namespace Learnova.Application.Quizzes.Command.SubmitQuizAttempt
{
    public class SubmitQuizAttemptCommandValidator : AbstractValidator<SubmitQuizAttemptCommand>
    {
        public SubmitQuizAttemptCommandValidator()
        {
            RuleFor(x => x.QuizId)
                .GreaterThan(0).WithMessage("QuizId must be greater than 0");

            RuleFor(x => x.Answers)
                .NotEmpty().WithMessage("Answers collection cannot be empty");

            RuleForEach(x => x.Answers)
                .ChildRules(answer =>
                {
                    answer.RuleFor(x => x.QuizQuestionId)
                        .GreaterThan(0).WithMessage("QuizQuestionId must be greater than 0");

                    answer.RuleFor(x => x.ChosenAnswerIndex)
                        .GreaterThanOrEqualTo(0).WithMessage("ChosenAnswerIndex must be greater than or equal to 0");
                });
        }
    }
}
