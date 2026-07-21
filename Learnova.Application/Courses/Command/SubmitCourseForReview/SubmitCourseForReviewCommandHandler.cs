using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Command.SubmitCourseForReview
{
    public class SubmitCourseForReviewCommandHandler(
        ILogger<SubmitCourseForReviewCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ICourseAccessService courseAccessService) : IRequestHandler<SubmitCourseForReviewCommand>
    {
        public async Task Handle(SubmitCourseForReviewCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Submitting Course {CourseId} for review.", request.Id);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Course submit-for-review failed because current user was not found.");
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

            if (course.Status != CourseStatus.Draft)
            {
                logger.LogWarning(
                    "Course {CourseId} cannot be submitted for review from status {Status}.",
                    course.Id,
                    course.Status);
                throw new BadRequestException("Only draft courses can be submitted for review.");
            }

            await EnsureCourseHasPublishableContentAsync(course.Id);

            course.Status = CourseStatus.PendingReview;
            course.SubmittedAt = DateTime.UtcNow;

            await unitOfWork.CompleteAsync(cancellationToken);
        }

        private async Task EnsureCourseHasPublishableContentAsync(int courseId)
        {
            var modules = (await unitOfWork.module.GetAllWithCondition(
                module => module.CourseId == courseId))
                .ToList();

            if (!modules.Any())
            {
                throw new BadRequestException("Course must have at least one module before review.");
            }

            var moduleIds = modules.Select(module => module.Id).ToList();
            var lessons = await unitOfWork.lesson.GetAllWithCondition(
                lesson => moduleIds.Contains(lesson.ModuleId));

            if (!lessons.Any())
            {
                throw new BadRequestException("Course must have at least one lesson before review.");
            }
        }
    }
}
