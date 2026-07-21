using Learnova.Application.Caching;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Command.UpdateCourse
{
    public class UpdateCourseCommandHandler(
        ILogger<UpdateCourseCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICacheInvalidationService cacheInvalidationService,
        ICourseSearchService courseSearchService) : IRequestHandler<UpdateCourseCommand>
    {
        public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Starting course Updating. Title: {Title}, Price: {Price}, DurationInHours: {DurationInHours}",
                request.Title,
                request.Price,
                request.DurationInHours);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Course creation failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var subCategory = await unitOfWork.subCategory.GetById(request.SubCategoryId);

            if (subCategory is null)
            {
                logger.LogWarning("SubCategory not found. SubCategoryId: {SubCategoryId}", request.SubCategoryId);
                throw new NotFoundException("SubCategory not found.");
            }

            var course = await unitOfWork.course.GetById(request.Id);

            if (course is null)
            {
                throw new NotFoundException("Course not found.");
            }

            await courseAccessService.EnsureInstructorOwnsCourseAsync(
                request.Id,
                user.Id,
                cancellationToken);

            if (course.Status == CourseStatus.Archived)
            {
                throw new BadRequestException("Archived courses cannot be updated.");
            }

            var wasPublished = course.Status == CourseStatus.Published;

            course.Title = request.Title;
            course.Description = request.Description;
            course.Price = request.Price;
            course.Thumbnail = request.Thumbnail;
            course.Level = request.Level;
            course.Language = string.IsNullOrWhiteSpace(request.Language) ? "Arabic" : request.Language;
            course.PreviewVideoUrl = request.PreviewVideoUrl;
            course.DurationInHours = request.DurationInHours;
            course.SubCategoryId = request.SubCategoryId;

            if (wasPublished)
            {
                course.Status = CourseStatus.PendingReview;
                course.SubmittedAt = DateTime.UtcNow;
            }

            await unitOfWork.CompleteAsync(cancellationToken);

            if (wasPublished)
            {
                await courseSearchService.DeleteCourseAsync(course.Id, cancellationToken);
                await cacheInvalidationService.EvictCoursesAsync(cancellationToken);
            }
        }
    }
}

