using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Application.Payments.Specifications
{
    public class PaymentTransactionByStripeIdentifiersSpecification
        : BaseSpecification<PaymentTransaction>
    {
        public PaymentTransactionByStripeIdentifiersSpecification(
            string provider,
            string? stripeChargeId,
            string? stripePaymentIntentId,
            string? providerTransactionId)
            : base(x => x.Provider == provider &&
                ((stripeChargeId != null && stripeChargeId != "" && x.StripeChargeId == stripeChargeId) ||
                 (stripePaymentIntentId != null && stripePaymentIntentId != "" && x.StripePaymentIntentId == stripePaymentIntentId) ||
                 (providerTransactionId != null && providerTransactionId != "" && x.ProviderTransactionId == providerTransactionId)))
        {
            AddInclude(query => query
                .Include(x => x.Order)
                .ThenInclude(x => x.Items)
                .AsSplitQuery());
        }
    }
}
