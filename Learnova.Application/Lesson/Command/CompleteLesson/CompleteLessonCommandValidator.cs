using FluentValidation;

namespace Learnova.Application.Lesson.Command.CompleteLesson
{
    public class CompleteLessonCommandValidator : AbstractValidator<CompleteLessonCommand>
    {
        public CompleteLessonCommandValidator()
        {
            RuleFor(x => x.LessonId)
                .GreaterThan(0);

            RuleFor(x => x.CourseId)
                .GreaterThan(0);

            RuleFor(x => x.ModuleId)
                .GreaterThan(0);
        }
    }
}
