using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Modules.Command.UpdateModule
{
    public class UpdateModuleCommandValidator:AbstractValidator<UpdateModuleCommand>    
    {
        public UpdateModuleCommandValidator()
        {
            RuleFor(x => x.CourseId)
               .GreaterThan(0).WithMessage("CourseId must be more than 0");

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Position)
                .GreaterThan(0);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

        }

    }
}
