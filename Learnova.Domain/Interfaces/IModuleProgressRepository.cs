using Learnova.Domain.Entities;

namespace Learnova.Domain.Interfaces
{
    public interface IModuleProgressRepository : IGenericRepository<ModuleProgress>
    {
        Task<ModuleProgress?> CheckModuleProgressAsync(string studentId, int moduleId);
    }
}
