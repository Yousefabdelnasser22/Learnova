using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using Learnova.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Repositories
{
    public class LessonProgressRepository : GenericRepository<LessonProgress>, ILessonProgressRepository
    {
        private readonly AppDbContext _context;

        public LessonProgressRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<LessonProgress?> checkLessonProgress(string studentId, int lessonId)
        {

           return await _context.LessonProgress
             .FirstOrDefaultAsync(lp => lp.StudentId == studentId
                                    && lp.LessonId == lessonId&&!lp.IsDeleted);
        }

    }
}
