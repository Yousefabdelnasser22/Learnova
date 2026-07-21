using MediatR;

namespace Learnova.Application.Courses.Command.ArchiveCourse
{
    public class ArchiveCourseCommand : IRequest
    {
        public int Id { get; set; }
    }
}
