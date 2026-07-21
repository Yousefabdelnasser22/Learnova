using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Quizzes.DTO;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Query.GetQuizById
{
    public class GetQuizByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetQuizByIdQueryHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService) : IRequestHandler<GetQuizByIdQuery, GetQuizByIdDTO>
    {
        public async Task<GetQuizByIdDTO> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting quiz details for QuizId: {QuizId}", request.Id);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Quiz details request failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var quizzes = await unitOfWork.quiz.GetAllWithCondition(q => q.Id == request.Id, q => q.Course, q => q.Questions);
            var quiz = quizzes.FirstOrDefault();

            if (quiz is null)
            {
                logger.LogWarning("Quiz not found. QuizId: {QuizId}", request.Id);
                throw new NotFoundException("Quiz not found.");
            }

            await courseAccessService.EnsureCanViewCourseContentAsync(
                quiz.CourseId,
                user,
                cancellationToken);

            var quizDto = mapper.Map<GetQuizByIdDTO>(quiz);

            logger.LogInformation("Quiz retrieved successfully. QuizId: {QuizId}", request.Id);

            return quizDto;
        }
    }
}
