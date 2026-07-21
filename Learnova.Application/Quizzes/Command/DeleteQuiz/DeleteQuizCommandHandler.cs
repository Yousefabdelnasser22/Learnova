using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Quizzes.Services;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Command.DeleteQuiz
{
    public class DeleteQuizCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteQuizCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService,
        IQuizAttemptInvalidationService quizAttemptInvalidationService) : IRequestHandler<DeleteQuizCommand>
    {
        public async Task Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting Quiz with id :{Id}", request.Id);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Quiz delete failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var quiz = await unitOfWork.quiz.GetById(request.Id);

            if (quiz is null)
            {
                logger.LogWarning("Quiz not found. QuizId: {QuizId}", request.Id);
                throw new NotFoundException("Quiz not found.");
            }

            await courseAccessService.EnsureInstructorOwnsCourseAsync(
                quiz.CourseId,
                user.Id,
                cancellationToken);

            var course = await unitOfWork.course.GetById(quiz.CourseId);
            if (course is null)
            {
                throw new NotFoundException("Course not found.");
            }

            await quizAttemptInvalidationService.InvalidateAttemptsForQuizAsync(
                quiz.Id,
                cancellationToken);

            await unitOfWork.quiz.Delete(request.Id);
            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);

            logger.LogInformation("Quiz deleted successfully. QuizId: {QuizId}", request.Id);
        }
    }
}

