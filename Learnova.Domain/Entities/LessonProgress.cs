using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class LessonProgress : BaseEntity
    {
        public string StudentId { get; set; } = default!;
        public int LessonId { get; set; }

        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        public ApplicationUser Student { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
    }
}
