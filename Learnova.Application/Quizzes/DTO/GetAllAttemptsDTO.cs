using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.DTO
{
    public class GetAllAttemptsDTO
    {
        public int AttemptId { get; set; }
        public string StudentEmail { get; set; } = null!;

        public string QuizTitle { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }

        public bool IsPass { get; set; }
    }
}
