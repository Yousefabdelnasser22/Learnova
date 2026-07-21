using Learnova.Application.Caching;
using Microsoft.AspNetCore.OutputCaching;

namespace Learnova.Api.Services
{
    public sealed class OutputCacheInvalidationService(IOutputCacheStore outputCacheStore) : ICacheInvalidationService
    {
        public ValueTask EvictCoursesAsync(CancellationToken cancellationToken = default)
            => outputCacheStore.EvictByTagAsync("courses", cancellationToken);

        public ValueTask EvictCategoriesAsync(CancellationToken cancellationToken = default)
            => outputCacheStore.EvictByTagAsync("categories", cancellationToken);
    }
}
