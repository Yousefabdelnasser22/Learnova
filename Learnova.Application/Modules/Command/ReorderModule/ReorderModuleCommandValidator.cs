using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Modules.Command.ReorderModule
{
    public class ReorderModuleCommandValidator : AbstractValidator<ReorderModuleCommand>
    {
        public ReorderModuleCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0);

            RuleFor(x => x.ModuleId)
                .GreaterThan(0);

            RuleFor(x => x.NewPosition)
                .GreaterThan(0);
        }
    }
}
