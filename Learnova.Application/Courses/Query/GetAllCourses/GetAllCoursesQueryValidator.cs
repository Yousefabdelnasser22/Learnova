using FluentValidation;

namespace Learnova.Application.Courses.Query.GetAllCourses
{
    public class GetAllCoursesQueryValidator : AbstractValidator<GetAllCoursesQuery>
    {
        public GetAllCoursesQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(100);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0)
                .When(x => x.SubCategoryId.HasValue);

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
                .WithMessage("MinPrice must be less than or equal to MaxPrice.");

            RuleFor(x => x.Level)
                .IsInEnum()
                .When(x => x.Level.HasValue);

            RuleFor(x => x.Sort)
                .Must(BeSupportedSort)
                .When(x => !string.IsNullOrWhiteSpace(x.Sort))
                .WithMessage("Sort must be one of: title, title_desc, price, price_desc, newest, oldest.");
        }

        private static bool BeSupportedSort(string? sort)
        {
            return sort?.Trim().ToLowerInvariant() is
                "title" or
                "title_desc" or
                "price" or
                "price_desc" or
                "newest" or
                "oldest";
        }
    }
}
