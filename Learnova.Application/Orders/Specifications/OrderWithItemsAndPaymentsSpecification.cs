using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Application.Orders.Specifications
{
    public class OrderWithItemsAndPaymentsSpecification : BaseSpecification<Order>
    {
        public OrderWithItemsAndPaymentsSpecification(int orderId)
            : base(x => x.Id == orderId)
        {
            AddInclude(query => query
                .Include(x => x.Items)
                .Include(x => x.PaymentTransactions)
                .AsSplitQuery());
        }
    }
}
