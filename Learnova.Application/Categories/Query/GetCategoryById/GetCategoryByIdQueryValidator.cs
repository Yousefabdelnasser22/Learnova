using FluentValidation;

namespace Learnova.Application.Categories.Query.GetCategoryById
{
    public sealed class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0");
        }
    }
}
