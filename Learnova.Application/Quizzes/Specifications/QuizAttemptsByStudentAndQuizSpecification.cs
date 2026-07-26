using Learnova.Domain.Specifications;

namespace Learnova.Application.Quizzes.Specifications
{
    using QuizAttemptEntity = Learnova.Domain.Entities.QuizAttempt;

    public class QuizAttemptsByStudentAndQuizSpecification : BaseSpecification<QuizAttemptEntity>
    {
        public QuizAttemptsByStudentAndQuizSpecification(int quizId, string studentId, int pageNumber, int pageSize, string? search)
            : base(q =>
                q.QuizId == quizId &&
                q.StudentId == studentId &&
                (string.IsNullOrWhiteSpace(search) ||
                 q.Quiz.Title.Contains(search)))
        {
            AddInclude(q => q.Quiz);
            AddOrderByDescending(q => q.SubmittedAt);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }
    }
}
