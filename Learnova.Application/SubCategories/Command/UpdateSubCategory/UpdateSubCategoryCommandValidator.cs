using FluentValidation;

namespace Learnova.Application.SubCategories.Command.UpdateSubCategory
{
    public class UpdateSubCategoryCommandValidator : AbstractValidator<UpdateSubCategoryCommand>
    {
        public UpdateSubCategoryCommandValidator()
        {
            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0).WithMessage("SubCategoryId must be more than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        }
    }
}
