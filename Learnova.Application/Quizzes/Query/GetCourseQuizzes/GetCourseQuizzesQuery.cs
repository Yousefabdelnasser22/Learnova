using Learnova.Application.Common.Queries;
using Learnova.Application.Quizzes.DTO;
using MediatR;

namespace Learnova.Application.Quizzes.Query.GetCourseQuizzes
{
    public class GetCourseQuizzesQuery : PagedSearchQuery, IRequest<IEnumerable<GetAllQuizzesDTO>>
    {
        public int CourseId { get; set; }
    }
}
