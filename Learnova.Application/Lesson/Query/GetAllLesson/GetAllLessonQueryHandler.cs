using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Lesson.DTO;
using Learnova.Application.Lesson.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Lesson.Query.GetAllLesson
{
    public class GetAllLessonQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllLessonQueryHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService) : IRequestHandler<GetAllLessonQuery, List<LessonDTO>>
    {
        public async Task<List<LessonDTO>> Handle(GetAllLessonQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Getting all lessons for CourseId: {CourseId}, ModuleId: {ModuleId}",
                request.CourseId,
                request.ModuleId);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Lesson listing failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var module = await unitOfWork.module.GetById(request.ModuleId);

            if (module is null)
            {
                logger.LogWarning(
                    "Module not found. ModuleId: {ModuleId}",
                    request.ModuleId);

                throw new NotFoundException("Module not found.");
            }

            if (module.CourseId != request.CourseId)
            {
                logger.LogWarning(
                    "Module {ModuleId} does not belong to Course {CourseId}",
                    request.ModuleId,
                    request.CourseId);

                throw new NotFoundException("Module not found in this course.");
            }

            await courseAccessService.EnsureCanViewCourseContentAsync(
                request.CourseId,
                user,
                cancellationToken);

            var spec = new LessonsByModuleSpecification(
                request.ModuleId,
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim());

            var lessons = await unitOfWork.lesson.GetAllWithSpecAsync(spec);

            logger.LogInformation(
                "Retrieved {LessonCount} lessons for ModuleId: {ModuleId}",
                lessons.Count,
                request.ModuleId);

            return mapper.Map<List<LessonDTO>>(lessons);
        }
    }
}

