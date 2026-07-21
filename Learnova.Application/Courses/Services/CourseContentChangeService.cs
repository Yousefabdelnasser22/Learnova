using Learnova.Application.Caching;
using Learnova.Application.Exceptions;
using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;

namespace Learnova.Application.Courses.Services
{
    using EnrollmentEntity = Learnova.Domain.Entites.Enrollment;

    public sealed class CourseContentChangeService(
        ICourseSearchService courseSearchService,
        ICacheInvalidationService cacheInvalidationService,
        IUnitOfWork unitOfWork) : ICourseContentChangeService
    {
        public bool MarkPendingReviewIfPublished(Course course)
        {
            if (course.Status == CourseStatus.Archived)
            {
                throw new BadRequestException("Archived courses cannot be modified.");
            }

            if (course.Status != CourseStatus.Published)
            {
                return false;
            }

            course.Status = CourseStatus.PendingReview;
            course.SubmittedAt = DateTime.UtcNow;

            return true;
        }

        public async Task RemoveFromDiscoveryIfNeededAsync(
            int courseId,
            bool wasPublished,
            CancellationToken cancellationToken = default)
        {
            if (!wasPublished)
            {
                return;
            }

            await InvalidateCompletedCourseWorkAsync(courseId, cancellationToken);
            await courseSearchService.DeleteCourseAsync(courseId, cancellationToken);
            await cacheInvalidationService.EvictCoursesAsync(cancellationToken);
        }

        private async Task InvalidateCompletedCourseWorkAsync(
            int courseId,
            CancellationToken cancellationToken)
        {
            var completedEnrollments = await unitOfWork
                .Repository<EnrollmentEntity>()
                .GetAllWithCondition(e =>
                    e.CourseId == courseId &&
                    (e.IsCompleted || e.Status == EnrollmentStatus.Completed));

            foreach (var enrollment in completedEnrollments)
            {
                enrollment.ProgressPercentage = 0;
                enrollment.IsCompleted = false;
                enrollment.CompletedAt = null;

                if (enrollment.Status == EnrollmentStatus.Completed)
                {
                    enrollment.Status = EnrollmentStatus.Active;
                }
            }

            var certificates = await unitOfWork
                .Repository<Certificate>()
                .GetAllWithCondition(c => c.CourseId == courseId);

            foreach (var certificate in certificates)
            {
                await unitOfWork.Repository<Certificate>().Delete(certificate.Id);
            }

            await unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}
