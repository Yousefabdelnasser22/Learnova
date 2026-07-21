using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Carts.Command.AddCourseToCart
{
    public class AddCourseToCartCommandValidator:AbstractValidator<AddCourseToCartCommand>
    {
        public AddCourseToCartCommandValidator()
        {
            RuleFor(x => x.CourseId)
              .GreaterThan(0).WithMessage("CourseId must be more than 0");
        }
    }
}
