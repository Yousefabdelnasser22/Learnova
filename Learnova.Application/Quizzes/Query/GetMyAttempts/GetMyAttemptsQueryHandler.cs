using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Quizzes.DTO;
using Learnova.Application.Quizzes.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Query.GetMyAttempts
{
    public class GetMyAttemptsQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetMyAttemptsQueryHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        IMapper mapper) : IRequestHandler<GetMyAttemptsQuery, IEnumerable<GetMyAttemptsDTO>>
    {
        public async Task<IEnumerable<GetMyAttemptsDTO>> Handle(GetMyAttemptsQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();
            if (user == null)
            {
                logger.LogWarning("Unauthorized attempt to get current user quiz attempts for QuizId {QuizId}", request.QuizId);
                throw new UnauthorizedException("User is not authenticated.");
            }

            var quiz = await unitOfWork.quiz.GetById(request.QuizId);
            if (quiz == null)
            {
                logger.LogWarning("Quiz not found while getting current user attempts for QuizId {QuizId}", request.QuizId);
                throw new NotFoundException("Quiz not found.");
            }

            await courseAccessService.EnsureStudentEnrolledInCourseAsync(
                quiz.CourseId,
                user.Id,
                cancellationToken);

            var spec = new QuizAttemptsByStudentAndQuizSpecification(
                request.QuizId,
                user.Id,
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim());

            var attempts = await unitOfWork.quizAttempt.GetAllWithSpecAsync(spec);

            return mapper.Map<IEnumerable<GetMyAttemptsDTO>>(attempts);
        }
    }
}

