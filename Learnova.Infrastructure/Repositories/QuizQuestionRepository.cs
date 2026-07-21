using Learnova.Domain.Entites;
using Learnova.Domain.Interfaces;
using Learnova.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Repositories
{
    public class QuizQuestionRepository : GenericRepository<QuizQuestion>, IQuizQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuizQuestionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void DeleteRange(IEnumerable<QuizQuestion> questions)
        {
            _context.QuizQuestions.RemoveRange(questions);
        }
    }
}
