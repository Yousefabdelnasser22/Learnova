using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Modules.Command.UpdateModule
{
    public class UpdateModuleCommandHandler(
      IUnitOfWork unitOfWork,
      ILogger<UpdateModuleCommandHandler> logger,
      IUserContext userContext,
      ICourseAccessService courseAccessService,
      ICourseContentChangeService courseContentChangeService)
      : IRequestHandler<UpdateModuleCommand>
    {
        public async Task Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Starting module update. Id: {ModuleId}, Title: {Title}, Description: {Description}, Position: {Position}, CourseId: {CourseId}",
                request.Id,
                request.Title,
                request.Description,
                request.Position,
                request.CourseId);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Module update failed because current user was not found.");
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
                request.CourseId,
                user.Id,
                cancellationToken);

            var course = await unitOfWork.course.GetById(module.CourseId);
            if (course is null)
            {
                throw new NotFoundException("Course not found.");
            }

            var modules = (await unitOfWork.module.GetAllWithCondition(
                item => item.CourseId == request.CourseId))
                .OrderBy(item => item.Position)
                .ToList();

            var maxPosition = modules.Count;
            if (request.Position > maxPosition)
            {
                throw new BadRequestException($"Position must be between 1 and {maxPosition}.");
            }

            if (request.Position != module.Position)
            {
                if (request.Position < module.Position)
                {
                    foreach (var item in modules.Where(item =>
                        item.Id != module.Id &&
                        item.Position >= request.Position &&
                        item.Position < module.Position))
                    {
                        item.Position++;
                    }
                }
                else
                {
                    foreach (var item in modules.Where(item =>
                        item.Id != module.Id &&
                        item.Position <= request.Position &&
                        item.Position > module.Position))
                    {
                        item.Position--;
                    }
                }
            }

            module.Title = request.Title;
            module.Description = request.Description;
            module.Position = request.Position;
            module.CourseId = request.CourseId;

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);

            logger.LogInformation("Module with Id: {ModuleId} updated successfully.", request.Id);
        }
    }
}

