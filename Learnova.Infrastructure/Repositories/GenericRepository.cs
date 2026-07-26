using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using Learnova.Domain.Specifications;
using Learnova.Infrastructure.Data;
using Learnova.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Learnova.Infrastructure.Repositories
{
    public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : BaseEntity
    {
        public async Task Add(T entity)
        {
            await context.Set<T>().AddAsync(entity);   
        }

        public async Task Delete(int id)
        {
            var entity = await context.Set<T>().FindAsync(id);
            if (entity is null)
            {
                return;
            }

            entity.IsDeleted=true;
        }

        public void HardDelete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllWithCondition(Expression<Func<T, bool>>? filter = null,params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            var query = SpecificationEvaluator.GetQuery(context.Set<T>(), spec);

            return await query.ToListAsync();
        }

        public async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec)
        {
            var query = SpecificationEvaluator.GetQuery(context.Set<T>(), spec);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, object>>? include = null)
        {
            var query = context.Set<T>().AsQueryable();

            if (include != null)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }
        public async Task<T?> GetById(int id, Expression<Func<T, object>>? include = null)
        {
           
            var query = context.Set<T>().Where(o => o.Id == id);

          
            if (include != null)
            {
                query = query.Include(include); 
            }

           
            return await query.FirstOrDefaultAsync();
        }

        public void Update(T entity)
        {
           context.Set<T>().Update(entity);

        }

        public async Task<bool> AnyWithSpecAsync(ISpecification<T> spec)
        {
            var query = SpecificationEvaluator.GetQuery(
                context.Set<T>(),
                spec
            );

            return await query.AnyAsync();
        }

        public async Task<int> CountWithSpecAsync(ISpecification<T> spec)
        {
            var query = SpecificationEvaluator.GetQuery(
                context.Set<T>(),
                spec
            );

            return await query.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }
    }
}
