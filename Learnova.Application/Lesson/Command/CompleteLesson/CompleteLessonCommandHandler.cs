using Learnova.Application.Courses.Services;
using Learnova.Application.Enrollment.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Lesson.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Lesson.Command.CompleteLesson
{
    public class CompleteLessonCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CompleteLessonCommand> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        IEnrollmentProgressService enrollmentProgressService
    ) : IRequestHandler<CompleteLessonCommand, bool>
    {
        public async Task<bool> Handle(CompleteLessonCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Start CompleteLesson for LessonId: {LessonId}", request.LessonId);

            var user = userContext.GetCurrentUser();
            if (user == null)
            {
                logger.LogWarning("CompleteLesson failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var lessonSpec = new LessonByIdWithModuleSpecification(request.LessonId);
            var lesson = await unitOfWork.Repository<Learnova.Domain.Entites.Lesson>()
                .GetEntityWithSpecAsync(lessonSpec);
            if (lesson == null)
            {
                logger.LogWarning("Lesson not found: {LessonId}", request.LessonId);
                throw new NotFoundException("Lesson not found.");
            }

            if (lesson.ModuleId != request.ModuleId || lesson.Module.CourseId != request.CourseId)
            {
                logger.LogWarning(
                    "Lesson {LessonId} does not belong to requested Module {ModuleId} / Course {CourseId}",
                    request.LessonId,
                    request.ModuleId,
                    request.CourseId);
                throw new NotFoundException("Lesson not found.");
            }

            int courseId = lesson.Module.CourseId;
            int moduleId = lesson.ModuleId;

            logger.LogInformation("Resolved CourseId {CourseId} and ModuleId {ModuleId} for LessonId {LessonId}", courseId, moduleId, request.LessonId);

            await courseAccessService.EnsureStudentEnrolledInCourseAsync(
                courseId,
                user.Id,
                cancellationToken);

            var enroll = await unitOfWork.enrollment.GetByStudentAndCourseAsync(user.Id, courseId);
            if (enroll == null)
            {
                logger.LogWarning(
                    "Enrollment record was not found after access verification for User {UserId} in Course {CourseId}",
                    user.Id,
                    courseId);
                throw new NotFoundException("Enrollment not found.");
            }

            await unitOfWork.ExecuteInTransactionAsync(async transactionCancellationToken =>
            {
                var progress = await unitOfWork.lessonProgress
                    .checkLessonProgress(user.Id, request.LessonId);

                if (progress == null)
                {
                    logger.LogInformation(
                        "Creating LessonProgress for User {UserId}, Lesson {LessonId}",
                        user.Id,
                        request.LessonId);

                    progress = new LessonProgress
                    {
                        StudentId = user.Id,
                        LessonId = request.LessonId
                    };

                    await unitOfWork.lessonProgress.Add(progress);
                }
                else
                {
                    logger.LogInformation(
                        "Updating LessonProgress for User {UserId}, Lesson {LessonId}",
                        user.Id,
                        request.LessonId);
                }

                progress.IsCompleted = true;
                progress.CompletedAt ??= DateTime.UtcNow;

                
                await unitOfWork.CompleteAsync(transactionCancellationToken);

                await enrollmentProgressService.RecalculateModuleProgressAsync(
                    user.Id,
                    moduleId,
                    transactionCancellationToken);

                
                await unitOfWork.CompleteAsync(transactionCancellationToken);

                await enrollmentProgressService.RecalculateCourseProgressAsync(
                    user.Id,
                    courseId,
                    transactionCancellationToken);

                await unitOfWork.CompleteAsync(transactionCancellationToken);
            }, cancellationToken);

            logger.LogInformation("CompleteLesson finished successfully for LessonId: {LessonId}", request.LessonId);

            return true;
        }
    }
}

