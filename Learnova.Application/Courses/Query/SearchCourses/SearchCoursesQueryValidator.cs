using FluentValidation;

namespace Learnova.Application.Courses.Query.SearchCourses
{
    public class SearchCoursesQueryValidator : AbstractValidator<SearchCoursesQuery>
    {
        public SearchCoursesQueryValidator()
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty().WithMessage("SearchTerm is required.")
                .MaximumLength(100).WithMessage("SearchTerm must not exceed 100 characters.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 50).WithMessage("Limit must be between 1 and 50.");
        }
    }
}
