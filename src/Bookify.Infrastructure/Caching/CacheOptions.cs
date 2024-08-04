using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions Create(TimeSpan? expiration)
    {
        return expiration is not null ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration } : DefaultExpiration;
    }
    public static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };
}