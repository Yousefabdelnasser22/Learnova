using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Quizzes.DTO;
using Learnova.Application.Quizzes.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Query.GetAllAttempts
{
    public class GetAllAttemptsQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetAllAttemptsQueryHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        IMapper mapper) : IRequestHandler<GetAllAttemptsQuery, IEnumerable<GetAllAttemptsDTO>>
    {
        public async Task<IEnumerable<GetAllAttemptsDTO>> Handle(GetAllAttemptsQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();
            if (user == null)
            {
                logger.LogWarning("Unauthorized attempt to get all quiz attempts for QuizId {QuizId}", request.QuizId);
                throw new UnauthorizedException("User is not authenticated.");
            }

            var quiz = await unitOfWork.quiz.GetById(request.QuizId);
            if (quiz == null)
            {
                logger.LogWarning("Quiz not found while getting all attempts for QuizId {QuizId}", request.QuizId);
                throw new NotFoundException("Quiz not found.");
            }

            await courseAccessService.EnsureInstructorOwnsCourseAsync(
                quiz.CourseId,
                user.Id,
                cancellationToken);

            var spec = new QuizAttemptsByQuizSpecification(
                request.QuizId,
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim());

            var attempts = await unitOfWork.quizAttempt.GetAllWithSpecAsync(spec);

            return mapper.Map<IEnumerable<GetAllAttemptsDTO>>(attempts);
        }
    }
}

