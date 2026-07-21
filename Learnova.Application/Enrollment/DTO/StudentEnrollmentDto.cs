using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learnova.Domain.Enums;

namespace Learnova.Application.Enrollment.DTO
{
    public class StudentEnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = default!;
        public string? CourseDescription { get; set; }
        public DateTime EnrolledAt { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public EnrollmentStatus Status { get; set; }
    }
}
