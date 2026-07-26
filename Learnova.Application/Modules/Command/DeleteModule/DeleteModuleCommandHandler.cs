using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Modules.Command.DeleteModule
{
    public class DeleteModuleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteModuleCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService) : IRequestHandler<DeleteModuleCommand>
    {
        public async Task Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Module delete failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var module = await unitOfWork.module.GetById(request.Id);
            if (module is null)
            {
                logger.LogWarning("Module with Id: {ModuleId} was not found.", request.Id);
                throw new NotFoundException("Module not found.");
            }

            if (module.CourseId != request.CourseId)
            {
                logger.LogWarning(
                    "Module {ModuleId} does not belong to Course {CourseId}.",
                    module.Id,
                    request.CourseId);
                throw new NotFoundException("Module not found.");
            }

            await courseAccessService.EnsureInstructorOwnsCourseAsync(
                module.CourseId,
                user.Id,
                cancellationToken);

            var course = await unitOfWork.course.GetById(module.CourseId);
            if (course is null)
            {
                throw new NotFoundException("Course not found.");
            }

            var lessons = (await unitOfWork.lesson.GetAllWithCondition(
                lesson => lesson.ModuleId == module.Id))
                .ToList();

            var lessonIds = lessons.Select(lesson => lesson.Id).ToList();

            foreach (var lesson in lessons)
            {
                lesson.IsDeleted = true;
            }

            var lessonProgress = lessonIds.Any()
                ? await unitOfWork.Repository<LessonProgress>()
                    .GetAllWithCondition(progress => lessonIds.Contains(progress.LessonId))
                : Enumerable.Empty<LessonProgress>();

            foreach (var progress in lessonProgress)
            {
                progress.IsDeleted = true;
            }

            var moduleProgress = await unitOfWork.Repository<ModuleProgress>()
                .GetAllWithCondition(progress => progress.ModuleId == module.Id);

            foreach (var progress in moduleProgress)
            {
                progress.IsDeleted = true;
            }

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            var followingModules = await unitOfWork.module.GetAllWithCondition(
                item => item.CourseId == module.CourseId && item.Position > module.Position);

            foreach (var followingModule in followingModules)
            {
                followingModule.Position--;
            }

            await unitOfWork.module.Delete(module.Id);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);
        }
    }
}

