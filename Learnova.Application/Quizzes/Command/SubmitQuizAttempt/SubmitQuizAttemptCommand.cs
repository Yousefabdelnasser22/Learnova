using Learnova.Application.Quizzes.DTO;
using Learnova.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Command.SubmitQuizAttempt
{
    public class SubmitQuizAttemptCommand:IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int QuizId { get; set; }
        
        public ICollection<QuizAnswerDTO> Answers { get; set; } = new List<QuizAnswerDTO>();
    }
}
