using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Learnova.Application.Modules.Command.ReorderModule
{
    public class ReorderModuleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ReorderModuleCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService)
        : IRequestHandler<ReorderModuleCommand>
    {
        public async Task Handle(ReorderModuleCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Reordering module. ModuleId: {ModuleId}, CourseId: {CourseId}, NewPosition: {NewPosition}",
                request.ModuleId,
                request.CourseId,
                request.NewPosition);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Module reorder failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var module = await unitOfWork.module.GetById(request.ModuleId);

            if (module is null || module.IsDeleted)
            {
                logger.LogWarning(
                    "Module not found while reordering. ModuleId: {ModuleId}, CourseId: {CourseId}",
                    request.ModuleId,
                    request.CourseId);

                throw new NotFoundException("Module not found.");
            }

            if (module.CourseId != request.CourseId)
            {
                logger.LogWarning(
                    "Module does not belong to requested course. ModuleId: {ModuleId}, ActualCourseId: {ActualCourseId}, RequestedCourseId: {RequestedCourseId}",
                    request.ModuleId,
                    module.CourseId,
                    request.CourseId);

                throw new NotFoundException("Module not found in this course.");
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

            var modules = (await unitOfWork.module.GetAllWithCondition(
                x => x.CourseId == request.CourseId))
                .OrderBy(x => x.Position)
                .ToList();

            if (!modules.Any())
            {
                throw new NotFoundException("Modules not found for this course.");
            }

            var maxPosition = modules.Count;

            if (request.NewPosition > maxPosition)
            {
                throw new BadRequestException(
                    $"NewPosition must be between 1 and {maxPosition}.");
            }

            var oldPosition = module.Position;
            var newPosition = request.NewPosition;

            if (oldPosition == newPosition)
            {
                logger.LogInformation(
                    "Module reorder skipped because position is unchanged. ModuleId: {ModuleId}, Position: {Position}",
                    request.ModuleId,
                    oldPosition);
                return;
            }

            if (newPosition < oldPosition)
            {
                foreach (var item in modules.Where(x => x.Position >= newPosition && x.Position < oldPosition))
                {
                    item.Position++;
                }
            }
            else
            {
                foreach (var item in modules.Where(x => x.Position <= newPosition && x.Position > oldPosition))
                {
                    item.Position--;
                }
            }

            module.Position = newPosition;

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            unitOfWork.module.Update(module);
            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);

            logger.LogInformation(
                "Module reordered successfully. ModuleId: {ModuleId}, OldPosition: {OldPosition}, NewPosition: {NewPosition}, CourseId: {CourseId}",
                request.ModuleId,
                oldPosition,
                newPosition,
                request.CourseId);
        }
    }
}

