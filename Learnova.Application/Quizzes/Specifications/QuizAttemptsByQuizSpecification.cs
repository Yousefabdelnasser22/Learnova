using Learnova.Domain.Specifications;

namespace Learnova.Application.Quizzes.Specifications
{
    using QuizAttemptEntity = Learnova.Domain.Entities.QuizAttempt;

    public class QuizAttemptsByQuizSpecification : BaseSpecification<QuizAttemptEntity>
    {
        public QuizAttemptsByQuizSpecification(int quizId, int pageNumber, int pageSize, string? search)
            : base(q =>
                q.QuizId == quizId &&
                (string.IsNullOrWhiteSpace(search) ||
                 q.Quiz.Title.Contains(search) ||
                 (q.Student.Email != null && q.Student.Email.Contains(search)) ||
                 (q.Student.UserName != null && q.Student.UserName.Contains(search))))
        {
            AddInclude(q => q.Quiz);
            AddInclude(q => q.Student);
            AddOrderByDescending(q => q.SubmittedAt);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }
    }
}
