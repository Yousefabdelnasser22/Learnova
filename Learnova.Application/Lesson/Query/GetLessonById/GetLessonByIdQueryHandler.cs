using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Lesson.DTO;
using Learnova.Application.Lesson.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Lesson.Query.GetLessonById
{
    public class GetLessonByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetLessonByIdQuery> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService) : IRequestHandler<GetLessonByIdQuery, LessonDTO>
    {
        public async Task<LessonDTO> Handle(GetLessonByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Getting lesson by id. CourseId: {CourseId}, ModuleId: {ModuleId}, LessonId: {LessonId}",
                request.CourseId,
                request.ModuleId,
                request.Id);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Lesson details request failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var lessonSpec = new LessonByIdWithModuleSpecification(request.Id);
            var lesson = await unitOfWork.Repository<Learnova.Domain.Entities.Lesson>()
                .GetEntityWithSpecAsync(lessonSpec);

            if (lesson is null)
            {
                logger.LogWarning("Lesson not found. LessonId: {LessonId}", request.Id);
                throw new NotFoundException("Lesson not found.");
            }

            if (lesson.ModuleId != request.ModuleId || lesson.Module.CourseId != request.CourseId)
            {
                logger.LogWarning(
                    "Lesson {LessonId} does not belong to requested Module {ModuleId} / Course {CourseId}",
                    request.Id,
                    request.ModuleId,
                    request.CourseId);

                throw new NotFoundException("Lesson not found.");
            }

            await courseAccessService.EnsureCanViewCourseContentAsync(
                lesson.Module.CourseId,
                user,
                cancellationToken);

            logger.LogInformation(
                "Lesson retrieved successfully. LessonId: {LessonId}",
                request.Id);

            return mapper.Map<LessonDTO>(lesson);
        }
    }
}

