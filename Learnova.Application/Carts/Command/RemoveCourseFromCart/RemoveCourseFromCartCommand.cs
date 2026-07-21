using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Carts.Command.RemoveCourseFromCart
{
    public class RemoveCourseFromCartCommand(int courseId) : IRequest
    {
        public int CourseId { get; set; }=courseId;
    }
}
