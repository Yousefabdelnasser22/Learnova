using Learnova.Application.Carts.Command.AddCourseToCart;
using Learnova.Application.Carts.Command.ClearCart;
using Learnova.Application.Carts.Command.RemoveCourseFromCart;
using Learnova.Application.Carts.Query.GetMyCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Learnova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(IMediator mediator) : ControllerBase
    {
        [HttpGet("my-cart")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get the current user's cart",
            Description = "Retrieves the authenticated user's shopping cart and its course items.")]
        public async Task<IActionResult> GetMyCart()
        {
            var cart = await mediator.Send(new GetMyCartQuery());

            return Ok(cart);
        }

        [HttpPost]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Add a course to the cart",
            Description = "Adds the specified course to the authenticated user's shopping cart.")]
        public async Task<IActionResult> AddItemToCart(int courseId)
        {
            await mediator.Send(new AddCourseToCartCommand(courseId));

            return Created();
        }

        [HttpDelete("remove-course/{courseId}")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Remove a course from the cart",
            Description = "Removes the specified course from the authenticated user's shopping cart.")]
        public async Task<IActionResult> RemoveCourse(int courseId)
        {
            await mediator.Send(new RemoveCourseFromCartCommand(courseId));

            return NoContent();
        }

        [HttpDelete("clear")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Clear the cart",
            Description = "Removes all courses from the authenticated user's shopping cart.")]
        public async Task<IActionResult> Clear()
        {
            await mediator.Send(new ClearCartCommand());

            return NoContent();
        }
    }
}
