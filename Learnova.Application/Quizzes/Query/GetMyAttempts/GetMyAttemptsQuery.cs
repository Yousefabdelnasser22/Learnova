using Learnova.Application.Common.Queries;
using Learnova.Application.Quizzes.DTO;
using MediatR;

namespace Learnova.Application.Quizzes.Query.GetMyAttempts
{
    public class GetMyAttemptsQuery(int quizId) : PagedSearchQuery, IRequest<IEnumerable<GetMyAttemptsDTO>>
    {
        public int QuizId { get; set; } = quizId;
    }
}
