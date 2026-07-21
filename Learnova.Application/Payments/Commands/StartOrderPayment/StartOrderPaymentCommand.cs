using Learnova.Application.Payments.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Payments.Commands.StartOrderPayment
{
    public class StartOrderPaymentCommand(int orderId)
    : IRequest<StartOrderPaymentResultDto>
    {
        public int OrderId { get; set; } = orderId;
    }
}
