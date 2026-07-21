using Learnova.Application.Reviews.Command.CreateReview;
using Learnova.Application.Reviews.Command.DeleteReview;
using Learnova.Application.Reviews.Command.UpdateReview;
using Learnova.Application.Reviews.Query.GetCourseReviews;
using Learnova.Application.Reviews.Query.GetMyReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController(IMediator mediator) : ControllerBase
    {
        [Route("/api/courses/{courseId}/reviews")]
        [HttpPost]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Create a course review",
            Description = "Creates a review for the specified course on behalf of the authenticated student.")]
        public async Task<IActionResult> Add(CreateReviewCommand command, int courseId)
        {
            command.CourseId = courseId;

            var review = await mediator.Send(command);

            return Ok(review);
        }

        [Route("/api/courses/{courseId}/reviews")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get course reviews",
            Description = "Retrieves a paginated list of reviews for the specified course.")]
        public async Task<IActionResult> GetCourseReviews(
            int courseId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var reviews = await mediator.Send(new GetCourseReviewsQuery
            {
                CourseId = courseId,
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            if (reviews == null)
            {
                return NotFound();
            }

            return Ok(reviews);
        }

        [Route("/api/courses/{courseId}/reviews/my")]
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get my course review",
            Description = "Retrieves the authenticated user's review for the specified course.")]
        public async Task<IActionResult> GetMyReview(int courseId)
        {
            var review = await mediator.Send(new GetMyReviewQuery() { CourseId = courseId });

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

        [HttpPut("{reviewId}")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Update a course review",
            Description = "Updates a review owned by the authenticated user.")]
        public async Task<IActionResult> Update(UpdateReviewCommand command, int reviewId)
        {
            command.ReviewId = reviewId;

            var review = await mediator.Send(command);

            return Ok(review);
        }

        [HttpDelete("{reviewId}")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Delete a course review",
            Description = "Deletes a review owned by the authenticated user.")]
        public async Task<IActionResult> Delete(int reviewId)
        {
            await mediator.Send(new DeleteReviewCommand
            {
                ReviewId = reviewId
            });

            return NoContent();
        }
    }
}
