using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using Learnova.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Repositories
{
    public class ModuleProgressRepository : GenericRepository<ModuleProgress>, IModuleProgressRepository
    {
        private readonly AppDbContext _context;

        public ModuleProgressRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ModuleProgress?> CheckModuleProgressAsync(string studentId, int moduleId)
        {
            return await _context.ModuleProgress
                .FirstOrDefaultAsync(mp => mp.StudentId == studentId
                                        && mp.ModuleId == moduleId
                                        && !mp.IsDeleted);
        }

    }
}
