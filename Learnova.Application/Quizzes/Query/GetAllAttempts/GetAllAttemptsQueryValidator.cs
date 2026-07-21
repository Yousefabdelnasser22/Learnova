using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Learnova.Application.Quizzes.Query.GetAllAttempts
{
    public class GetAllAttemptsQueryValidator : AbstractValidator<GetAllAttemptsQuery>
    {
        public GetAllAttemptsQueryValidator()
        {
            RuleFor(x => x.QuizId)
                .GreaterThan(0);

            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(100);
        }
    }
}
