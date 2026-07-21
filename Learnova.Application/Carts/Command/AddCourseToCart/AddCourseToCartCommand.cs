using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Carts.Command.AddCourseToCart
{
    public class AddCourseToCartCommand(int courseId):IRequest
    {
        public int CourseId { get; set; } = courseId;
    }
}
