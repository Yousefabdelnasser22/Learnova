using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.DTO
{
    public class GetAllQuizzesDTO
    {
        public int QuizId { get; set; }
        public string CourseName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int QuestionsCount { get; set; }
    }
}
