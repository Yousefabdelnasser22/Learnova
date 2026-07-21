using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.SubCategories.Command.CreateSubCategory
{
    public class CreateSubCategoryCommandValidator:AbstractValidator<CreateSubCategoryCommand>
    {
        public CreateSubCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("CategoryId must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("SubCategory name is required.")
                .MaximumLength(100)
                .WithMessage("SubCategory name must not exceed 100 characters.");
        }
    }
}
