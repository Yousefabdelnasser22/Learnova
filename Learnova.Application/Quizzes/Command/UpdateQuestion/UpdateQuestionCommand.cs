using MediatR;

namespace Learnova.Application.Quizzes.Command.UpdateQuestion
{
    public class UpdateQuestionCommand : IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        public string Question { get; set; } = null!;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
    }
}
