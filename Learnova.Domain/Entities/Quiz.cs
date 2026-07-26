using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class Quiz:BaseEntity
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>(); 
        public Course Course { get; set; } = null!;
        public ICollection<QuizAttempt> Attempts { get; set; } = [];
    }
}
