using FluentValidation;

namespace Learnova.Application.Categories.Query.GetAllCategories
{
    public class GetAllCategoriesQueryValidator : AbstractValidator<GetAllCategoriesQuery>
    {
        public GetAllCategoriesQueryValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(100);
        }
    }
}
