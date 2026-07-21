using MediatR;

namespace Learnova.Application.Payments.Commands.HandleStripeRefund
{
    public class HandleStripeRefundCommand(
        string eventId,
        string eventType,
        string? refundId,
        string? stripeChargeId,
        string? stripePaymentIntentId,
        string? providerTransactionId,
        long? refundAmountMinor,
        long? cumulativeRefundedAmountMinor,
        string? currency,
        string? refundStatus,
        string? failureReason,
        DateTime? refundCreatedAt) : IRequest
    {
        public string EventId { get; } = eventId;

        public string EventType { get; } = eventType;

        public string? RefundId { get; } = refundId;

        public string? StripeChargeId { get; } = stripeChargeId;

        public string? StripePaymentIntentId { get; } = stripePaymentIntentId;

        public string? ProviderTransactionId { get; } = providerTransactionId;

        public long? RefundAmountMinor { get; } = refundAmountMinor;

        public long? CumulativeRefundedAmountMinor { get; } = cumulativeRefundedAmountMinor;

        public string? Currency { get; } = currency;

        public string? RefundStatus { get; } = refundStatus;

        public string? FailureReason { get; } = failureReason;

        public DateTime? RefundCreatedAt { get; } = refundCreatedAt;
    }
}
