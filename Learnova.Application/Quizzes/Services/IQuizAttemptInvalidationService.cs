namespace Learnova.Application.Quizzes.Services
{
    public interface IQuizAttemptInvalidationService
    {
        Task InvalidateAttemptsForQuizAsync(
            int quizId,
            CancellationToken cancellationToken = default);
    }
}
