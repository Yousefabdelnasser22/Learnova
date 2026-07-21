using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Payments.Commands.ConfirmStripeCheckoutPayment
{
    public class ConfirmStripeCheckoutPaymentCommand(
    string eventId,
    string eventType,
    int orderId,
    int paymentTransactionId,
    string providerTransactionId,
    string? stripePaymentIntentId,
    string? paymentStatus,
    long? amountTotal,
    string? currency) : IRequest
    {
        public string EventId { get; } = eventId;

        public string EventType { get; } = eventType;

        public int OrderId { get; } = orderId;

        public int PaymentTransactionId { get; } = paymentTransactionId;

        public string ProviderTransactionId { get; } = providerTransactionId;

        public string? StripePaymentIntentId { get; } = stripePaymentIntentId;

        public string? PaymentStatus { get; } = paymentStatus;

        public long? AmountTotal { get; } = amountTotal;

        public string? Currency { get; } = currency;
    }
}
