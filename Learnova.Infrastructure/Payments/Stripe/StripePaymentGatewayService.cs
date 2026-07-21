using Learnova.Application.Payments.Gateway;
using Learnova.Domain.Enums;
using Learnova.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Learnova.Infrastructure.Payments.Stripe
{
    public class StripePaymentGatewayService : IPaymentGatewayService
    {
        private readonly StripeSettings settings;

        public StripePaymentGatewayService(IOptions<StripeSettings> options)
        {
            settings = options.Value;
            StripeConfiguration.ApiKey = settings.SecretKey;
        }

        public async Task<CreateGatewayPaymentResult> CreateCheckoutSessionAsync(
            CreateGatewayPaymentRequest request,
            CancellationToken cancellationToken)
        {
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = settings.SuccessUrl,
                CancelUrl = settings.CancelUrl,
                ClientReferenceId = request.OrderId.ToString(),

                Metadata = new Dictionary<string, string>
                {
                    ["orderId"] = request.OrderId.ToString(),
                    ["orderNumber"] = request.OrderNumber,
                    ["paymentTransactionId"] = request.PaymentTransactionId.ToString()
                },

                LineItems = request.Items.Select(item => new SessionLineItemOptions
                {
                    Quantity = item.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = MapCurrency(request.Currency),
                        UnitAmount = ToStripeAmount(item.UnitPrice),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name
                        }
                    }
                }).ToList()
            };

            var service = new SessionService();

            var session = await service.CreateAsync(
                options,
                cancellationToken: cancellationToken);

            return new CreateGatewayPaymentResult
            {
                Provider = "Stripe",
                ProviderTransactionId = session.Id,
                CheckoutUrl = session.Url
            };
        }

        public async Task<GatewayCheckoutSession?> GetCheckoutSessionAsync(
            string providerTransactionId,
            CancellationToken cancellationToken)
        {
            var service = new SessionService();

            try
            {
                var session = await service.GetAsync(
                    providerTransactionId,
                    cancellationToken: cancellationToken);

                return new GatewayCheckoutSession
                {
                    ProviderTransactionId = session.Id,
                    CheckoutUrl = session.Url,
                    IsActive = string.Equals(session.Status, "open", StringComparison.OrdinalIgnoreCase),
                    IsCompleted = string.Equals(session.Status, "complete", StringComparison.OrdinalIgnoreCase),
                    PaymentStatus = session.PaymentStatus
                };
            }
            catch (StripeException ex) when (ex.StripeError?.Type == "invalid_request_error")
            {
                return null;
            }
        }

        public async Task ExpireCheckoutSessionAsync(
            string providerTransactionId,
            CancellationToken cancellationToken)
        {
            var service = new SessionService();

            try
            {
                await service.ExpireAsync(
                    providerTransactionId,
                    cancellationToken: cancellationToken);
            }
            catch (StripeException ex) when (ex.StripeError?.Type == "invalid_request_error")
            {
            }
        }

        private static string MapCurrency(Currency currency)
        {
            return currency switch
            {
                Currency.EGP => "egp",
                Currency.USD => "usd",
                _ => throw new InvalidOperationException("Unsupported currency.")
            };
        }

        private static long ToStripeAmount(decimal amount)
        {
            return decimal.ToInt64(decimal.Round(amount * 100, 0, MidpointRounding.AwayFromZero));
        }
    }
}
