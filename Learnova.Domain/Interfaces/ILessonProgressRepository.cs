using Learnova.Domain.Entities;

namespace Learnova.Domain.Interfaces
{
    public interface ILessonProgressRepository : IGenericRepository<LessonProgress>
    {
        Task<LessonProgress?> checkLessonProgress(string studentId, int lessonId);
    }
}
