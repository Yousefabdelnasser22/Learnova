using Learnova.Application.Orders.Command.CheckoutCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IMediator mediator) : ControllerBase
    {
        [HttpPost("checkout-cart")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Checkout the current cart",
            Description = "Creates a pending order and payment transaction from the authenticated user's current cart.")]
        public async Task<IActionResult> CheckoutCart()
        {
            var orderId = await mediator.Send(new CheckoutCartCommand());
            return Created(string.Empty, new { orderId });
        }
    }
}
