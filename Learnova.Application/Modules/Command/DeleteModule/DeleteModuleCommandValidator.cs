using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Modules.Command.DeleteModule
{
    public class DeleteModuleCommandValidator:AbstractValidator<DeleteModuleCommand>
    {
        public DeleteModuleCommandValidator()
        {

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be more than 0");

            RuleFor(x => x.CourseId)
               .GreaterThan(0).WithMessage("Id must be more than 0");
        }
    }
}
