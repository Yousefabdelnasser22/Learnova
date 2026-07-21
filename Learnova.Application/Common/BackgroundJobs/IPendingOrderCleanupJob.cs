namespace Learnova.Application.Common.BackgroundJobs
{
    public interface IPendingOrderCleanupJob
    {
        Task CleanupAsync();
    }
}
