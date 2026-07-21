using Microsoft.AspNetCore.OutputCaching;

namespace Learnova.Api.Services
{
    public sealed class ResilientRedisOutputCacheStore : IOutputCacheStore, IDisposable
    {
        private const int CacheRetryDelayMilliseconds = 30_000;
        private readonly IOutputCacheStore _redisStore;
        private readonly ILogger<ResilientRedisOutputCacheStore> _logger;
        private long _retryCacheAfterMilliseconds;

        public ResilientRedisOutputCacheStore(
            IOutputCacheStore redisStore,
            ILogger<ResilientRedisOutputCacheStore> logger)
        {
            _redisStore = redisStore;
            _logger = logger;
        }

        public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
        {
            if (IsCacheTemporarilyUnavailable())
            {
                return null;
            }

            try
            {
                return await _redisStore.GetAsync(key, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                MarkCacheAsTemporarilyUnavailable();
                _logger.LogWarning(
                    exception,
                    "Output cache read failed for key {CacheKey}. The request will continue without cache.",
                    key);

                return null;
            }
        }

        public async ValueTask SetAsync(
            string key,
            byte[] value,
            string[]? tags,
            TimeSpan validFor,
            CancellationToken cancellationToken)
        {
            if (IsCacheTemporarilyUnavailable())
            {
                return;
            }

            try
            {
                await _redisStore.SetAsync(key, value, tags, validFor, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                MarkCacheAsTemporarilyUnavailable();
                _logger.LogWarning(
                    exception,
                    "Output cache write failed for key {CacheKey}. The response will be returned without caching.",
                    key);
            }
        }

        public async ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
        {
            if (IsCacheTemporarilyUnavailable())
            {
                return;
            }

            try
            {
                await _redisStore.EvictByTagAsync(tag, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                MarkCacheAsTemporarilyUnavailable();
                _logger.LogWarning(
                    exception,
                    "Output cache eviction failed for tag {CacheTag}. The completed operation will not be reported as failed.",
                    tag);
            }
        }

        private bool IsCacheTemporarilyUnavailable()
            => Environment.TickCount64 < Interlocked.Read(ref _retryCacheAfterMilliseconds);

        private void MarkCacheAsTemporarilyUnavailable()
            => Interlocked.Exchange(
                ref _retryCacheAfterMilliseconds,
                Environment.TickCount64 + CacheRetryDelayMilliseconds);

        public void Dispose()
        {
            if (_redisStore is IDisposable disposableStore)
            {
                disposableStore.Dispose();
            }
        }
    }

    public static class ResilientOutputCacheStoreServiceCollectionExtensions
    {
        public static IServiceCollection AddResilientOutputCacheStore(this IServiceCollection services)
        {
            var storeDescriptor = services.LastOrDefault(
                descriptor => descriptor.ServiceType == typeof(IOutputCacheStore))
                ?? throw new InvalidOperationException("An output cache store must be registered first.");

            services.Remove(storeDescriptor);
            services.Add(new ServiceDescriptor(
                typeof(IOutputCacheStore),
                serviceProvider => new ResilientRedisOutputCacheStore(
                    CreateStore(serviceProvider, storeDescriptor),
                    serviceProvider.GetRequiredService<ILogger<ResilientRedisOutputCacheStore>>()),
                storeDescriptor.Lifetime));

            return services;
        }

        private static IOutputCacheStore CreateStore(
            IServiceProvider serviceProvider,
            ServiceDescriptor storeDescriptor)
        {
            if (storeDescriptor.ImplementationInstance is IOutputCacheStore storeInstance)
            {
                return storeInstance;
            }

            if (storeDescriptor.ImplementationFactory is not null)
            {
                return (IOutputCacheStore)storeDescriptor.ImplementationFactory(serviceProvider);
            }

            if (storeDescriptor.ImplementationType is not null)
            {
                return (IOutputCacheStore)ActivatorUtilities.CreateInstance(
                    serviceProvider,
                    storeDescriptor.ImplementationType);
            }

            throw new InvalidOperationException("The configured output cache store cannot be created.");
        }
    }
}
