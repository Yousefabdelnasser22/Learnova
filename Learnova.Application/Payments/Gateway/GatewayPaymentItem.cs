using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Payments.Gateway
{
    public class GatewayPaymentItem
    {

        public string Name { get; set; } = null!;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; } = 1;
    }
}
