using Learnova.Domain.Entites;

namespace Learnova.Domain.Interfaces
{
    public interface ILessonProgressRepository : IGenericRepository<LessonProgress>
    {
        Task<LessonProgress?> checkLessonProgress(string studentId, int lessonId);
    }
}
