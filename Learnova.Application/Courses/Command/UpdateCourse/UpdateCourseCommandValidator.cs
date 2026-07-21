using FluentValidation;

namespace Learnova.Application.Courses.Command.UpdateCourse
{
    public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            RuleFor(x => x.Title)
              .NotEmpty().WithMessage("Title is required")
              .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.PreviewVideoUrl)
                .Must(BeAValidUrl)
                .When(x => !string.IsNullOrWhiteSpace(x.PreviewVideoUrl))
                .WithMessage("PreviewVideoUrl must be a valid absolute URL.");

            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Level must be a valid course level.");

            RuleFor(x => x.Language)
                .NotEmpty().WithMessage("Language is required.")
                .Must(BeSupportedLanguage).WithMessage("Language must be Arabic or English.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

            RuleFor(x => x.DurationInHours)
                .GreaterThan(0).WithMessage("Duration must be greater than 0");

            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0).WithMessage("SubCategoryId must be more than 0");
        }

        private static bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        private static bool BeSupportedLanguage(string? language)
        {
            return string.Equals(language, "Arabic", StringComparison.OrdinalIgnoreCase)
                || string.Equals(language, "English", StringComparison.OrdinalIgnoreCase);
        }
    }
}
