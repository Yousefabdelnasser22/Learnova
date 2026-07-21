using Learnova.Application.Certificates.Specifications;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Orders.Specifications;
using Learnova.Application.Payments.Specifications;
using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Payments.Commands.HandleStripeRefund
{
    using EnrollmentEntity = Learnova.Domain.Entites.Enrollment;

    public class HandleStripeRefundCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<HandleStripeRefundCommandHandler> logger)
        : IRequestHandler<HandleStripeRefundCommand>
    {
        private const string Provider = "Stripe";

        public async Task Handle(
            HandleStripeRefundCommand request,
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

            var paymentSpec = new PaymentTransactionByStripeIdentifiersSpecification(
                Provider,
                request.StripeChargeId,
                request.StripePaymentIntentId,
                request.ProviderTransactionId);

            var payment = await unitOfWork
                .Repository<PaymentTransaction>()
                .GetEntityWithSpecAsync(paymentSpec);

            if (payment is null)
            {
                logger.LogWarning(
                    "Stripe refund webhook ignored because payment transaction was not found. EventId: {EventId}, EventType: {EventType}, ChargeId: {ChargeId}, PaymentIntentId: {PaymentIntentId}",
                    request.EventId,
                    request.EventType,
                    request.StripeChargeId,
                    request.StripePaymentIntentId);

                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            ApplyWebhookAudit(payment, request);
            ApplyStripeIdentifiers(payment, request);

            if (IsFailedRefund(request))
            {
                payment.RefundId = request.RefundId ?? payment.RefundId;
                payment.FailureReason = request.FailureReason ?? request.RefundStatus;

                logger.LogWarning(
                    "Stripe refund failed. EventId: {EventId}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}, RefundId: {RefundId}, FailureReason: {FailureReason}",
                    request.EventId,
                    payment.OrderId,
                    payment.Id,
                    request.RefundId,
                    payment.FailureReason);

                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            if (IsRefundObjectEvent(request) &&
                !IsSucceededRefund(request))
            {
                if (IsCanceledRefund(request))
                {
                    payment.RefundId = request.RefundId ?? payment.RefundId;
                    payment.FailureReason = request.FailureReason ?? request.RefundStatus;

                    logger.LogWarning(
                        "Stripe refund was not completed. EventId: {EventId}, EventType: {EventType}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}, RefundId: {RefundId}, RefundStatus: {RefundStatus}",
                        request.EventId,
                        request.EventType,
                        payment.OrderId,
                        payment.Id,
                        request.RefundId,
                        request.RefundStatus);
                }
                else
                {
                    logger.LogInformation(
                        "Stripe refund webhook recorded without changing payment state because refund is not succeeded. EventId: {EventId}, EventType: {EventType}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}, RefundId: {RefundId}, RefundStatus: {RefundStatus}",
                        request.EventId,
                        request.EventType,
                        payment.OrderId,
                        payment.Id,
                        request.RefundId,
                        request.RefundStatus);
                }

                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            if (!CanApplyRefund(payment.Status))
            {
                logger.LogInformation(
                    "Stripe refund webhook ignored because payment status cannot be refunded. EventId: {EventId}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}, PaymentStatus: {PaymentStatus}",
                    request.EventId,
                    payment.OrderId,
                    payment.Id,
                    payment.Status);

                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            var refundedAmount = ResolveRefundedAmount(payment, request);

            if (refundedAmount <= 0)
            {
                logger.LogInformation(
                    "Stripe refund webhook ignored because refunded amount is missing or zero. EventId: {EventId}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}",
                    request.EventId,
                    payment.OrderId,
                    payment.Id);

                await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);
                return;
            }

            payment.RefundId = request.RefundId ?? payment.RefundId;
            payment.RefundedAmount = refundedAmount;
            payment.RefundedAt = request.RefundCreatedAt ?? payment.RefundedAt ?? DateTime.UtcNow;

            if (refundedAmount >= payment.Amount)
            {
                payment.Status = PaymentStatus.Refunded;
                payment.Order.Status = OrderStatus.Refunded;
                payment.Order.RefundedAt = payment.RefundedAt;

                await RevokeRefundedEnrollmentsAsync(payment, cancellationToken);
            }
            else
            {
                payment.Status = PaymentStatus.PartiallyRefunded;
            }

            await AddProcessedWebhookEventAndSaveAsync(request, cancellationToken);

            logger.LogInformation(
                "Stripe refund webhook processed. EventId: {EventId}, EventType: {EventType}, OrderId: {OrderId}, PaymentTransactionId: {PaymentTransactionId}, PaymentStatus: {PaymentStatus}, RefundedAmount: {RefundedAmount}",
                request.EventId,
                request.EventType,
                payment.OrderId,
                payment.Id,
                payment.Status,
                payment.RefundedAmount);
        }

        private async Task RevokeRefundedEnrollmentsAsync(
            PaymentTransaction payment,
            CancellationToken cancellationToken)
        {
            foreach (var courseId in payment.Order.Items.Select(x => x.CourseId).Distinct())
            {
                var laterPaidOrderSpec = new PaidOrderItemByStudentAndCourseSpecification(
                    payment.Order.StudentId,
                    courseId,
                    payment.OrderId);

                var hasAnotherPaidOrder = await unitOfWork
                    .Repository<OrderItem>()
                    .AnyWithSpecAsync(laterPaidOrderSpec);

                if (hasAnotherPaidOrder)
                    continue;

                var enrollmentSpec = new EnrollmentByStudentAndCourseSpecification(
                    payment.Order.StudentId,
                    courseId);

                var enrollment = await unitOfWork
                    .Repository<EnrollmentEntity>()
                    .GetEntityWithSpecAsync(enrollmentSpec);

                if (enrollment is null || enrollment.Status == EnrollmentStatus.Revoked)
                    continue;

                enrollment.Status = EnrollmentStatus.Revoked;
                enrollment.IsCompleted = false;
                enrollment.CompletedAt = null;

                var certificateSpec = new CertificateByStudentAndCourseSpecification(
                    payment.Order.StudentId,
                    courseId);

                var certificate = await unitOfWork
                    .Repository<Certificate>()
                    .GetEntityWithSpecAsync(certificateSpec);

                if (certificate is not null)
                {
                    await unitOfWork.Repository<Certificate>().Delete(certificate.Id);
                }
            }
        }

        private static bool CanApplyRefund(PaymentStatus status)
        {
            return status == PaymentStatus.Success ||
                status == PaymentStatus.PartiallyRefunded ||
                status == PaymentStatus.Refunded;
        }

        private static bool IsFailedRefund(HandleStripeRefundCommand request)
        {
            return string.Equals(request.EventType, "refund.failed", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(request.RefundStatus, "failed", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCanceledRefund(HandleStripeRefundCommand request)
        {
            return string.Equals(request.RefundStatus, "canceled", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRefundObjectEvent(HandleStripeRefundCommand request)
        {
            return string.Equals(request.EventType, "refund.created", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(request.EventType, "refund.updated", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSucceededRefund(HandleStripeRefundCommand request)
        {
            return string.Equals(request.RefundStatus, "succeeded", StringComparison.OrdinalIgnoreCase);
        }

        private static decimal ResolveRefundedAmount(
            PaymentTransaction payment,
            HandleStripeRefundCommand request)
        {
            if (request.CumulativeRefundedAmountMinor.HasValue)
                return ToMajorUnits(request.CumulativeRefundedAmountMinor.Value);

            if (!request.RefundAmountMinor.HasValue)
                return payment.RefundedAmount ?? 0m;

            var refundAmount = ToMajorUnits(request.RefundAmountMinor.Value);

            if (payment.RefundedAmount is null)
                return refundAmount;

            if (!string.IsNullOrWhiteSpace(request.RefundId) &&
                string.Equals(payment.RefundId, request.RefundId, StringComparison.Ordinal))
            {
                return Math.Max(payment.RefundedAmount.Value, refundAmount);
            }

            return Math.Max(payment.RefundedAmount.Value, refundAmount);
        }

        private static decimal ToMajorUnits(long amountMinor)
        {
            return amountMinor / 100m;
        }

        private static void ApplyStripeIdentifiers(
            PaymentTransaction payment,
            HandleStripeRefundCommand request)
        {
            payment.StripeChargeId = request.StripeChargeId ?? payment.StripeChargeId;
            payment.StripePaymentIntentId = request.StripePaymentIntentId ?? payment.StripePaymentIntentId;
        }

        private static void ApplyWebhookAudit(
            PaymentTransaction payment,
            HandleStripeRefundCommand request)
        {
            payment.LastWebhookEventId = request.EventId;
            payment.LastWebhookEventType = request.EventType;
            payment.LastWebhookReceivedAt = DateTime.UtcNow;
        }

        private async Task AddProcessedWebhookEventAndSaveAsync(
            HandleStripeRefundCommand request,
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
