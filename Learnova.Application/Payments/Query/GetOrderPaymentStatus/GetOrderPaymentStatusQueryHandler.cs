using Learnova.Application.Exceptions;
using Learnova.Application.Orders.Specifications;
using Learnova.Application.Payments.DTO;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;

namespace Learnova.Application.Payments.Query.GetOrderPaymentStatus
{
    public class GetOrderPaymentStatusQueryHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext)
        : IRequestHandler<GetOrderPaymentStatusQuery, OrderPaymentStatusDto>
    {
        public async Task<OrderPaymentStatusDto> Handle(
            GetOrderPaymentStatusQuery request,
            CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedException("User is not authenticated.");

            var spec = new OrderWithPaymentsSpecification(request.OrderId);

            var order = await unitOfWork
                .Repository<Order>()
                .GetEntityWithSpecAsync(spec);

            if (order is null)
                throw new NotFoundException("Order not found.");

            if (order.StudentId != user.Id)
                throw new ForbiddenAccessException("You are not allowed to view payment status for this order.");

            var payment = order.PaymentTransactions
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .FirstOrDefault();

            var isPaid = order.Status == OrderStatus.Paid &&
                order.PaymentTransactions.Any(x =>
                    x.Status == PaymentStatus.Success ||
                    x.Status == PaymentStatus.PartiallyRefunded);

            return new OrderPaymentStatusDto
            {
                OrderId = order.Id,
                OrderStatus = order.Status,
                PaymentStatus = payment?.Status,
                IsPaid = isPaid,
                Message = GetPaymentStatusMessage(order.Status, payment?.Status, isPaid),
                CheckoutSessionId = payment?.ProviderTransactionId,
                PaidAt = order.PaidAt,
                CreatedAt = order.CreatedAt
            };
        }

        private static string GetPaymentStatusMessage(
            OrderStatus orderStatus,
            PaymentStatus? paymentStatus,
            bool isPaid)
        {
            if (paymentStatus is null)
                return "Order has no payment transaction yet.";

            if (paymentStatus == PaymentStatus.Refunded)
                return "Payment was refunded.";

            if (paymentStatus == PaymentStatus.PartiallyRefunded)
                return "Payment was partially refunded.";

            if (paymentStatus == PaymentStatus.Expired)
                return "Payment session expired. Please start a new payment attempt.";

            if (isPaid)
                return "Payment confirmed.";

            return orderStatus switch
            {
                OrderStatus.Pending => "Payment is still pending confirmation.",
                OrderStatus.Failed => "Payment failed.",
                OrderStatus.Cancelled => "Payment was cancelled.",
                OrderStatus.Refunded => "Payment was refunded.",
                _ => "Payment is not confirmed."
            };
        }
    }
}
