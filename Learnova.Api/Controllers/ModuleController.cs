using Learnova.Application.Modules.Command.CreateModule;
using Learnova.Application.Modules.Command.DeleteModule;
using Learnova.Application.Modules.Command.ReorderModule;
using Learnova.Application.Modules.Command.UpdateModule;
using Learnova.Application.Modules.Query.GetAllModule;
using Learnova.Application.Modules.Query.GetModuleById;
using Learnova.Domain.Constant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [Route("api/courses/{courseId}/modules")]
    [ApiController]
    [Authorize]
    public class ModuleController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Create a course module",
            Description = "Creates a module in the specified instructor-owned course.")]
        public async Task<IActionResult> Add(int courseId, [FromBody] CreateModuleCommand command)
        {
            command.CourseId = courseId;

            await mediator.Send(command);

            return Created();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get a module by ID",
            Description = "Retrieves the specified module from a course.")]
        public async Task<IActionResult> GetById(int courseId, int id)
        {
            var module = await mediator.Send(new GetModuleByIdQuery
            {
                Id = id,
                CourseId = courseId
            });

            return Ok(module);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Update a course module",
            Description = "Updates the specified module in an instructor-owned course.")]
        public async Task<IActionResult> Update(int courseId, int id, [FromBody] UpdateModuleCommand command)
        {
            command.Id = id;
            command.CourseId = courseId;

            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Delete a course module",
            Description = "Deletes the specified module from an instructor-owned course.")]
        public async Task<IActionResult> Delete(int courseId, int id)
        {
            await mediator.Send(new DeleteModuleCommand(id)
            {
                CourseId = courseId
            });

            return NoContent();
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get course modules",
            Description = "Retrieves a paginated list of modules in the specified course.")]
        public async Task<IActionResult> GetById(
            int courseId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var module = await mediator.Send(new GetAllModuleQuery
            {
                CourseId = courseId,
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(module);
        }

        [HttpPatch("{id}/reorder")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Reorder a course module",
            Description = "Changes the display order of the specified module in an instructor-owned course.")]
        public async Task<IActionResult> Reorder(int courseId, int id, [FromBody] ReorderModuleCommand command)
        {
            command.CourseId = courseId;
            command.ModuleId = id;

            await mediator.Send(command);

            return NoContent();
        }
    }
}
