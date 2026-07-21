using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Courses.Command.DeleteCourse
{
    public class DeleteCourseCommand(int id):IRequest
    {
        public int Id { get; set; } = id;
    }
}
