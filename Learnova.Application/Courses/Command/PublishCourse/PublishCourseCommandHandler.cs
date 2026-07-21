using Learnova.Application.Caching;
using Learnova.Application.Common.BackgroundJobs;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Constant;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Command.PublishCourse
{
    public class PublishCourseCommandHandler(
        ILogger<PublishCourseCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ICacheInvalidationService cacheInvalidationService,
        IBackgroundJobScheduler backgroundJobScheduler) : IRequestHandler<PublishCourseCommand>
    {
        public async Task Handle(PublishCourseCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Publishing Course {CourseId}.", request.Id);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Course publish failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            if (!user.IsInRole(UserRole.Admin))
            {
                logger.LogWarning("User {UserId} is not allowed to publish courses.", user.Id);
                throw new ForbiddenAccessException("You are not allowed to publish this course.");
            }

            var course = await unitOfWork.course.GetById(request.Id);

            if (course is null)
            {
                logger.LogWarning("Course with Id: {CourseId} was not found.", request.Id);
                throw new NotFoundException("Course not found.");
            }

            if (course.Status != CourseStatus.PendingReview)
            {
                logger.LogWarning(
                    "Course {CourseId} cannot be published from status {Status}.",
                    course.Id,
                    course.Status);
                throw new BadRequestException("Only courses pending review can be published.");
            }

            await EnsureCourseHasPublishableContentAsync(course.Id);

            course.Status = CourseStatus.Published;
            course.PublishedAt = DateTime.UtcNow;

            await unitOfWork.CompleteAsync(cancellationToken);

            backgroundJobScheduler.Enqueue<ICourseIndexingJob>(
                job => job.IndexCourseAsync(course.Id));

            await cacheInvalidationService.EvictCoursesAsync(cancellationToken);
        }

        private async Task EnsureCourseHasPublishableContentAsync(int courseId)
        {
            var modules = (await unitOfWork.module.GetAllWithCondition(
                module => module.CourseId == courseId))
                .ToList();

            if (!modules.Any())
            {
                throw new BadRequestException("Course must have at least one module before publishing.");
            }

            var moduleIds = modules.Select(module => module.Id).ToList();
            var lessons = await unitOfWork.lesson.GetAllWithCondition(
                lesson => moduleIds.Contains(lesson.ModuleId));

            if (!lessons.Any())
            {
                throw new BadRequestException("Course must have at least one lesson before publishing.");
            }
        }
    }
}
