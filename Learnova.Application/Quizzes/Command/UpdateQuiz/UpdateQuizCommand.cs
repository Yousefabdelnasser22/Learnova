using MediatR;
using Learnova.Application.Quizzes.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Command.UpdateQuiz
{
    public class UpdateQuizCommand : IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public ICollection<QuizQuestionDTO> Questions { get; set; } = new List<QuizQuestionDTO>();
    }
}
