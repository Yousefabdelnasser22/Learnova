using FluentValidation;

namespace Learnova.Application.Courses.Command.SubmitCourseForReview
{
    public class SubmitCourseForReviewCommandValidator : AbstractValidator<SubmitCourseForReviewCommand>
    {
        public SubmitCourseForReviewCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be more than 0");
        }
    }
}
