using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Quizzes.Services;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Command.UpdateQuiz
{
    public class UpdateQuizCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateQuizCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService,
        IQuizAttemptInvalidationService quizAttemptInvalidationService) : IRequestHandler<UpdateQuizCommand>
    {
        public async Task Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting quiz update. QuizId: {QuizId}, Title: {Title}", request.Id, request.Title);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Quiz update failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var quiz = await unitOfWork.quiz.GetById(request.Id, q => q.Questions);

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

            quiz.Title = request.Title;
            await quizAttemptInvalidationService.InvalidateAttemptsForQuizAsync(
                quiz.Id,
                cancellationToken);

            unitOfWork.quizQuestion.DeleteRange(quiz.Questions);
            quiz.Questions = request.Questions.Select(q => new QuizQuestion
            {
                Question = q.Question,
                Options = q.Options,
                CorrectAnswerIndex = q.CorrectAnswerIndex
            }).ToList();

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);

            logger.LogInformation("Quiz updated successfully. QuizId: {QuizId}", request.Id);
        }
    }
}

