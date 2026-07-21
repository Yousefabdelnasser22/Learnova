using Learnova.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Learnova.Domain.Specifications
{
    public abstract class BaseSpecification<TEntity> : ISpecification<TEntity> where TEntity : BaseEntity
    {
        protected BaseSpecification()
        {
        }

        protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<TEntity, bool>>? Criteria { get; }

        public List<Expression<Func<TEntity, object>>> Includes { get; } = new();

        public Func<IQueryable<TEntity>, IQueryable<TEntity>>? IncludeExpression { get; private set; }

        public List<OrderExpressionInfo<TEntity>> OrderByExpressions { get; } = new();

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPaginationEnabled { get; private set; }

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeExpression)
        {
            IncludeExpression = includeExpression;
        }

        protected void AddOrderBy(Expression<Func<TEntity, object>> orderExpression)
        {
            OrderByExpressions.Add(new OrderExpressionInfo<TEntity>
            {
                OrderExpression = orderExpression
            });
        }

        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderExpression)
        {
            OrderByExpressions.Add(new OrderExpressionInfo<TEntity>
            {
                OrderExpression = orderExpression,
                IsDescending = true
            });
        }

        protected void ApplyPagination(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPaginationEnabled = true;
        }
    }
}
