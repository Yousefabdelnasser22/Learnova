using Learnova.Domain.Enums;

namespace Learnova.Application.Payments.DTO
{
    public class OrderPaymentStatusDto
    {
        public int OrderId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public bool IsPaid { get; set; }

        public string Message { get; set; } = null!;

        public string? CheckoutSessionId { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
