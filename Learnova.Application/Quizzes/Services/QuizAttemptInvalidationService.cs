using Learnova.Domain.Interfaces;

namespace Learnova.Application.Quizzes.Services
{
    public class QuizAttemptInvalidationService(IUnitOfWork unitOfWork) : IQuizAttemptInvalidationService
    {
        public async Task InvalidateAttemptsForQuizAsync(
            int quizId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var attempts = await unitOfWork.quizAttempt.GetAllWithCondition(
                attempt => attempt.QuizId == quizId,
                attempt => attempt.Answers);

            foreach (var attempt in attempts)
            {
                attempt.IsDeleted = true;

                foreach (var answer in attempt.Answers)
                {
                    answer.IsDeleted = true;
                }
            }
        }
    }
}
