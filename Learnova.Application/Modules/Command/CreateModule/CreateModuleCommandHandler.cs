using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Modules.Command.CreateModule
{
    public class CreateModuleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateModuleCommandHandler> logger,
        IMapper mapper,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService) : IRequestHandler<CreateModuleCommand>
    {
        public async Task Handle(CreateModuleCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Module creation failed because current user was not found.");
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

            var modules = (await unitOfWork.module.GetAllWithCondition(
                module => module.CourseId == request.CourseId))
                .OrderBy(module => module.Position)
                .ToList();

            var maxPosition = modules.Count + 1;
            if (request.Position > maxPosition)
            {
                throw new BadRequestException($"Position must be between 1 and {maxPosition}.");
            }

            foreach (var existingModule in modules.Where(module => module.Position >= request.Position))
            {
                existingModule.Position++;
            }

            var module = mapper.Map<Learnova.Domain.Entities.Module>(request);
            await unitOfWork.module.Add(module);

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(course);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                course.Id,
                wasPublished,
                cancellationToken);
        }
    }
}

