using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Application.Payments.Specifications
{
    public class PaymentTransactionByProviderTransactionIdSpecification
        : BaseSpecification<PaymentTransaction>
    {
        public PaymentTransactionByProviderTransactionIdSpecification(
            string provider,
            string providerTransactionId)
            : base(x => x.Provider == provider &&
                x.ProviderTransactionId == providerTransactionId)
        {
            AddInclude(query => query
                .Include(x => x.Order)
                .ThenInclude(x => x.PaymentTransactions)
                .AsSplitQuery());
        }
    }
}
