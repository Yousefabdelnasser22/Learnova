using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Command.CreateQuiz
{
    public class CreateQuizCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateQuizCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService) : IRequestHandler<CreateQuizCommand>
    {
        public async Task Handle(CreateQuizCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting to handle CreateQuizCommand for CourseId: {CourseId}", request.CourseId);

            var user = userContext.GetCurrentUser();

            if (user == null)
            {
                logger.LogWarning("No user found in context.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            await courseAccessService.EnsureInstructorOwnsCourseAsync(
                request.CourseId,
                user.Id,
                cancellationToken);

            var course = await unitOfWork.course.GetById(request.CourseId);
            if (course is null)
            {
                throw new NotFoundException("Course not found.");
            }

            var quiz = new Quiz()
            {
                Title = request.Title,
                CourseId = request.CourseId,
                Questions = request.Questions.Select(q => new QuizQuestion
                {
                    Question = q.Question,
                    Options = q.Options,
                    CorrectAnswerIndex = q.CorrectAnswerIndex
                }).ToList()
            };

            logger.LogInformation("Creating quiz with title '{QuizTitle}' for CourseId: {CourseId}", quiz.Title, quiz.CourseId);
            await unitOfWork.quiz.Add(quiz);

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);

            logger.LogInformation("Quiz '{QuizTitle}' successfully created for CourseId: {CourseId}", quiz.Title, quiz.CourseId);
        }
    }
}

