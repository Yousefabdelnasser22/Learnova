using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class QuizAttempt:BaseEntity
    {
        public int QuizId { get; set; }
        public string StudentId { get; set; } = null!;
        public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>(); 
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public int Score { get; set; }    
        public int TotalQuestions { get; set; }

        public bool IsPass { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;
    }
}
