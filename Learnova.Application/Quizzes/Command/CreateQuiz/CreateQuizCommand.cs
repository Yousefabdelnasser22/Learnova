using Learnova.Application.Quizzes.DTO;
using Learnova.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Command.CreateQuiz
{
    public class CreateQuizCommand:IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public ICollection<QuizQuestionDTO> Questions { get; set; } = new List<QuizQuestionDTO>();
        
        
    }
}
