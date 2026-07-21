using Learnova.Application.User;

namespace Learnova.Application.Courses.Services
{
    public interface ICourseAccessService
    {
        Task EnsureCanViewCourseContentAsync(
            int courseId,
            CurrentUser user,
            CancellationToken cancellationToken = default);

        Task EnsureStudentEnrolledInCourseAsync(
            int courseId,
            string userId,
            CancellationToken cancellationToken = default);

        Task EnsureInstructorOwnsCourseAsync(
            int courseId,
            string userId,
            CancellationToken cancellationToken = default);
    }
}
