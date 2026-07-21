using FluentValidation;

namespace Learnova.Application.SubCategories.Query.GetSubCategoryById
{
    public class GetSubCategoryByIdQueryValidator : AbstractValidator<GetSubCategoryByIdQuery>
    {
        public GetSubCategoryByIdQueryValidator()
        {
            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0).WithMessage("SubCategoryId must be more than 0");
        }
    }
}
