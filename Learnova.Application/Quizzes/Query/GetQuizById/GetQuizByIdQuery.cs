using Learnova.Application.Quizzes.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Query.GetQuizById
{
    public class GetQuizByIdQuery(int id) : IRequest<GetQuizByIdDTO>
    {
        public int Id { get; } = id;
    }
}
