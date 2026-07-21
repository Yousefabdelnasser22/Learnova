using Learnova.Application.Courses.Command.ArchiveCourse;
using Learnova.Application.Courses.Command.CreateCourse;
using Learnova.Application.Courses.Command.DeleteCourse;
using Learnova.Application.Courses.Command.PublishCourse;
using Learnova.Application.Courses.Command.SubmitCourseForReview;
using Learnova.Application.Courses.Command.UpdateCourse;
using Learnova.Application.Courses.Query.GetAllCourses;
using Learnova.Application.Courses.Query.GetCourseForManagement;
using Learnova.Application.Courses.Query.GetCourseById;
using Learnova.Application.Courses.Query.SearchCourses;
using Learnova.Application.Enrollment.Command.EnrollStudent;
using Learnova.Application.Enrollment.Command.UnenrollStudent;
using Learnova.Application.Enrollment.DTO;
using Learnova.Application.Enrollment.Query.GetCourseEnrollments;
using Learnova.Application.Enrollment.Query.IsStudentEnrolled;
using Learnova.Domain.Constant;
using Learnova.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    
    [Route("api/courses")]
    [ApiController]
    public class CourseController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [OutputCache(PolicyName = "CourseList")]
        [SwaggerOperation(
            Summary = "Get all courses",
            Description = "Retrieves a paginated list of published courses with optional search, filtering, and sorting.")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? categoryId,
            [FromQuery] int? subcategoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] CourseLevel? level,
            [FromQuery] string? sort,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var courses = await mediator.Send(new GetAllCoursesQuery
            {
                Search = search,
                CategoryId = categoryId,
                SubCategoryId = subcategoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Level = level,
                Sort = sort,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            if (courses == null)
            {
                return NotFound();
            }
            return Ok(courses);
        }

        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Search courses",
            Description = "Returns a limited set of courses matching the supplied search term.")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] int limit = 10)
        {
            var courses = await mediator.Send(new SearchCoursesQuery(searchTerm, limit));

            return Ok(courses);
        }

        [HttpGet("{id}")]
        [OutputCache(PolicyName = "CourseDetails")]
        [SwaggerOperation(
            Summary = "Get a course by ID",
            Description = "Retrieves the public details of the specified course.")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await mediator.Send(new GetCourseByIdQuery(id));

            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [HttpGet("{id}/manage")]
        [Authorize(Roles = $"{UserRole.Instructor},{UserRole.Admin}")]
        [SwaggerOperation(
            Summary = "Get a course for management",
            Description = "Retrieves management details for a course available to its instructor or an administrator.")]
        public async Task<IActionResult> GetForManagement(int id)
        {
            var course = await mediator.Send(new GetCourseForManagementQuery(id));

            return Ok(course);
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Create a course",
            Description = "Creates a new course owned by the authenticated instructor.")]
        public async Task<IActionResult> Add([FromBody] CreateCourseCommand command)
        {
            await mediator.Send(command);

            return Created();
        }


        [HttpPut("{id}")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Update a course",
            Description = "Updates the specified course owned by the authenticated instructor.")]
        public async Task<IActionResult> Update([FromBody] UpdateCourseCommand command, int id)
        {
            command.Id = id;
            await mediator.Send(command);
            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Delete a course",
            Description = "Deletes the specified course owned by the authenticated instructor.")]
        public async Task<IActionResult> Delete(int id)
        {
            await mediator.Send(new DeleteCourseCommand(id));
            return NoContent();
        }

        [HttpPost("{id}/submit-for-review")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Submit a course for review",
            Description = "Submits the specified instructor-owned course for administrative review.")]
        public async Task<IActionResult> SubmitForReview(int id)
        {
            await mediator.Send(new SubmitCourseForReviewCommand
            {
                Id = id
            });

            return NoContent();
        }

        [HttpPost("{id}/publish")]
        [Authorize(Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Publish a course",
            Description = "Publishes a reviewed course and makes it available to students. Administrator access is required.")]
        public async Task<IActionResult> Publish(int id)
        {
            await mediator.Send(new PublishCourseCommand
            {
                Id = id
            });

            return NoContent();
        }

        [HttpPost("{id}/archive")]
        [Authorize(Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Archive a course",
            Description = "Archives the specified course. Administrator access is required.")]
        public async Task<IActionResult> Archive(int id)
        {
            await mediator.Send(new ArchiveCourseCommand
            {
                Id = id
            });

            return NoContent();
        }

        [HttpPost("{courseId}/enroll")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Enroll in a course",
            Description = "Enrolls the authenticated student in the specified eligible course.")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            await mediator.Send(new EnrollStudentCommand
            {
                CourseId = courseId
            });
            return Created();
        }

        [HttpDelete("{courseId}/unenroll")]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Unenroll from a course",
            Description = "Removes the authenticated student's enrollment from the specified course when allowed.")]
        public async Task<IActionResult> UnEnroll(int courseId)
        {
            await mediator.Send(new UnenrollStudentCommand
            {
                CourseId = courseId
            });
            return NoContent();
        }

        [HttpGet("{courseId}/enrollments")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Get course enrollments",
            Description = "Retrieves a paginated list of students enrolled in an instructor-owned course.")]
        public async Task<ActionResult<IEnumerable<CourseEnrollmentDto>>> GetCourseEnrollments(
            int courseId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await mediator.Send(new GetCourseEnrollmentsQuery
            {
                CourseId = courseId,
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            return Ok(result);
        }

        [HttpGet("{courseId}/is-enrolled")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Check course enrollment",
            Description = "Checks whether the authenticated student is enrolled in the specified course.")]
        public async Task<IActionResult> IsStudentEnrolled(int courseId)
        {
            var result = await mediator.Send(new IsStudentEnrolledQuery() { CourseId = courseId });
            return Ok(new { isEnrolled = result });
        }
    }
}
