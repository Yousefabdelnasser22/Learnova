using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.DTO
{
    public class GetQuizByIdQuestionDTO
    {
        public int QuestionId { get; set; }
        public string Question { get; set; } = null!;
        public List<string> Options { get; set; } = new List<string>();
    }
}
