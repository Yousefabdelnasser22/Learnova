using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Lesson.Command.CreateLesson
{
    public class CreateLessonCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateLessonCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService)
        : IRequestHandler<CreateLessonCommand>
    {
        public async Task Handle(CreateLessonCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Creating lesson in ModuleId: {ModuleId}, CourseId: {CourseId}",
                request.ModuleId,
                request.CourseId);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Lesson creation failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var module = await unitOfWork.module.GetById(request.ModuleId, m => m.Course);

            if (module is null || module.IsDeleted)
            {
                logger.LogWarning(
                    "Module not found while creating lesson. ModuleId: {ModuleId}, CourseId: {CourseId}",
                    request.ModuleId,
                    request.CourseId);

                throw new NotFoundException("Module not found.");
            }

            if (module.CourseId != request.CourseId)
            {
                logger.LogWarning(
                    "Module does not belong to requested course while creating lesson. ModuleId: {ModuleId}, ActualCourseId: {ActualCourseId}, RequestedCourseId: {RequestedCourseId}",
                    request.ModuleId,
                    module.CourseId,
                    request.CourseId);

                throw new NotFoundException("Module not found in this course.");
            }

            await courseAccessService.EnsureInstructorOwnsCourseAsync(
                module.CourseId,
                user.Id,
                cancellationToken);

            var lessons = (await unitOfWork.lesson.GetAllWithCondition(
                lesson => lesson.ModuleId == request.ModuleId))
                .OrderBy(lesson => lesson.Position)
                .ToList();

            var maxPosition = lessons.Count + 1;
            if (request.Position > maxPosition)
            {
                throw new BadRequestException($"Position must be between 1 and {maxPosition}.");
            }

            foreach (var existingLesson in lessons.Where(lesson => lesson.Position >= request.Position))
            {
                existingLesson.Position++;
            }

            var lesson = mapper.Map<Learnova.Domain.Entities.Lesson>(request);
            lesson.ModuleId = request.ModuleId;

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(module.Course);

            await unitOfWork.lesson.Add(lesson);
            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                module.CourseId,
                wasPublished,
                cancellationToken);

            logger.LogInformation(
                "Lesson created successfully. LessonId: {LessonId}, ModuleId: {ModuleId}, CourseId: {CourseId}, Position: {Position}",
                lesson.Id,
                request.ModuleId,
                request.CourseId,
                lesson.Position);
        }
    }
}

