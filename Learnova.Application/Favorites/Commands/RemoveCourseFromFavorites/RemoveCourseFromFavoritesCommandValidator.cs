using FluentValidation;

namespace Learnova.Application.Favorites.Commands.RemoveCourseFromFavorites
{
    public sealed class RemoveCourseFromFavoritesCommandValidator : AbstractValidator<RemoveCourseFromFavoritesCommand>
    {
        public RemoveCourseFromFavoritesCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than 0");
        }
    }
}
