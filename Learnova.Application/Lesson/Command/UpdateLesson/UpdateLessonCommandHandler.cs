using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Lesson.Command.UpdateLesson
{
    public class UpdateLessonCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateLessonCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICourseContentChangeService courseContentChangeService)
        : IRequestHandler<UpdateLessonCommand>
    {
        public async Task Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Updating lesson in ModuleId: {ModuleId}, CourseId: {CourseId}",
                request.ModuleId,
                request.CourseId);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Lesson update failed because current user was not found.");
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
                    "Module not found while updating lesson. ModuleId: {ModuleId}, LessonId: {LessonId}",
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

            var lessons = (await unitOfWork.lesson.GetAllWithCondition(
                item => item.ModuleId == request.ModuleId))
                .OrderBy(item => item.Position)
                .ToList();

            var maxPosition = lessons.Count;
            if (request.Position > maxPosition)
            {
                throw new BadRequestException($"Position must be between 1 and {maxPosition}.");
            }

            if (request.Position != lesson.Position)
            {
                if (request.Position < lesson.Position)
                {
                    foreach (var item in lessons.Where(item =>
                        item.Id != lesson.Id &&
                        item.Position >= request.Position &&
                        item.Position < lesson.Position))
                    {
                        item.Position++;
                    }
                }
                else
                {
                    foreach (var item in lessons.Where(item =>
                        item.Id != lesson.Id &&
                        item.Position <= request.Position &&
                        item.Position > lesson.Position))
                    {
                        item.Position--;
                    }
                }
            }

            lesson.TextContent = request.TextContent;
            lesson.Description = request.Description;
            lesson.Position = request.Position;
            lesson.ModuleId = request.ModuleId;
            lesson.PdfUrl = request.PdfUrl;
            lesson.VideoUrl = request.VideoUrl;
            lesson.Id = request.Id;
            lesson.Title = request.Title;

            var wasPublished = courseContentChangeService.MarkPendingReviewIfPublished(module.Course);

            await unitOfWork.CompleteAsync(cancellationToken);
            await courseContentChangeService.RemoveFromDiscoveryIfNeededAsync(
                module.CourseId,
                wasPublished,
                cancellationToken);

            logger.LogInformation(
                "Lesson Updated successfully. LessonId: {LessonId}, ModuleId: {ModuleId}, CourseId: {CourseId}, Position: {Position}",
                lesson.Id,
                request.ModuleId,
                request.CourseId,
                lesson.Position);
        }
    }
}

