using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Lesson.Command.DeleteLesson
{
    public class DeleteLessonCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteLessonCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService) : IRequestHandler<DeleteLessonCommand>
    {
        public async Task Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
              "Deleting lesson in ModuleId: {ModuleId}, CourseId: {CourseId}",
              request.ModuleId,
              request.CourseId);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Lesson delete failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var lesson = await unitOfWork.lesson.GetById(request.Id);
            if (lesson is null)
            {
                throw new NotFoundException("Lesson not found.");
            }

            var module = await unitOfWork.module.GetById(lesson.ModuleId, m => m.Course);

            if (module is null || module.IsDeleted)
            {
                logger.LogWarning(
                    "Module not found while deleting lesson. ModuleId: {ModuleId}, LessonId: {LessonId}",
                    lesson.ModuleId,
                    lesson.Id);

                throw new NotFoundException("Module not found.");
            }

            if (lesson.ModuleId != request.ModuleId || module.CourseId != request.CourseId)
            {
                logger.LogWarning(
                    "Lesson {LessonId} does not belong to requested Module {ModuleId} / Course {CourseId}.",
                    lesson.Id,
                    request.ModuleId,
                    request.CourseId);

                throw new NotFoundException("Lesson not found.");
            }

            await courseAccessService.EnsureInstructorOwnsCourseAsync(
                module.CourseId,
                user.Id,
                cancellationToken);

            var lessonProgress = await unitOfWork.Repository<LessonProgress>()
                .GetAllWithCondition(progress => progress.LessonId == lesson.Id);

            foreach (var progress in lessonProgress)
            {
                progress.IsDeleted = true;
            }

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(module.Course);

            var followingLessons = await unitOfWork.lesson.GetAllWithCondition(
                item => item.ModuleId == lesson.ModuleId && item.Position > lesson.Position);

            foreach (var followingLesson in followingLessons)
            {
                followingLesson.Position--;
            }

            await unitOfWork.lesson.Delete(lesson.Id);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                module.CourseId,
                wasPublished,
                cancellationToken);

            logger.LogInformation(
                "Lesson Deleting successfully. LessonId: {LessonId}, ModuleId: {ModuleId}, CourseId: {CourseId}",
                request.Id,
                request.ModuleId,
                request.CourseId);
        }
    }
}

