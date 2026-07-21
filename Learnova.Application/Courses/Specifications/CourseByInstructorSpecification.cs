using Learnova.Domain.Entites;
using Learnova.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Courses.Specifications
{
    public class CourseByInstructorSpecification:BaseSpecification<Course>
    {
        public CourseByInstructorSpecification(string instructorId, int courseId):base(c=>c.Id==courseId&&c.InstructorId==instructorId)
        {
            
        }
    }
}
