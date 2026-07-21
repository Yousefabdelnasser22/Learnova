using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Command.DeleteQuiz
{
    public class DeleteQuizCommandValidator : AbstractValidator<DeleteQuizCommand>
    {
        public DeleteQuizCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be more than 0");
        }
    }
}
