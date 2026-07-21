using Learnova.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Interfaces
{
    public interface IQuizQuestionRepository : IGenericRepository<QuizQuestion>
    {
        void DeleteRange(IEnumerable<QuizQuestion> questions);
    }
}
