using Learnova.Domain.Specifications;

namespace Learnova.Application.Quizzes.Specifications
{
    using QuizEntity = Learnova.Domain.Entities.Quiz;

    public class CourseQuizzesSpecification : BaseSpecification<QuizEntity>
    {
        public CourseQuizzesSpecification(int courseId, int pageNumber, int pageSize, string? search)
            : base(q =>
                q.CourseId == courseId &&
                (string.IsNullOrWhiteSpace(search) ||
                 q.Title.Contains(search) ||
                 q.Course.Title.Contains(search)))
        {
            AddInclude(q => q.Course);
            AddInclude(q => q.Questions);
            AddOrderBy(q => q.Id);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }
    }
}
