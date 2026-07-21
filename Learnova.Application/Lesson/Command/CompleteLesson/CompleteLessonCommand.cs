using MediatR;

namespace Learnova.Application.Lesson.Command.CompleteLesson
{
    public class CompleteLessonCommand : IRequest<bool>
    {
        public int LessonId { get; set; }
        public int CourseId { get; set; }
        public int ModuleId { get; set; }
    }
}
