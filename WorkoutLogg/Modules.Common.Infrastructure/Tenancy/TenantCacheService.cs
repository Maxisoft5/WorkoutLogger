using Modules.Common.Infrastructure.Caching;
using Modules.Common.Infrastructure.RateLimiting;

namespace Modules.Common.Infrastructure.Tenancy;

public sealed class TenantCacheService(HybridCacheService inner, TenantContext tenant) : ICacheService
{
    private string Key(string key) => $"tenant:{tenant.Current.Id}:{key}";
    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class => inner.GetAsync<T>(Key(key), ct);
    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default) where T : class => inner.SetAsync(Key(key), value, expiry, ct);
    public Task RemoveAsync(string key, CancellationToken ct = default) => inner.RemoveAsync(Key(key), ct);
    public Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiry = null, CancellationToken ct = default) where T : class => inner.GetOrCreateAsync(Key(key), factory, expiry, ct);
}

public sealed class TenantLoginRateLimiter(LoginRateLimiter inner, TenantContext tenant) : ILoginRateLimiter
{
    private string Key(string email) => $"{tenant.Current.Id}:{email}";
    public Task<LoginRateLimitResult> CheckAsync(string email, string? ipAddress, CancellationToken ct = default) => inner.CheckAsync(Key(email), ipAddress, ct);
    public Task RecordFailureAsync(string email, string? ipAddress, CancellationToken ct = default) => inner.RecordFailureAsync(Key(email), ipAddress, ct);
    public Task ResetAsync(string email, string? ipAddress, CancellationToken ct = default) => inner.ResetAsync(Key(email), ipAddress, ct);
}
