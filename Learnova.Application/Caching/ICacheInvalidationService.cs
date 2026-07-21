namespace Learnova.Application.Caching
{
    public interface ICacheInvalidationService
    {
        ValueTask EvictCoursesAsync(CancellationToken cancellationToken = default);

        ValueTask EvictCategoriesAsync(CancellationToken cancellationToken = default);
    }
}
