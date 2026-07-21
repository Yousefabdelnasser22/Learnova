using Learnova.Domain.Entites;
using Learnova.Domain.Enums;

public class PaymentTransaction : BaseEntity
{
    public int OrderId { get; set; }

    public decimal Amount { get; set; }
    public Currency Currency { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? Provider { get; set; }
    public string? ProviderTransactionId { get; set; }

    // Stripe audit fields
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? StripeChargeId { get; set; }

    public string? CustomerEmail { get; set; }
    public string? ReceiptUrl { get; set; }

    // Failure / expiration
    public string? FailureReason { get; set; }
    public DateTime? ExpiredAt { get; set; }

    // Refund tracking
    public string? RefundId { get; set; }
    public decimal? RefundedAmount { get; set; }
    public DateTime? RefundedAt { get; set; }

    // Dispute / chargeback tracking
    public string? DisputeId { get; set; }
    public string? DisputeStatus { get; set; }
    public string? DisputeReason { get; set; }
    public decimal? DisputedAmount { get; set; }
    public DateTime? DisputedAt { get; set; }
    public DateTime? DisputeClosedAt { get; set; }

    // Optional webhook trace
    public string? LastWebhookEventId { get; set; }
    public string? LastWebhookEventType { get; set; }
    public DateTime? LastWebhookReceivedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public DateTime? FailedAt { get; set; }

    public Order Order { get; set; } = null!;
}