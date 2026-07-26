using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Specifications
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> GetQuery<TEntity>(
            IQueryable<TEntity> inputQuery,
            ISpecification<TEntity> specification) where TEntity : BaseEntity
        {
            var query = inputQuery;

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            foreach (var include in specification.Includes)
            {
                query = query.Include(include);
            }

            if (specification.IncludeExpression != null)
            {
                query = specification.IncludeExpression(query);
            }

            IOrderedQueryable<TEntity>? orderedQuery = null;

            foreach (var orderByExpression in specification.OrderByExpressions)
            {
                if (orderedQuery == null)
                {
                    orderedQuery = orderByExpression.IsDescending
                        ? query.OrderByDescending(orderByExpression.OrderExpression)
                        : query.OrderBy(orderByExpression.OrderExpression);
                }
                else
                {
                    orderedQuery = orderByExpression.IsDescending
                        ? orderedQuery.ThenByDescending(orderByExpression.OrderExpression)
                        : orderedQuery.ThenBy(orderByExpression.OrderExpression);
                }
            }

            if (orderedQuery != null)
            {
                query = orderedQuery;
            }

            if (specification.IsPaginationEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            return query;
        }
    }
}
