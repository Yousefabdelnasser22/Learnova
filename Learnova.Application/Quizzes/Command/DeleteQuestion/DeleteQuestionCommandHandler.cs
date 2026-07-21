using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Quizzes.Services;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Command.DeleteQuestion
{
    public class DeleteQuestionCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteQuestionCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService,
        IQuizAttemptInvalidationService quizAttemptInvalidationService) : IRequestHandler<DeleteQuestionCommand>
    {
        public async Task Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting question. QuestionId: {QuestionId}", request.Id);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Question delete failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var question = await unitOfWork.quizQuestion.GetById(request.Id);
            if (question is null)
            {
                logger.LogWarning("Question not found. QuestionId: {QuestionId}", request.Id);
                throw new NotFoundException("Question not found.");
            }

            var quizzes = await unitOfWork.quiz.GetAllWithCondition(q => q.Questions.Any(question => question.Id == request.Id));
            var quiz = quizzes.FirstOrDefault();

            if (quiz is null)
            {
                logger.LogWarning("Quiz not found for QuestionId: {QuestionId}", request.Id);
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

            await unitOfWork.quizQuestion.Delete(request.Id);
            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);

            logger.LogInformation("Question deleted successfully. QuestionId: {QuestionId}", request.Id);
        }
    }
}

