namespace Learnova.Application.Enrollment.Services
{
    public interface IEnrollmentProgressService
    {
        Task RecalculateModuleProgressAsync(
            string studentId,
            int moduleId,
            CancellationToken cancellationToken = default);

        Task RecalculateCourseProgressAsync(
            string studentId,
            int courseId,
            CancellationToken cancellationToken = default);
    }
}
