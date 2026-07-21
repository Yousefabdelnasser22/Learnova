using Learnova.Domain.Entites;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Modules.Specifications
{
    public class ModuleProgressByStudentAndModuleSpecification : BaseSpecification<ModuleProgress>
    {
        public ModuleProgressByStudentAndModuleSpecification(string studentId, int moduleId)
            : base(mp => mp.StudentId == studentId && mp.ModuleId == moduleId)
        {
        }
    }
}
