using Learnova.Application.Common.BackgroundJobs;
using Learnova.Application.Orders.Specifications;
using Learnova.Application.Payments.Gateway;
using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Orders.Jobs
{
    public sealed class PendingOrderCleanupJob(
        IUnitOfWork unitOfWork,
        IPaymentGatewayService paymentGatewayService,
        ILogger<PendingOrderCleanupJob> logger) : IPendingOrderCleanupJob
    {
        private const int ExpirationHours = 24;

        public async Task CleanupAsync()
        {
            var now = DateTime.UtcNow;
            var cutoff = now.AddHours(-ExpirationHours);

            logger.LogInformation(
                "Pending order cleanup job started. Cutoff: {Cutoff}.",
                cutoff);

            var spec = new ExpiredPendingOrdersSpecification(cutoff);
            var orders = await unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            var cancelledOrdersCount = 0;
            var expiredPaymentsCount = 0;

            foreach (var order in orders)
            {
                if (order.PaymentTransactions.Any(payment => payment.Status == PaymentStatus.Success))
                {
                    logger.LogWarning(
                        "Pending order cleanup skipped Order {OrderId} because it has a successful payment.",
                        order.Id);

                    continue;
                }

                var pendingPayments = order.PaymentTransactions
                    .Where(payment => payment.Status == PaymentStatus.Pending)
                    .ToList();

                var stripePaymentsToExpire = new List<PaymentTransaction>();
                var shouldSkipOrder = false;

                foreach (var payment in pendingPayments)
                {
                    if (!string.Equals(payment.Provider, "Stripe", StringComparison.OrdinalIgnoreCase) ||
                        string.IsNullOrWhiteSpace(payment.ProviderTransactionId))
                    {
                        continue;
                    }

                    var checkoutSession = await paymentGatewayService.GetCheckoutSessionAsync(
                        payment.ProviderTransactionId,
                        CancellationToken.None);

                    if (checkoutSession?.IsCompleted == true)
                    {
                        logger.LogWarning(
                            "Pending order cleanup skipped Order {OrderId} because Stripe session {SessionId} is completed.",
                            order.Id,
                            payment.ProviderTransactionId);

                        shouldSkipOrder = true;
                        break;
                    }

                    if (checkoutSession?.IsActive == true)
                    {
                        stripePaymentsToExpire.Add(payment);
                    }
                }

                if (shouldSkipOrder)
                {
                    continue;
                }

                foreach (var payment in stripePaymentsToExpire)
                {
                    await paymentGatewayService.ExpireCheckoutSessionAsync(
                        payment.ProviderTransactionId!,
                        CancellationToken.None);
                }

                foreach (var payment in pendingPayments)
                {
                    payment.Status = PaymentStatus.Expired;
                    payment.ExpiredAt = now;
                    expiredPaymentsCount++;
                }

                order.Status = OrderStatus.Cancelled;
                order.CancelledAt = now;
                cancelledOrdersCount++;
            }

            await unitOfWork.CompleteAsync();

            logger.LogInformation(
                "Pending order cleanup job completed. Cancelled orders: {CancelledOrdersCount}, Expired payments: {ExpiredPaymentsCount}.",
                cancelledOrdersCount,
                expiredPaymentsCount);
        }
    }
}
