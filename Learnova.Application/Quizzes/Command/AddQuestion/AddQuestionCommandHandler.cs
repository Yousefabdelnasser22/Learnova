using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Quizzes.Services;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Command.AddQuestion
{
    public class AddQuestionCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AddQuestionCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService,
        IQuizAttemptInvalidationService quizAttemptInvalidationService) : IRequestHandler<AddQuestionCommand>
    {
        public async Task Handle(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Adding question to quiz. QuizId: {QuizId}", request.QuizId);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Add question failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var quiz = await unitOfWork.quiz.GetById(request.QuizId, q => q.Questions);

            if (quiz is null)
            {
                logger.LogWarning("Quiz not found. QuizId: {QuizId}", request.QuizId);
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

            quiz.Questions.Add(new QuizQuestion
            {
                Question = request.Question,
                Options = request.Options,
                CorrectAnswerIndex = request.CorrectAnswerIndex
            });

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);

            logger.LogInformation("Question added successfully to QuizId: {QuizId}", request.QuizId);
        }
    }
}

