using Learnova.Domain.Entites;

namespace Learnova.Domain.Interfaces
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, int courseId);
    }
}
