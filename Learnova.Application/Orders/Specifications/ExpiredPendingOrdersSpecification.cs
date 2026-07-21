using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Application.Orders.Specifications
{
    public class ExpiredPendingOrdersSpecification : BaseSpecification<Order>
    {
        public ExpiredPendingOrdersSpecification(DateTime cutoff)
            : base(order => order.Status == OrderStatus.Pending && order.CreatedAt <= cutoff)
        {
            AddInclude(query => query
                .Include(order => order.PaymentTransactions)
                .AsSplitQuery());
        }
    }
}
