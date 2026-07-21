using Learnova.Application.Enrollment.DTO;
using Learnova.Application.Enrollment.Query.GetStudentEnrollments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnrollmentsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get my enrollments",
            Description = "Retrieves a paginated list of courses in which the authenticated student is enrolled.")]
        public async Task<ActionResult<IEnumerable<StudentEnrollmentDto>>> GetMyEnrollments(
           [FromQuery] string? search,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10
           )
        {
            var result = await mediator.Send(new GetStudentEnrollmentsQuery
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            return Ok(result);
        }
    }
}
