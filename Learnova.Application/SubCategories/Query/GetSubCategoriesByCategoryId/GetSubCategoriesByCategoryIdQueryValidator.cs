using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.SubCategories.Query.GetSubCategoriesByCategoryId
{
    public class GetSubCategoriesByCategoryIdQueryValidator:AbstractValidator<GetSubCategoriesByCategoryIdQuery>
    {
        public GetSubCategoriesByCategoryIdQueryValidator()
        {
          RuleFor(x => x.CategoryId)
         .GreaterThan(0)
         .WithMessage("CategoryId must be greater than 0.");

            RuleFor(x => x.Search)
                .MaximumLength(100);
        }
    }
}
