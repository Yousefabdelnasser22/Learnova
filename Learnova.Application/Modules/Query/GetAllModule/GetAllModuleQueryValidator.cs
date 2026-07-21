using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Modules.Query.GetAllModule
{
    public class GetAllModuleQueryValidator:AbstractValidator<GetAllModuleQuery>    
    {
        public GetAllModuleQueryValidator()
        {

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Id must be more than 0");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(100);
        }
    }
}
