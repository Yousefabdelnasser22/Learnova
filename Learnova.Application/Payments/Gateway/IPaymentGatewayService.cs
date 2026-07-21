using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Payments.Gateway
{
    public interface IPaymentGatewayService
    {
        Task<CreateGatewayPaymentResult> CreateCheckoutSessionAsync(
        CreateGatewayPaymentRequest request,
        CancellationToken cancellationToken);

        Task<GatewayCheckoutSession?> GetCheckoutSessionAsync(
            string providerTransactionId,
            CancellationToken cancellationToken);

        Task ExpireCheckoutSessionAsync(
            string providerTransactionId,
            CancellationToken cancellationToken);
    }
}
