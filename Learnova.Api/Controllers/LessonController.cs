using Learnova.Application.Lesson.Command.CompleteLesson;
using Learnova.Application.Lesson.Command.CreateLesson;
using Learnova.Application.Lesson.Command.DeleteLesson;
using Learnova.Application.Lesson.Command.UpdateLesson;
using Learnova.Application.Lesson.Query.GetAllLesson;
using Learnova.Application.Lesson.Query.GetLessonById;
using Learnova.Domain.Constant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [Route("api/courses/{courseId}/modules/{moduleId}/lessons")]
    [ApiController]
    [Authorize]
    public class LessonController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Create a lesson",
            Description = "Creates a lesson inside the specified module of an instructor-owned course.")]
        public async Task<IActionResult> Add(int courseId, int moduleId, [FromBody] CreateLessonCommand command)
        {
            command.CourseId = courseId;
            command.ModuleId = moduleId;

            await mediator.Send(command);

            return Created();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Update a lesson",
            Description = "Updates the specified lesson inside an instructor-owned course module.")]
        public async Task<IActionResult> Update(int id, int courseId, int moduleId, [FromBody] UpdateLessonCommand command)
        {
            command.CourseId = courseId;
            command.ModuleId = moduleId;
            command.Id = id;

            await mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Delete a lesson",
            Description = "Deletes the specified lesson from an instructor-owned course module.")]
        public async Task<IActionResult> Delete(int id, int courseId, int moduleId)
        {
            await mediator.Send(new DeleteLessonCommand(id)
            {
                CourseId = courseId,
                ModuleId = moduleId
            });

            return NoContent();
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get module lessons",
            Description = "Retrieves a paginated list of lessons in the specified course module.")]
        public async Task<IActionResult> GetAll(
            int courseId,
            int moduleId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var lessons = await mediator.Send(new GetAllLessonQuery
            {
                CourseId = courseId,
                ModuleId = moduleId,
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(lessons);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get a lesson by ID",
            Description = "Retrieves the specified lesson from a course module.")]
        public async Task<IActionResult> GetById(int id, int courseId, int moduleId)
        {
            var lessons = await mediator.Send(new GetLessonByIdQuery
            {
                Id = id,
                CourseId = courseId,
                ModuleId = moduleId
            });

            return Ok(lessons);
        }

        [HttpPost("{lessonId}/complete")]
        [SwaggerOperation(
            Summary = "Mark a lesson as complete",
            Description = "Records completion of the specified lesson for the authenticated student.")]
        public async Task<IActionResult> CompleteLesson(int courseId, int moduleId, int lessonId)
        {
            var result = await mediator.Send(
                new CompleteLessonCommand
                {
                    LessonId = lessonId,
                    CourseId = courseId,
                    ModuleId = moduleId
                }
            );

            return Ok(result);
        }
    }
}
