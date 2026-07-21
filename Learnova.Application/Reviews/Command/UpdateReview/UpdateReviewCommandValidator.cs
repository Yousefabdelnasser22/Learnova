using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Command.UpdateReview
{
    public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .GreaterThan(0).WithMessage("ReviewId must be more than 0");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Comment));
        }
    }
}
