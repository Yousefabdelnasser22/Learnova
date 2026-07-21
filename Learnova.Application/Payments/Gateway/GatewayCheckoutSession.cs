namespace Learnova.Application.Payments.Gateway
{
    public class GatewayCheckoutSession
    {
        public string ProviderTransactionId { get; set; } = null!;

        public string? CheckoutUrl { get; set; }

        public bool IsActive { get; set; }

        public bool IsCompleted { get; set; }

        public string? PaymentStatus { get; set; }
    }
}
