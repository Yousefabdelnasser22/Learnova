using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Command.DeleteReview
{
    public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
    {
        public DeleteReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .GreaterThan(0).WithMessage("ReviewId must be more than 0");
        }
    }
}
