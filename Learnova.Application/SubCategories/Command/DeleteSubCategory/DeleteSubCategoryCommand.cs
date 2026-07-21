using MediatR;

namespace Learnova.Application.SubCategories.Command.DeleteSubCategory
{
    public class DeleteSubCategoryCommand : IRequest
    {
        public int SubCategoryId { get; set; }
    }
}
