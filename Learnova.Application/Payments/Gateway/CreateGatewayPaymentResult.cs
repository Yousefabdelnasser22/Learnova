using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Payments.Gateway
{
    public class CreateGatewayPaymentResult
    {
        public string Provider { get; set; } = null!;

        public string ProviderTransactionId { get; set; } = null!;

        public string CheckoutUrl { get; set; } = null!;
    }
}
