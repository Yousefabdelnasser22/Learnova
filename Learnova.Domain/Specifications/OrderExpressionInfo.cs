using System;
using System.Linq.Expressions;

namespace Learnova.Domain.Specifications
{
    public class OrderExpressionInfo<TEntity>
    {
        public Expression<Func<TEntity, object>> OrderExpression { get; set; } = null!;
        public bool IsDescending { get; set; } = false;
    }
}
