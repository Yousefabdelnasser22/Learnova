using MediatR;

namespace Learnova.Application.Payments.Commands.ExpireStripeCheckoutPayment
{
    public class ExpireStripeCheckoutPaymentCommand(
        string eventId,
        string eventType,
        string providerTransactionId) : IRequest
    {
        public string EventId { get; } = eventId;

        public string EventType { get; } = eventType;

        public string ProviderTransactionId { get; } = providerTransactionId;
    }
}
