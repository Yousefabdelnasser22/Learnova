using Learnova.Application.Payments.Specifications;
using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Payments.Commands.ExpireStripeCheckoutPayment
{
    public class ExpireStripeCheckoutPaymentCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ExpireStripeCheckoutPaymentCommandHandler> logger)
        : IRequestHandler<ExpireStripeCheckoutPaymentCommand>
    {
        private const string Provider = "Stripe";

        public async Task Handle(
            ExpireStripeCheckoutPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var processedEventSpec = new ProcessedWebhookEventByEventIdSpecification(
                Provider,
                request.EventId);

            var existingProcessedEvent = await unitOfWork
                .Repository<ProcessedWebhookEvent>()
                .GetEntityWithSpecAsync(processedEventSpec);

            if (existingProcessedEvent is not null)
            {
                logger.LogInformation(
                    "Duplicate Stripe webhook event ignored. EventId: {EventId}, EventType: {EventType}",
                    request.EventId,
                    request.EventType);

                return;
            }

            var paymentSpec = new PaymentTransactionByProviderTransactionIdSpecification(
                Provider,
                request.ProviderTransactionId);

            var payment = await unitOfWork
                .Repository<PaymentTransaction>()
                .GetEntityWithSpecAsync(paymentSpec);

            if (payment is null)
            {
                logger.LogWarning(
                    "Stripe checkout.session.expired ignored because payment transaction was not found. EventId: {EventId}, SessionId: {SessionId}",
                    request.EventId,
                    request.ProviderTransactionId);

                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            if (payment.Status == PaymentStatus.Success ||
                payment.Order.Status == OrderStatus.Paid)
            {
                logger.LogInformation(
                    "Stripe checkout.session.expired ignored because payment is already successful or order is already paid. EventId: {EventId}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}",
                    request.EventId,
                    payment.OrderId,
                    payment.Id);

                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            if (payment.Status == PaymentStatus.Pending)
            {
                var now = DateTime.UtcNow;

                payment.Status = PaymentStatus.Expired;
                payment.ExpiredAt = now;

                if (payment.Order.Status == OrderStatus.Pending &&
                    !payment.Order.PaymentTransactions.Any(x => x.Status == PaymentStatus.Pending))
                {
                    payment.Order.Status = OrderStatus.Cancelled;
                    payment.Order.CancelledAt = now;
                }

                logger.LogInformation(
                    "Stripe checkout session marked as expired. EventId: {EventId}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}",
                    request.EventId,
                    payment.OrderId,
                    payment.Id);
            }
            else
            {
                logger.LogInformation(
                    "Stripe checkout.session.expired ignored because payment transaction is not pending. EventId: {EventId}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}, PaymentStatus: {PaymentStatus}",
                    request.EventId,
                    payment.OrderId,
                    payment.Id,
                    payment.Status);
            }

            await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
        }

        private async Task AddProcessedWebhookEventAndSaveAsync(
            ExpireStripeCheckoutPaymentCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.Repository<ProcessedWebhookEvent>().Add(new ProcessedWebhookEvent
            {
                Provider = Provider,
                EventId = request.EventId,
                EventType = request.EventType,
                ProcessedAt = DateTime.UtcNow
            });

            try
            {
                await unitOfWork.CompleteAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (
                ex.InnerException?.Message.Contains("IX_ProcessedWebhookEvents_Provider_EventId") == true)
            {
                logger.LogInformation(
                    "Duplicate Stripe webhook event ignored while saving processed event. EventId: {EventId}, EventType: {EventType}",
                    request.EventId,
                    request.EventType);
            }
        }
    }
}
