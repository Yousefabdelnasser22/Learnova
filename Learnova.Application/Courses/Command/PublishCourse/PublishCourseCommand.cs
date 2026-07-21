using MediatR;

namespace Learnova.Application.Courses.Command.PublishCourse
{
    public class PublishCourseCommand : IRequest
    {
        public int Id { get; set; }
    }
}
