using Learnova.Application.Categories.DTO;
using Learnova.Application.Common.Queries;
using MediatR;

namespace Learnova.Application.Categories.Query.GetAllCategories
{
    public class GetAllCategoriesQuery : SearchQuery, IRequest<IEnumerable<CategoryDto>>
    {
    }
}
