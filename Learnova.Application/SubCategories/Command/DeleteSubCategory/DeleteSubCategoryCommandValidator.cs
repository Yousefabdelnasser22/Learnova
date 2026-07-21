using FluentValidation;

namespace Learnova.Application.SubCategories.Command.DeleteSubCategory
{
    public class DeleteSubCategoryCommandValidator : AbstractValidator<DeleteSubCategoryCommand>
    {
        public DeleteSubCategoryCommandValidator()
        {
            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0).WithMessage("SubCategoryId must be more than 0");
        }
    }
}
