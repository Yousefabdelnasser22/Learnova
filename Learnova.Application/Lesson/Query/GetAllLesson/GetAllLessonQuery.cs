using Learnova.Application.Common.Queries;
using Learnova.Application.Lesson.DTO;
using MediatR;

namespace Learnova.Application.Lesson.Query.GetAllLesson
{
    public class GetAllLessonQuery : PagedSearchQuery, IRequest<List<LessonDTO>>
    {
        public int CourseId { get; set; }

        public int ModuleId { get; set; }
    }
}
