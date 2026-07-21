using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed,
        Refunded,
        Expired,
        PartiallyRefunded
    }
}
