using Learnova.Application.Common.Queries;
using Learnova.Application.SubCategories.DTO;
using MediatR;

namespace Learnova.Application.SubCategories.Query.GetSubCategoriesByCategoryId
{
    public class GetSubCategoriesByCategoryIdQuery(int categoryId) : SearchQuery, IRequest<ICollection<SubCategoryDTO>>
    {
        public int CategoryId { get; set; } = categoryId;
    }
}
