using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace TodoApp.Application.Behaviours;

/// <summary>
/// Marker interface for commands that should invalidate cache entries on success.
/// </summary>
public interface ICacheInvalidatingCommand
{
    IEnumerable<string> CacheKeysToInvalidate { get; }
}

/// <summary>
/// MediatR pipeline behavior that invalidates cache entries after a command succeeds.
/// </summary>
public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

    public CacheInvalidationBehavior(IDistributedCache cache, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (request is ICacheInvalidatingCommand invalidatingCommand)
        {
            foreach (var key in invalidatingCommand.CacheKeysToInvalidate)
            {
                try
                {
                    await _cache.RemoveAsync(key, cancellationToken);
                    _logger.LogDebug("Cache invalidated for key: {CacheKey}", key);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to invalidate cache for key: {CacheKey}.", key);
                }
            }
        }

        return response;
    }
}
