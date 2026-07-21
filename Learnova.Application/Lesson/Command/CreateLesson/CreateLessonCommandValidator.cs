using FluentValidation;
using Learnova.Application.Lesson.Command.CreateLesson;

public class CreateLessonCommandValidator : AbstractValidator<CreateLessonCommand>
{
    public CreateLessonCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0);

        RuleFor(x => x.ModuleId)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Position)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.VideoUrl)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.VideoUrl))
            .WithMessage("VideoUrl must be a valid absolute URL.");

        RuleFor(x => x.PdfUrl)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.PdfUrl))
            .WithMessage("PdfUrl must be a valid absolute URL.");

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.VideoUrl) ||
                !string.IsNullOrWhiteSpace(x.TextContent) ||
                !string.IsNullOrWhiteSpace(x.PdfUrl))
            .WithMessage("At least one of VideoUrl, TextContent, or PdfUrl must be provided.");
    }

    private static bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
