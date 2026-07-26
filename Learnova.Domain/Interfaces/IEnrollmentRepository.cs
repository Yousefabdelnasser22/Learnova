using Learnova.Domain.Entities;

namespace Learnova.Domain.Interfaces
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, int courseId);
    }
}
