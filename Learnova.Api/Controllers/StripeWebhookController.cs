using Learnova.Application.Payments.Commands.ConfirmStripeCheckoutPayment;
using Learnova.Application.Payments.Commands.ExpireStripeCheckoutPayment;
using Learnova.Application.Payments.Commands.HandleStripeRefund;
using Learnova.Infrastructure.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [ApiController]
    [Route("api/payments/stripe/webhook")]
    [AllowAnonymous]
    public class StripeWebhookController(
        IMediator mediator,
        IOptions<StripeSettings> stripeOptions,
        ILogger<StripeWebhookController> logger)
        : ControllerBase
    {
        [HttpPost]
        [EnableRateLimiting("webhook")]
        [SwaggerOperation(
            Summary = "Handle Stripe webhook events",
            Description = "Validates and processes Stripe Checkout completion, expiration, and refund webhook events.")]
        public async Task<IActionResult> Handle(
            CancellationToken cancellationToken)
        {
            var json = await new StreamReader(HttpContext.Request.Body)
                .ReadToEndAsync(cancellationToken);

            var signatureHeader = Request.Headers["Stripe-Signature"];

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signatureHeader,
                    stripeOptions.Value.WebhookSecret,
                    300,
                    false);
            }
            catch (StripeException ex)
            {
                logger.LogWarning(ex, "Invalid Stripe webhook signature.");
                return BadRequest();
            }

            if (IsRefundEvent(stripeEvent.Type))
            {
                await HandleRefundEvent(stripeEvent, cancellationToken);
                return Ok();
            }

            if (stripeEvent.Type != "checkout.session.completed" &&
                stripeEvent.Type != "checkout.session.expired")
            {
                logger.LogInformation(
                    "Ignored Stripe webhook event type. EventId: {EventId}, EventType: {EventType}",
                    stripeEvent.Id,
                    stripeEvent.Type);

                return Ok();
            }

            var session = stripeEvent.Data.Object as Session;
            if (session is null)
            {
                logger.LogWarning(
                    "Stripe webhook event missing checkout session object. EventId: {EventId}",
                    stripeEvent.Id);

                return Ok();
            }

            if (stripeEvent.Type == "checkout.session.expired")
            {
                if (string.IsNullOrWhiteSpace(session.Id))
                {
                    logger.LogWarning(
                        "Stripe checkout.session.expired event missing session id. EventId: {EventId}",
                        stripeEvent.Id);

                    return Ok();
                }

                await mediator.Send(
                    new ExpireStripeCheckoutPaymentCommand(
                        stripeEvent.Id,
                        stripeEvent.Type,
                        session.Id),
                    cancellationToken);

                return Ok();
            }

            if (session.Metadata is null)
            {
                logger.LogWarning(
                    "Stripe checkout session metadata is missing. EventId: {EventId}, SessionId: {SessionId}",
                    stripeEvent.Id,
                    session.Id);

                return Ok();
            }

            if (!session.Metadata.TryGetValue("orderId", out var orderIdValue))
            {
                logger.LogWarning(
                    "Stripe checkout session metadata missing orderId. EventId: {EventId}, SessionId: {SessionId}",
                    stripeEvent.Id,
                    session.Id);

                return Ok();
            }

            if (!session.Metadata.TryGetValue("paymentTransactionId", out var paymentTransactionIdValue))
            {
                logger.LogWarning(
                    "Stripe checkout session metadata missing paymentTransactionId. EventId: {EventId}, SessionId: {SessionId}",
                    stripeEvent.Id,
                    session.Id);

                return Ok();
            }

            if (!int.TryParse(orderIdValue, out var orderId))
            {
                logger.LogWarning(
                    "Stripe checkout session metadata has malformed orderId. EventId: {EventId}, SessionId: {SessionId}",
                    stripeEvent.Id,
                    session.Id);

                return Ok();
            }

            if (!int.TryParse(paymentTransactionIdValue, out var paymentTransactionId))
            {
                logger.LogWarning(
                    "Stripe checkout session metadata has malformed paymentTransactionId. EventId: {EventId}, SessionId: {SessionId}",
                    stripeEvent.Id,
                    session.Id);

                return Ok();
            }

            await mediator.Send(
                new ConfirmStripeCheckoutPaymentCommand(
                    stripeEvent.Id,
                    stripeEvent.Type,
                    orderId,
                    paymentTransactionId,
                    session.Id,
                    session.PaymentIntentId,
                    session.PaymentStatus,
                    session.AmountTotal,
                    session.Currency),
                cancellationToken);

            return Ok();
        }

        private async Task HandleRefundEvent(
            Event stripeEvent,
            CancellationToken cancellationToken)
        {
            if (stripeEvent.Data.Object is Refund refund)
            {
                await mediator.Send(
                    new HandleStripeRefundCommand(
                        stripeEvent.Id,
                        stripeEvent.Type,
                        refund.Id,
                        refund.ChargeId,
                        refund.PaymentIntentId,
                        null,
                        refund.Amount,
                        null,
                        refund.Currency,
                        refund.Status,
                        refund.FailureReason,
                        refund.Created),
                    cancellationToken);

                return;
            }

            if (stripeEvent.Data.Object is Charge charge)
            {
                await mediator.Send(
                    new HandleStripeRefundCommand(
                        stripeEvent.Id,
                        stripeEvent.Type,
                        null,
                        charge.Id,
                        charge.PaymentIntentId,
                        null,
                        null,
                        charge.AmountRefunded,
                        charge.Currency,
                        charge.Refunded ? "refunded" : null,
                        null,
                        null),
                    cancellationToken);

                return;
            }

            logger.LogWarning(
                "Stripe refund webhook event has unsupported object type. EventId: {EventId}, EventType: {EventType}",
                stripeEvent.Id,
                stripeEvent.Type);
        }

        private static bool IsRefundEvent(string eventType)
        {
            return eventType == "refund.created" ||
                eventType == "refund.updated" ||
                eventType == "refund.failed" ||
                eventType == "charge.refunded";
        }
    }
}
