using Learnova.Domain.Entites;
using Learnova.Domain.Specifications;
using System.Linq.Expressions;

namespace Learnova.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllWithCondition(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includes);

        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec);

        Task<bool> AnyWithSpecAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, object>>? include = null);
        Task<T?> GetById(int id, Expression<Func<T, object>>? include = null);

        Task<int> CountWithSpecAsync(ISpecification<T> spec);

        Task Add(T entity);

        void Update(T entity);

        Task Delete(int id);

        void HardDelete(T entity);
    }
}
