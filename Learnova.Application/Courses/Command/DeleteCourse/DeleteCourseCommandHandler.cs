using Learnova.Application.Caching;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Command.DeleteCourse
{
    using EnrollmentEntity = Learnova.Domain.Entites.Enrollment;

    public class DeleteCourseCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteCourseCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        ICacheInvalidationService cacheInvalidationService,
        ICourseSearchService courseSearchService) : IRequestHandler<DeleteCourseCommand>
    {
        public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting Course with id :{Id}", request.Id);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Course delete failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var course = await unitOfWork.course.GetById(request.Id);
            if (course is null)
            {
                logger.LogWarning("Course with Id: {CourseId} was not found.", request.Id);
                throw new NotFoundException("Course not found.");
            }

            await courseAccessService.EnsureInstructorOwnsCourseAsync(
                request.Id,
                user.Id,
                cancellationToken);

            var wasPublished = course.Status == CourseStatus.Published;

            var enrollmentCount = await unitOfWork.Repository<EnrollmentEntity>()
                .CountAsync(enrollment => enrollment.CourseId == request.Id);

            if (enrollmentCount > 0)
            {
                throw new BadRequestException("Courses with enrollments cannot be deleted. Archive the course instead.");
            }

            var orderItemCount = await unitOfWork.Repository<OrderItem>()
                .CountAsync(orderItem => orderItem.CourseId == request.Id);

            if (orderItemCount > 0)
            {
                throw new BadRequestException("Courses with orders cannot be deleted. Archive the course instead.");
            }

            await unitOfWork.course.Delete(request.Id);
            await unitOfWork.CompleteAsync(cancellationToken);

            if (wasPublished)
            {
                await courseSearchService.DeleteCourseAsync(request.Id, cancellationToken);
                await cacheInvalidationService.EvictCoursesAsync(cancellationToken);
            }
        }
    }
}

