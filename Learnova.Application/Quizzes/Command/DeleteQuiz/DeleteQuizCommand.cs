using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Command.DeleteQuiz
{
    public class DeleteQuizCommand(int id) : IRequest
    {
        public int Id { get; set; } = id;
    }
}
