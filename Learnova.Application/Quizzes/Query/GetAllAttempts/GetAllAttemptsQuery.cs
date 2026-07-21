using Learnova.Application.Common.Queries;
using Learnova.Application.Quizzes.DTO;
using MediatR;

namespace Learnova.Application.Quizzes.Query.GetAllAttempts
{
    public class GetAllAttemptsQuery(int quizId) : PagedSearchQuery, IRequest<IEnumerable<GetAllAttemptsDTO>>
    {
        public int QuizId { get; set; } = quizId;
    }
}
