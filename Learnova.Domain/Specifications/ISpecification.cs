using Learnova.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Learnova.Domain.Specifications
{
    public interface ISpecification<TEntity> where TEntity : BaseEntity
    {
        Expression<Func<TEntity, bool>>? Criteria { get; }

        List<Expression<Func<TEntity, object>>> Includes { get; }

        Func<IQueryable<TEntity>, IQueryable<TEntity>>? IncludeExpression { get; }

        List<OrderExpressionInfo<TEntity>> OrderByExpressions { get; }

        int Take { get; }

        int Skip { get; }

        bool IsPaginationEnabled { get; }
    }
}
