using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Quizzes.Specifications
{
    public class PassedQuizAttemptSpecification : BaseSpecification<QuizAttempt>
    {
        public PassedQuizAttemptSpecification(string studentId, int quizId)
            : base(qa => qa.StudentId == studentId
                        && qa.QuizId == quizId
                        && qa.IsPass)
        {
        }
    }
}
