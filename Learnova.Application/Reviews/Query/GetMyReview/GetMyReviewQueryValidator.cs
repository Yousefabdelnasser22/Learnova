using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Query.GetMyReview
{
    public class GetMyReviewQueryValidator : AbstractValidator<GetMyReviewQuery>
    {
        public GetMyReviewQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be more than 0");
        }
    }
}
