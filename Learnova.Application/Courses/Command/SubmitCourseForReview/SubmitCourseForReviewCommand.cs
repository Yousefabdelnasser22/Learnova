using MediatR;

namespace Learnova.Application.Courses.Command.SubmitCourseForReview
{
    public class SubmitCourseForReviewCommand : IRequest
    {
        public int Id { get; set; }
    }
}
