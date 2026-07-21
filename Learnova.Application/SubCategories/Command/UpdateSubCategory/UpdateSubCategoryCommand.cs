using MediatR;

namespace Learnova.Application.SubCategories.Command.UpdateSubCategory
{
    public class UpdateSubCategoryCommand : IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int SubCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
