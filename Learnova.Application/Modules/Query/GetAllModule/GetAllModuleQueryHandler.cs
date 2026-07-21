using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Modules.DTO;
using Learnova.Application.Modules.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Modules.Query.GetAllModule
{
    public class GetAllModuleQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllModuleQueryHandler> logger,
        ICourseAccessService courseAccessService,
        IUserContext userContext)
        : IRequestHandler<GetAllModuleQuery, List<ModuleDTO>>
    {
        public async Task<List<ModuleDTO>> Handle(GetAllModuleQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Getting modules for CourseId: {CourseId}",
                request.CourseId);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Module listing failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            await courseAccessService.EnsureCanViewCourseContentAsync(
                request.CourseId,
                user,
                cancellationToken);

            var spec = new ModulesByCourseSpecification(
                request.CourseId,
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim());

            var modules = await unitOfWork.module.GetAllWithSpecAsync(spec);

            if (!modules.Any())
            {
                logger.LogWarning(
                    "No modules found for CourseId: {CourseId}",
                    request.CourseId);

                return new List<ModuleDTO>();
            }

            var result = mapper.Map<List<ModuleDTO>>(modules);

            logger.LogInformation(
                "Retrieved {Count} modules for CourseId: {CourseId}",
                result.Count,
                request.CourseId);

            return result;
        }
    }
}
