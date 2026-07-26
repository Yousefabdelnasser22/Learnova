using Learnova.Application.Carts.Specifications;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.Favorites.Specifications;
using Learnova.Application.Orders.Specifications;
using Learnova.Application.Payments.Specifications;
using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Payments.Commands.ConfirmStripeCheckoutPayment
{
    using EnrollmentEntity = Learnova.Domain.Entities.Enrollment;

    public class ConfirmStripeCheckoutPaymentCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ConfirmStripeCheckoutPaymentCommandHandler> logger)
        : IRequestHandler<ConfirmStripeCheckoutPaymentCommand>
    {
        private const string Provider = "Stripe";

        public async Task Handle(
            ConfirmStripeCheckoutPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var processedEventSpec = new ProcessedWebhookEventByEventIdSpecification(Provider, request.EventId);

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

            var orderSpec = new OrderWithItemsAndPaymentsSpecification(request.OrderId);

            var order = await unitOfWork
                .Repository<Order>()
                .GetEntityWithSpecAsync(orderSpec);

            if (order is null)
                throw new NotFoundException($"Order with id {request.OrderId} was not found.");

            if (order.Items is null || order.Items.Count == 0)
                throw new BadRequestException("Order has no items.");

            if (order.PaymentTransactions is null || order.PaymentTransactions.Count == 0)
                throw new BadRequestException("Order has no payment transactions.");

            var payment = order.PaymentTransactions
                .FirstOrDefault(x => x.Id == request.PaymentTransactionId);

            if (payment is null)
                throw new BadRequestException("Payment transaction was not found for this order.");

            if (!string.Equals(payment.Provider, Provider, StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Payment provider mismatch.");

            if (!string.Equals(payment.ProviderTransactionId, request.ProviderTransactionId, StringComparison.Ordinal))
                throw new BadRequestException("Stripe session id does not match the payment transaction.");

            payment.StripeCheckoutSessionId = request.ProviderTransactionId;
            payment.StripePaymentIntentId = request.StripePaymentIntentId ?? payment.StripePaymentIntentId;
            payment.LastWebhookEventId = request.EventId;
            payment.LastWebhookEventType = request.EventType;
            payment.LastWebhookReceivedAt = DateTime.UtcNow;

            if (!IsPaidPaymentStatus(request.PaymentStatus))
            {
                logger.LogWarning(
                    "Stripe checkout.session.completed ignored because payment_status is not paid. EventId: {EventId}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}, PaymentStatus: {PaymentStatus}",
                    request.EventId,
                    request.OrderId,
                    request.PaymentTransactionId,
                    request.PaymentStatus);

                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            if (!IsExpectedAmountAndCurrency(payment, request.AmountTotal, request.Currency))
            {
                logger.LogWarning(
                    "Stripe checkout.session.completed ignored because amount or currency does not match. EventId: {EventId}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}, ExpectedAmountMinor: {ExpectedAmountMinor}, ActualAmountMinor: {ActualAmountMinor}, ExpectedCurrency: {ExpectedCurrency}, ActualCurrency: {ActualCurrency}",
                    request.EventId,
                    request.OrderId,
                    request.PaymentTransactionId,
                    ToStripeMinorUnits(payment.Amount),
                    request.AmountTotal,
                    MapCurrency(payment.Currency),
                    request.Currency);

                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            if (payment.Status == PaymentStatus.Success && order.Status == OrderStatus.Paid)
            {
                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            if (order.Status != OrderStatus.Pending)
                throw new BadRequestException("Only pending orders can be marked as paid.");

            if (payment.Status != PaymentStatus.Pending)
                throw new BadRequestException("Only pending payment transactions can be marked as success.");

            payment.Status = PaymentStatus.Success;
            payment.PaidAt = DateTime.UtcNow;

            order.Status = OrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;

            foreach (var item in order.Items)
            {
                var enrollmentSpec = new EnrollmentByStudentAndCourseSpecification(
                    order.StudentId,
                    item.CourseId);

                var existingEnrollment = await unitOfWork
                    .Repository<EnrollmentEntity>()
                    .GetEntityWithSpecAsync(enrollmentSpec);

                if (existingEnrollment is null)
                {
                    await unitOfWork.Repository<EnrollmentEntity>().Add(new EnrollmentEntity
                    {
                        StudentId = order.StudentId,
                        CourseId = item.CourseId,
                        Status = EnrollmentStatus.Active
                    });
                }
                else
                {
                    existingEnrollment.Status = EnrollmentStatus.Active;
                }

                var favoriteSpec = new FavoriteByStudentAndCourseSpecification(
                    order.StudentId,
                    item.CourseId);

                var favorites = await unitOfWork
                    .Repository<FavoriteList>()
                    .GetAllWithSpecAsync(favoriteSpec);

                foreach (var favorite in favorites)
                {
                    unitOfWork.Repository<FavoriteList>().HardDelete(favorite);
                }
            }

            var cartSpec = new CartWithItemsByStudentIdSpecification(order.StudentId);

            var cart = await unitOfWork
                .Repository<Cart>()
                .GetEntityWithSpecAsync(cartSpec);

            if (cart is not null && cart.Items is not null)
            {
                foreach (var cartItem in cart.Items.ToList())
                {
                    unitOfWork.Repository<CartItem>().HardDelete(cartItem);
                }
            }

            await AddProcessedWebhookEventAsync(request);

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

                return;
            }

            logger.LogInformation(
                "Stripe payment confirmed successfully. EventId: {EventId}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}",
                request.EventId,
                request.OrderId,
                request.PaymentTransactionId);
        }

        internal static bool IsPaidPaymentStatus(string? paymentStatus)
        {
            return string.Equals(paymentStatus, "paid", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsExpectedAmountAndCurrency(
            PaymentTransaction payment,
            long? actualAmountTotal,
            string? actualCurrency)
        {
            if (!actualAmountTotal.HasValue || string.IsNullOrWhiteSpace(actualCurrency))
                return false;

            return actualAmountTotal.Value == ToStripeMinorUnits(payment.Amount) &&
                string.Equals(actualCurrency, MapCurrency(payment.Currency), StringComparison.OrdinalIgnoreCase);
        }

        internal static long ToStripeMinorUnits(decimal amount)
        {
            return decimal.ToInt64(decimal.Round(amount * 100, 0, MidpointRounding.AwayFromZero));
        }

        internal static string MapCurrency(Currency currency)
        {
            return currency switch
            {
                Currency.EGP => "egp",
                Currency.USD => "usd",
                _ => throw new InvalidOperationException("Unsupported currency.")
            };
        }

        private async Task AddProcessedWebhookEventAndSaveAsync(
            ConfirmStripeCheckoutPaymentCommand request,
            CancellationToken cancellationToken)
        {
            await AddProcessedWebhookEventAsync(request);

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

        private async Task AddProcessedWebhookEventAsync(
            ConfirmStripeCheckoutPaymentCommand request)
        {
            await unitOfWork.Repository<ProcessedWebhookEvent>().Add(new ProcessedWebhookEvent
            {
                Provider = Provider,
                EventId = request.EventId,
                EventType = request.EventType,
                ProcessedAt = DateTime.UtcNow
            });
        }
    }
}
