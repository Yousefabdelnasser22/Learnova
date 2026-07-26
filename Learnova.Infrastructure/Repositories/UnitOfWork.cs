using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using Learnova.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Learnova.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = new();

        public AppDbContext Context { get; }

        public IGenericRepository<Course> course { get; }

        public IGenericRepository<Learnova.Domain.Entities.Module> module { get; }

        public IGenericRepository<Lesson> lesson { get; }

        public IEnrollmentRepository enrollment { get; }

        public ILessonProgressRepository lessonProgress { get; }

        public IModuleProgressRepository moduleProgress { get; }

        public IGenericRepository<Quiz> quiz { get; }

        public IQuizQuestionRepository quizQuestion { get; }

        public IGenericRepository<QuizAttempt> quizAttempt { get; }

        public IGenericRepository<Certificate> certificate { get; }

        public IGenericRepository<Review> review { get; }

        public IGenericRepository<Category> category { get; }

        public IGenericRepository<SubCategory> subCategory { get; }

        public UnitOfWork(AppDbContext context)
        {
            Context = context;

            course = Repository<Course>();
            module = Repository<Learnova.Domain.Entities.Module>();
            lesson = Repository<Lesson>();
            enrollment = new EnrollmentRepository(Context);
            lessonProgress = new LessonProgressRepository(Context);
            moduleProgress = new ModuleProgressRepository(Context);
            quiz = Repository<Quiz>();
            quizQuestion = new QuizQuestionRepository(Context);
            quizAttempt = Repository<QuizAttempt>();
            certificate = Repository<Certificate>();
            review = Repository<Review>();
            category = Repository<Category>();
            subCategory = Repository<SubCategory>();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var entityType = typeof(TEntity);

            if (!_repositories.TryGetValue(entityType, out var repository))
            {
                repository = new GenericRepository<TEntity>(Context);

                _repositories.Add(entityType, repository);
            }

            return (IGenericRepository<TEntity>)repository;
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
        {
            if (Context.Database.CurrentTransaction is not null)
            {
                await operation(cancellationToken);
                return;
            }

            await using var transaction = await Context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

            try
            {
                await operation(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
