using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Command.UpdateQuiz
{
    public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
    {
        public UpdateQuizCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be more than 0");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200);

            RuleFor(x => x.Questions)
                .NotEmpty().WithMessage("Quiz must have at least one question")
                .Must(q => q.Count <= 50).WithMessage("Max 50 questions allowed");

            RuleForEach(x => x.Questions).ChildRules(question =>
            {
                question.RuleFor(q => q.Question)
                    .NotEmpty().WithMessage("Question text is required");

                question.RuleFor(q => q.Options)
                    .Must(o => o.Count >= 2).WithMessage("Each question must have at least 2 options")
                    .Must(o => o.Count <= 6).WithMessage("Max 6 options per question");

                question.RuleFor(q => q.CorrectAnswerIndex)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("CorrectAnswerIndex must be valid")
                    .Must((dto, index) => index < dto.Options.Count)
                    .WithMessage("CorrectAnswerIndex out of range");
            });
        }
    }
}
