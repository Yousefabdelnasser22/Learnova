using Learnova.Application.Categories.Command.CreateCategory;
using Learnova.Application.Categories.Command.DeleteCategory;
using Learnova.Application.Categories.Command.UpdateCategory;
using Learnova.Application.Categories.Query.GetAllCategories;
using Learnova.Application.Categories.Query.GetCategoryById;
using Learnova.Application.SubCategories.Command.CreateSubCategory;
using Learnova.Application.SubCategories.Query.GetSubCategoriesByCategoryId;
using Learnova.Domain.Constant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Learnova.Api.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [OutputCache(PolicyName = "Categories")]
        [SwaggerOperation(
            Summary = "Get all categories",
            Description = "Retrieves categories, optionally filtered by a search term.")]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var categories = await mediator.Send(new GetAllCategoriesQuery
            {
                Search = search
            });
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [OutputCache(PolicyName = "Categories")]
        [SwaggerOperation(
            Summary = "Get a category by ID",
            Description = "Retrieves the category identified by the supplied category ID.")]
        public async Task<IActionResult> GetById(int categoryId)
        {
            var category = await mediator.Send(new GetCategoryByIdQuery() { CategoryId = categoryId });
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Create a category",
            Description = "Creates a new course category. Administrator access is required.")]
        public async Task<IActionResult> AddCategory(CreateCategoryCommand command)
        {
            await mediator.Send(command);
            return Created();
        }

        [HttpPut("{categoryId}")]
        [Authorize(Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Update a category",
            Description = "Updates the specified course category. Administrator access is required.")]
        public async Task<IActionResult> Update(UpdateCategoryCommand command, int categoryId)
        {
            command.CategoryId = categoryId;
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Delete a category",
            Description = "Deletes the specified course category. Administrator access is required.")]
        public async Task<IActionResult> Delete(int categoryId)
        {
            await mediator.Send(new DeleteCategoryCommand
            {
                CategoryId = categoryId
            });

            return NoContent();
        }
        [Route("{categoryId}/subcategories")]
        [HttpPost]
        [Authorize(Roles = UserRole.Admin)]
        [SwaggerOperation(
            Summary = "Create a subcategory",
            Description = "Creates a subcategory under the specified category. Administrator access is required.")]
        public async Task<IActionResult> AddSubCategory( int categoryId,CreateSubCategoryCommand command)
        {
            command.CategoryId = categoryId;    
            await mediator.Send(command);
            return Created();
        }

        [Route("{categoryId}/subcategories")]
        [HttpGet]
        [OutputCache(PolicyName = "Categories")]
        [SwaggerOperation(
            Summary = "Get a category's subcategories",
            Description = "Retrieves subcategories belonging to the specified category, optionally filtered by a search term.")]
        public async Task<IActionResult> GetSubCategoriesByCategoryId(int categoryId, [FromQuery] string? search)
        {
            
            var subcategory = await mediator.Send(new GetSubCategoriesByCategoryIdQuery(categoryId)
            {
                Search = search
            });
            return Ok(subcategory);
        }

    }
}
