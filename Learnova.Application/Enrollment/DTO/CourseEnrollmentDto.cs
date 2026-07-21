using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.DTO
{
    public class CourseEnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public string StudentId { get; set; } = default!;
        public string? StudentEmail { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
