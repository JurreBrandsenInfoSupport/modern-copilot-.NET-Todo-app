using MediatR;

namespace TodoApp.Application.Behaviours;

/// <summary>
/// Marker interface for queries that should be cached.
/// </summary>
public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan CacheDuration { get; }
}
