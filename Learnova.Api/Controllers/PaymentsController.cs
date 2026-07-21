using Learnova.Application.Payments.Commands.StartOrderPayment;
using Learnova.Application.Payments.DTO;
using Learnova.Application.Payments.Query.GetOrderPaymentStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("orders/{orderId:int}/start")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Start an order payment",
            Description = "Creates or resumes a Stripe Checkout session for the authenticated user's pending order.")]
        public async Task<ActionResult<StartOrderPaymentResultDto>> StartOrderPayment(
            int orderId)
        {
            var result = await mediator.Send(new StartOrderPaymentCommand(orderId));

            return Ok(result);
        }

        [HttpGet("orders/{orderId:int}/status")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get order payment status",
            Description = "Retrieves the latest payment and order status for an order owned by the authenticated user.")]
        public async Task<ActionResult<OrderPaymentStatusDto>> GetOrderPaymentStatus(
            int orderId)
        {
            var result = await mediator.Send(new GetOrderPaymentStatusQuery(orderId));

            return Ok(result);
        }
    }
}
