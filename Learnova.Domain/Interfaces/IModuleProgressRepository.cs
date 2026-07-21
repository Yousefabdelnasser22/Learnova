using Learnova.Domain.Entites;

namespace Learnova.Domain.Interfaces
{
    public interface IModuleProgressRepository : IGenericRepository<ModuleProgress>
    {
        Task<ModuleProgress?> CheckModuleProgressAsync(string studentId, int moduleId);
    }
}
