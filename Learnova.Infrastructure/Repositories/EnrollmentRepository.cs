using Learnova.Domain.Entites;
using Learnova.Domain.Interfaces;
using Learnova.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
    {

        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, int courseId)
        {
            return _context.Enrollments
        .FirstOrDefaultAsync(e => e.StudentId == studentId &&
                                  e.CourseId == courseId &&
                                  !e.IsDeleted);
        }

    }
}
