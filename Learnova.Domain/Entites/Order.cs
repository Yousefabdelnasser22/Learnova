using Learnova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = null!;

        public string StudentId { get; set; } = null!;

        public decimal TotalAmount { get; set; }
        public Currency Currency { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public ApplicationUser Student { get; set; } = null!;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
            = new List<PaymentTransaction>();
    }
}
