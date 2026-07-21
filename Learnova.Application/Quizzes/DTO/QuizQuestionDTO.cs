using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.DTO
{
    public class QuizQuestionDTO
    {
        public string Question { get; set; } = null!;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
    }
}
