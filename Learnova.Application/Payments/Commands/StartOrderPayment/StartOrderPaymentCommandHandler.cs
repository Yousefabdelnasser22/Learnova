using Learnova.Application.Exceptions;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Orders.Specifications;
using Learnova.Application.Payments.DTO;
using Learnova.Application.Payments.Gateway;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Payments.Commands.StartOrderPayment
{
    public class StartOrderPaymentCommandHandler(
     IUnitOfWork unitOfWork,
     IUserContext userContext,
     IPaymentGatewayService paymentGatewayService)
     : IRequestHandler<StartOrderPaymentCommand, StartOrderPaymentResultDto>
    {
        public async Task<StartOrderPaymentResultDto> Handle(
            StartOrderPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var spec = new OrderWithItemsAndPaymentsSpecification(request.OrderId);

            var order = await unitOfWork
                .Repository<Order>()
                .GetEntityWithSpecAsync(spec);

            if (order is null)
                throw new NotFoundException("Order not found.");

            if (order.StudentId != user.Id)
                throw new ForbiddenAccessException("You are not allowed to start payment for this order.");

            if (order.Status != OrderStatus.Pending)
                throw new BadRequestException("Only pending orders can start payment.");

            if (order.Items is null || order.Items.Count == 0)
                throw new BadRequestException("Order has no items.");

            if (order.PaymentTransactions is null || order.PaymentTransactions.Count == 0)
                throw new BadRequestException("Order has no payment transactions.");

            await EnsureOrderCanStartPaymentAsync(order, user.Id, cancellationToken);

            var payment = order.PaymentTransactions
                .FirstOrDefault(x => x.Status == PaymentStatus.Pending);

            if (payment is null)
            {
                if (order.PaymentTransactions.Any(x => x.Status == PaymentStatus.Success))
                    throw new BadRequestException("Checkout session is already completed and awaiting confirmation.");

                payment = new PaymentTransaction
                {
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    Currency = order.Currency,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await unitOfWork.Repository<PaymentTransaction>().Add(payment);
                await unitOfWork.CompleteAsync(cancellationToken);
            }

            if (string.Equals(payment.Provider, "Stripe", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(payment.ProviderTransactionId))
            {
                var existingSession = await paymentGatewayService.GetCheckoutSessionAsync(
                    payment.ProviderTransactionId,
                    cancellationToken);

                if (existingSession?.IsActive == true &&
                    !string.IsNullOrWhiteSpace(existingSession.CheckoutUrl))
                {
                    return new StartOrderPaymentResultDto
                    {
                        CheckoutUrl = existingSession.CheckoutUrl
                    };
                }

                if (existingSession?.IsCompleted == true)
                    throw new BadRequestException("Checkout session is already completed and awaiting confirmation.");

                if (existingSession is not null)
                {
                    await paymentGatewayService.ExpireCheckoutSessionAsync(
                        payment.ProviderTransactionId,
                        cancellationToken);
                }
            }

            var gatewayRequest = new CreateGatewayPaymentRequest
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                PaymentTransactionId = payment.Id,
                Amount = order.TotalAmount,
                Currency = order.Currency,
                Items = order.Items.Select(x => new GatewayPaymentItem
                {
                    Name = x.CourseTitleSnapshot,
                    UnitPrice = x.UnitPrice,
                    Quantity = 1
                }).ToList()
            };

            var gatewayResult = await paymentGatewayService
                .CreateCheckoutSessionAsync(gatewayRequest, cancellationToken);

            payment.Provider = gatewayResult.Provider;
            payment.ProviderTransactionId = gatewayResult.ProviderTransactionId;

            await unitOfWork.CompleteAsync(cancellationToken);

            return new StartOrderPaymentResultDto
            {
                CheckoutUrl = gatewayResult.CheckoutUrl
            };
        }

        private async Task EnsureOrderCanStartPaymentAsync(
            Order order,
            string studentId,
            CancellationToken cancellationToken)
        {
            foreach (var item in order.Items)
            {
                var course = await unitOfWork.course.GetById(item.CourseId);

                if (course is null || course.Status != CourseStatus.Published)
                    throw new BadRequestException($"Course '{item.CourseTitleSnapshot}' is not available for purchase.");

                if (course.Price != item.UnitPrice || course.Currency != item.Currency)
                    throw new BadRequestException($"Course '{item.CourseTitleSnapshot}' price has changed. Please checkout again.");

                var activeEnrollmentSpec = new ActiveEnrollmentByStudentAndCourseSpecification(
                    studentId,
                    item.CourseId);

                var existingEnrollment = await unitOfWork
                    .Repository<Learnova.Domain.Entites.Enrollment>()
                    .GetEntityWithSpecAsync(activeEnrollmentSpec);

                if (existingEnrollment is not null)
                    throw new BadRequestException($"You are already enrolled in course '{item.CourseTitleSnapshot}'.");
            }
        }
    }
}
