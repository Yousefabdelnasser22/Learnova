using Learnova.Application.SubCategories.DTO;
using MediatR;

namespace Learnova.Application.SubCategories.Query.GetSubCategoryById
{
    public class GetSubCategoryByIdQuery : IRequest<SubCategoryDTO>
    {
        public int SubCategoryId { get; set; }
    }
}
