using Learnova.Domain.Entities;

namespace Learnova.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Course> course { get; }
        IGenericRepository<Entities.Module> module { get; }
        IGenericRepository<Lesson> lesson { get; }

        IEnrollmentRepository enrollment { get; }

        ILessonProgressRepository lessonProgress { get; }
        IModuleProgressRepository moduleProgress { get; }
        IGenericRepository<Quiz> quiz { get; }
        IQuizQuestionRepository quizQuestion { get; }
        IGenericRepository<QuizAttempt> quizAttempt { get; }
        IGenericRepository<Certificate> certificate { get; }
        IGenericRepository<Review> review { get; }
        IGenericRepository<Category> category { get; }
        IGenericRepository<SubCategory> subCategory { get; }
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> operation,
            CancellationToken cancellationToken = default);
    }
}
