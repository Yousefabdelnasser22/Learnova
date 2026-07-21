using Learnova.Domain.Entites;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Orders.Specifications
{
    public class OrderWithPaymentsSpecification : BaseSpecification<Order>
    {
        public OrderWithPaymentsSpecification(int orderId)
            : base(x => x.Id == orderId)
        {
            AddInclude(x => x.PaymentTransactions);
        }
    }
}
