using Learnova.Domain.Entities;

namespace Learnova.Application.Courses.Services
{
    public interface ICourseContentChangeService
    {
        bool MarkPendingReviewIfPublished(Course course);

        Task RemoveFromDiscoveryIfNeededAsync(
            int courseId,
            bool wasPublished,
            CancellationToken cancellationToken = default);
    }
}
