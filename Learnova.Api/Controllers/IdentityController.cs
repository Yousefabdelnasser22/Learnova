using Learnova.Application.User.Command.AssignUserRole;
using Learnova.Application.User.Command.UnassignUserRole;
using Learnova.Application.User.Command.UpdateUserDetail;
using Learnova.Domain.Constant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [Route("api/identity")]
    [ApiController]
    public class IdentityController(IMediator mediator) : ControllerBase
    {
        [HttpPatch("user")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Update the current user's details",
            Description = "Updates profile details for the authenticated user.")]
        public async Task<IActionResult> UpdateUserDetail(UpdateUserDetailCommand updateUser)
        {
           await mediator.Send(updateUser);
           return NoContent();
        }


        [HttpPost("AssignRole")]
        [Authorize (Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Assign a role to a user",
            Description = "Assigns a system role to a user. Administrator access is required.")]
        public async Task<IActionResult> AssignUserRole(AssignUserRoleCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("UnassignRole")]
        [Authorize(Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Remove a role from a user",
            Description = "Removes a system role from a user. Administrator access is required.")]
        public async Task<IActionResult> UnassignUserRole(UnassignUserRoleCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }

    }
}
