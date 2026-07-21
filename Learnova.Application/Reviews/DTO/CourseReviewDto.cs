using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.DTO
{
    public class CourseReviewDto
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StudentName { get; set; } = null!;
    }
}
