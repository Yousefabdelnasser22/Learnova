using FluentValidation;

namespace Learnova.Application.Favorites.Query.IsCourseFavorite
{
    public sealed class IsCourseFavoriteQueryValidator : AbstractValidator<IsCourseFavoriteQuery>
    {
        public IsCourseFavoriteQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than 0");
        }
    }
}
