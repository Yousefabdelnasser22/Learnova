using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Modules.DTO;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Modules.Query.GetModuleById
{
    public class GetModuleByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetModuleByIdQueryHandler> logger,
        ICourseAccessService courseAccessService,
        IUserContext userContext)
        : IRequestHandler<GetModuleByIdQuery, ModuleDTO>
    {
        public async Task<ModuleDTO> Handle(GetModuleByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Getting module details for ModuleId: {ModuleId} in CourseId: {CourseId}",
                request.Id,
                request.CourseId);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Module details request failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var module = await unitOfWork.module.GetById(request.Id, m => m.Course);

            if (module is null)
            {
                logger.LogWarning(
                    "Module not found. ModuleId: {ModuleId}, CourseId: {CourseId}",
                    request.Id,
                    request.CourseId);

                throw new NotFoundException("Module not found.");
            }

            if (module.CourseId != request.CourseId)
            {
                logger.LogWarning(
                    "Module does not belong to the requested course. ModuleId: {ModuleId}, ActualCourseId: {ActualCourseId}, RequestedCourseId: {RequestedCourseId}",
                    request.Id,
                    module.CourseId,
                    request.CourseId);

                throw new NotFoundException("Module not found in this course.");
            }

            await courseAccessService.EnsureCanViewCourseContentAsync(
                module.CourseId,
                user,
                cancellationToken);

            var moduleDto = mapper.Map<ModuleDTO>(module);

            logger.LogInformation(
                "Module retrieved successfully. ModuleId: {ModuleId}, CourseId: {CourseId}",
                request.Id,
                request.CourseId);

            return moduleDto;
        }
    }
}

