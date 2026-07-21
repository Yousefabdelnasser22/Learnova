using MediatR;

namespace Learnova.Application.Quizzes.Command.DeleteQuestion
{
    public class DeleteQuestionCommand(int id) : IRequest
    {
        public int Id { get; set; } = id;
    }
}
