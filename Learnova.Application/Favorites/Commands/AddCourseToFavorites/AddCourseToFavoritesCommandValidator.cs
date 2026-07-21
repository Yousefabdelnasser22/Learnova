using FluentValidation;

namespace Learnova.Application.Favorites.Commands.AddCourseToFavorites
{
    public sealed class AddCourseToFavoritesCommandValidator : AbstractValidator<AddCourseToFavoritesCommand>
    {
        public AddCourseToFavoritesCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than 0");
        }
    }
}
