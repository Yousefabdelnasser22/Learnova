using Learnova.Application.Caching;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Constant;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Command.ArchiveCourse
{
    public class ArchiveCourseCommandHandler(
        ILogger<ArchiveCourseCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ICacheInvalidationService cacheInvalidationService,
        ICourseSearchService courseSearchService) : IRequestHandler<ArchiveCourseCommand>
    {
        public async Task Handle(ArchiveCourseCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Archiving Course {CourseId}.", request.Id);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Course archive failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            if (!user.IsInRole(UserRole.Admin))
            {
                logger.LogWarning("User {UserId} is not allowed to archive courses.", user.Id);
                throw new ForbiddenAccessException("You are not allowed to archive this course.");
            }

            var course = await unitOfWork.course.GetById(request.Id);

            if (course is null)
            {
                logger.LogWarning("Course with Id: {CourseId} was not found.", request.Id);
                throw new NotFoundException("Course not found.");
            }

            if (course.Status == CourseStatus.Archived)
            {
                logger.LogWarning("Course {CourseId} is already archived.", course.Id);
                throw new BadRequestException("Course is already archived.");
            }

            if (course.Status != CourseStatus.PendingReview && course.Status != CourseStatus.Published)
            {
                logger.LogWarning(
                    "Course {CourseId} cannot be archived from status {Status}.",
                    course.Id,
                    course.Status);
                throw new BadRequestException("Only pending review or published courses can be archived.");
            }

            var wasPublished = course.Status == CourseStatus.Published;

            course.Status = CourseStatus.Archived;
            course.ArchivedAt = DateTime.UtcNow;

            await unitOfWork.CompleteAsync(cancellationToken);

            if (wasPublished)
            {
                await courseSearchService.DeleteCourseAsync(course.Id, cancellationToken);
                await cacheInvalidationService.EvictCoursesAsync(cancellationToken);
            }
        }
    }
}

