using Learnova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Payments.Gateway
{
    public class CreateGatewayPaymentRequest
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public int PaymentTransactionId { get; set; }

        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public List<GatewayPaymentItem> Items { get; set; } = new();
    }
}
