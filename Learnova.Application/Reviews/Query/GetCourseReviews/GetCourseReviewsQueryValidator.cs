using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Query.GetCourseReviews
{
    public class GetCourseReviewsQueryValidator : AbstractValidator<GetCourseReviewsQuery>
    {
        public GetCourseReviewsQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be more than 0");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(100);
        }
    }
}
