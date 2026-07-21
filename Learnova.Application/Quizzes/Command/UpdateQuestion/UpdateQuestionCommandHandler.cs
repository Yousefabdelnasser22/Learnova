using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Quizzes.Services;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Command.UpdateQuestion
{
    public class UpdateQuestionCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateQuestionCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService,
        IQuizAttemptInvalidationService quizAttemptInvalidationService) : IRequestHandler<UpdateQuestionCommand>
    {
        public async Task Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting question update. QuestionId: {QuestionId}", request.Id);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Question update failed because current user was not found.");
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

            question.Question = request.Question;
            question.Options = request.Options;
            question.CorrectAnswerIndex = request.CorrectAnswerIndex;

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);

            logger.LogInformation("Question updated successfully. QuestionId: {QuestionId}", request.Id);
        }
    }
}

