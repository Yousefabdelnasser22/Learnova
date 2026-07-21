using Learnova.Application.Favorites.Commands.AddCourseToFavorites;
using Learnova.Application.Favorites.Commands.RemoveCourseFromFavorites;
using Learnova.Application.Favorites.Query.GetMyFavorites;
using Learnova.Application.Favorites.Query.IsCourseFavorite;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get my favorite courses",
            Description = "Retrieves all courses in the authenticated user's favorites list.")]
        public async Task<IActionResult> GetMyFavorites()
        {
            var favorites = await mediator.Send(new GetMyFavoritesQuery());

            return Ok(favorites);
        }

        [HttpGet("status/{courseId}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Check favorite status",
            Description = "Checks whether the specified course is in the authenticated user's favorites list.")]
        public async Task<IActionResult> IsCourseFavorite(int courseId)
        {
            var isFavorite = await mediator.Send(new IsCourseFavoriteQuery(courseId));

            return Ok(isFavorite);
        }

        [HttpPost("{courseId}")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Add a course to favorites",
            Description = "Adds the specified course to the authenticated user's favorites list.")]
        public async Task<IActionResult> AddCourseToFavorites(int courseId)
        {
            await mediator.Send(new AddCourseToFavoritesCommand(courseId));

            return Created();
        }

        [HttpDelete("{courseId}")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Remove a course from favorites",
            Description = "Removes the specified course from the authenticated user's favorites list.")]
        public async Task<IActionResult> RemoveCourseFromFavorites(int courseId)
        {
            await mediator.Send(new RemoveCourseFromFavoritesCommand(courseId));

            return NoContent();
        }
    }
}
