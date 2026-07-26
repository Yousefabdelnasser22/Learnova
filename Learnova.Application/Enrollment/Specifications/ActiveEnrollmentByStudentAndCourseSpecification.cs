using Learnova.Domain.Enums;
using Learnova.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Learnova.Application.Enrollment.Specifications
{
    using EnrollmentEntity = Learnova.Domain.Entities.Enrollment;
    public class ActiveEnrollmentByStudentAndCourseSpecification
    : BaseSpecification<EnrollmentEntity>
    {
        public ActiveEnrollmentByStudentAndCourseSpecification(string studentId, int courseId)
            : base(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId &&
                (e.Status == EnrollmentStatus.Active ||
                 e.Status == EnrollmentStatus.Completed))
        {
        }
    }
}
