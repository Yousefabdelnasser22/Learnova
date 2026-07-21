using MediatR;

namespace Learnova.Application.Quizzes.Command.AddQuestion
{
    public class AddQuestionCommand : IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int QuizId { get; set; }
        public string Question { get; set; } = null!;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
    }
}
