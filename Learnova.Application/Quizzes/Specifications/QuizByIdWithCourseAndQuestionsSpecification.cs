using Learnova.Domain.Specifications;

namespace Learnova.Application.Quizzes.Specifications
{
    using QuizEntity = Learnova.Domain.Entites.Quiz;

    public class QuizByIdWithCourseAndQuestionsSpecification : BaseSpecification<QuizEntity>
    {
        public QuizByIdWithCourseAndQuestionsSpecification(int quizId)
            : base(quiz => quiz.Id == quizId)
        {
            AddInclude(quiz => quiz.Course);
            AddInclude(quiz => quiz.Questions);
        }
    }
}
