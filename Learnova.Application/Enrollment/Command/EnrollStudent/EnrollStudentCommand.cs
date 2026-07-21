using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Command.EnrollStudent
{
    public class EnrollStudentCommand :IRequest
    {
        public int CourseId { get; set; }


       
    }
}
