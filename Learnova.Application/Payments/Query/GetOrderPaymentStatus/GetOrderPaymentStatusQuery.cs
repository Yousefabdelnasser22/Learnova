using Learnova.Application.Payments.DTO;
using MediatR;

namespace Learnova.Application.Payments.Query.GetOrderPaymentStatus
{
    public class GetOrderPaymentStatusQuery(int orderId) : IRequest<OrderPaymentStatusDto>
    {
        public int OrderId { get; } = orderId;
    }
}
