using Learnova.Application.Carts.Specifications;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.Orders.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;

namespace Learnova.Application.Orders.Command.CheckoutCart
{
    public class CheckoutCartCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork)
        : IRequestHandler<CheckoutCartCommand, int>
    {
        async Task<int> IRequestHandler<CheckoutCartCommand, int>.Handle(
            CheckoutCartCommand request,
            CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException();

            var cartSpec = new CartWithItemsByStudentIdSpecification(user.Id);
            var cart = await unitOfWork.Repository<Cart>().GetEntityWithSpecAsync(cartSpec);

            if (cart is null || !cart.Items.Any())
            {
                throw new BadRequestException("Cart is empty.");
            }

            foreach (var item in cart.Items)
            {
                if (item.Course is null)
                    throw new BadRequestException("One or more cart courses are not available.");

                if (item.Course.Status != CourseStatus.Published)
                    throw new BadRequestException($"Course '{item.Course.Title}' is not available.");

                var activeEnrollmentSpec = new ActiveEnrollmentByStudentAndCourseSpecification(user.Id, item.CourseId);
                var existingEnrollment = await unitOfWork.enrollment.GetEntityWithSpecAsync(activeEnrollmentSpec);

                if (existingEnrollment is not null)
                    throw new BadRequestException($"You are already enrolled in course '{item.Course.Title}'.");

                item.UnitPrice = item.Course.Price;
                item.Currency = item.Course.Currency;
            }

            var currency = cart.Items.First().Currency;

            if (cart.Items.Any(x => x.Currency != currency))
                throw new BadRequestException("All cart items must have the same currency.");

            var totalAmount = cart.Items.Sum(x => x.UnitPrice);

            var existingPendingOrderId = await FindMatchingPendingOrderIdAsync(
                user.Id,
                cart.Items,
                totalAmount,
                currency);

            if (existingPendingOrderId.HasValue)
                return existingPendingOrderId.Value;

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                StudentId = user.Id,
                TotalAmount = totalAmount,
                Currency = currency,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,

                Items = cart.Items.Select(x => new OrderItem
                {
                    CourseId = x.CourseId,
                    UnitPrice = x.UnitPrice,
                    Currency = x.Currency,
                    CourseTitleSnapshot = x.Course.Title
                }).ToList(),

                PaymentTransactions = new List<PaymentTransaction>
                {
                    new()
                    {
                        Amount = totalAmount,
                        Currency = currency,
                        Status = PaymentStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };

            await unitOfWork.Repository<Order>().Add(order);

            await unitOfWork.CompleteAsync(cancellationToken);

            return order.Id;
        }

        private async Task<int?> FindMatchingPendingOrderIdAsync(
            string studentId,
            IEnumerable<CartItem> cartItems,
            decimal totalAmount,
            Currency currency)
        {
            var pendingOrdersSpec = new PendingOrdersWithItemsAndPaymentsByStudentIdSpecification(studentId);
            var pendingOrders = await unitOfWork.Repository<Order>().GetAllWithSpecAsync(pendingOrdersSpec);

            var cartSnapshot = cartItems
                .Select(x => new OrderItemSnapshot(x.CourseId, x.UnitPrice, x.Currency))
                .OrderBy(x => x.CourseId)
                .ThenBy(x => x.UnitPrice)
                .ThenBy(x => x.Currency)
                .ToList();

            foreach (var order in pendingOrders)
            {
                if (order.TotalAmount != totalAmount || order.Currency != currency)
                    continue;

                if (order.PaymentTransactions is null ||
                    !order.PaymentTransactions.Any(x => x.Status == PaymentStatus.Pending))
                    continue;

                var orderSnapshot = order.Items
                    .Select(x => new OrderItemSnapshot(x.CourseId, x.UnitPrice, x.Currency))
                    .OrderBy(x => x.CourseId)
                    .ThenBy(x => x.UnitPrice)
                    .ThenBy(x => x.Currency)
                    .ToList();

                if (SnapshotsMatch(cartSnapshot, orderSnapshot))
                    return order.Id;
            }

            return null;
        }

        private static bool SnapshotsMatch(
            IReadOnlyList<OrderItemSnapshot> cartSnapshot,
            IReadOnlyList<OrderItemSnapshot> orderSnapshot)
        {
            if (cartSnapshot.Count != orderSnapshot.Count)
                return false;

            for (var i = 0; i < cartSnapshot.Count; i++)
            {
                if (cartSnapshot[i] != orderSnapshot[i])
                    return false;
            }

            return true;
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        private sealed record OrderItemSnapshot(int CourseId, decimal UnitPrice, Currency Currency);
    }
}
