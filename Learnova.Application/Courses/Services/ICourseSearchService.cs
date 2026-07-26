using Learnova.Domain.Entities;

namespace Learnova.Application.Courses.Services
{
    public interface ICourseSearchService
    {
        Task<List<int>> SearchAsync(string searchTerm, int limit = 10, CancellationToken cancellationToken = default);

        Task IndexCourseAsync(Course course, CancellationToken cancellationToken = default);

        Task DeleteCourseAsync(int courseId, CancellationToken cancellationToken = default);
    }
}
