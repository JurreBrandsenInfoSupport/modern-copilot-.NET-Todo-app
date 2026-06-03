using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace TodoApp.Application.Behaviours;

/// <summary>
/// MediatR pipeline behavior that caches responses for queries implementing ICacheableQuery.
/// </summary>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next();
        }

        var cacheKey = cacheableQuery.CacheKey;

        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (cachedData != null)
            {
                _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                var result = JsonSerializer.Deserialize<TResponse>(cachedData);
                if (result != null)
                    return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read from cache for key: {CacheKey}. Proceeding without cache.", cacheKey);
        }

        var response = await next();

        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheableQuery.CacheDuration
            };

            var serialized = JsonSerializer.Serialize(response);
            await _cache.SetStringAsync(cacheKey, serialized, options, cancellationToken);
            _logger.LogDebug("Cache set for key: {CacheKey} with TTL: {CacheDuration}", cacheKey, cacheableQuery.CacheDuration);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write to cache for key: {CacheKey}.", cacheKey);
        }

        return response;
    }
}
