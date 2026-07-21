using Learnova.Application.SubCategories.Command.DeleteSubCategory;
using Learnova.Application.SubCategories.Command.UpdateSubCategory;
using Learnova.Application.SubCategories.Query.GetSubCategoryById;
using Learnova.Domain.Constant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Swashbuckle.AspNetCore.Annotations;

namespace Learnova.Api.Controllers
{
    [Route("api/subcategories")]
    [ApiController]
    public class SubCategoriesController(IMediator mediator) : ControllerBase
    {
        [HttpGet("{subCategoryId}")]
        [OutputCache(PolicyName = "Categories")]
        [SwaggerOperation(
            Summary = "Get a subcategory by ID",
            Description = "Retrieves the subcategory identified by the supplied subcategory ID.")]
        public async Task<IActionResult> GetById(int subCategoryId)
        {
            var subCategory = await mediator.Send(new GetSubCategoryByIdQuery
            {
                SubCategoryId = subCategoryId
            });

            return Ok(subCategory);
        }

        [HttpPut("{subCategoryId}")]
        [Authorize(Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Update a subcategory",
            Description = "Updates the specified course subcategory. Administrator access is required.")]
        public async Task<IActionResult> Update(int subCategoryId, [FromBody] UpdateSubCategoryCommand command)
        {
            command.SubCategoryId = subCategoryId;
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{subCategoryId}")]
        [Authorize(Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Delete a subcategory",
            Description = "Deletes the specified course subcategory. Administrator access is required.")]
        public async Task<IActionResult> Delete(int subCategoryId)
        {
            var command = new DeleteSubCategoryCommand
            {
                SubCategoryId = subCategoryId
            };

            await mediator.Send(command);
            return NoContent();
        }
    }
}
