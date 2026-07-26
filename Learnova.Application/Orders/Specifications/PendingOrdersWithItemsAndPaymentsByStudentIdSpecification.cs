using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Application.Orders.Specifications
{
    public class PendingOrdersWithItemsAndPaymentsByStudentIdSpecification : BaseSpecification<Order>
    {
        public PendingOrdersWithItemsAndPaymentsByStudentIdSpecification(string studentId)
            : base(x => x.StudentId == studentId && x.Status == OrderStatus.Pending)
        {
            AddInclude(query => query
                .Include(x => x.Items)
                .Include(x => x.PaymentTransactions)
                .AsSplitQuery());
        }
    }
}
