using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Query.IsStudentEnrolled
{
    public class IsStudentEnrolledQuery:IRequest<bool>
    {
        public int CourseId { get; set; }
    }
}
