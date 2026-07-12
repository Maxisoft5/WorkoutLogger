using Microsoft.Extensions.Logging.Abstractions;
using Modules.Common.Infrastructure.RateLimiting;

namespace Modules.Users.Tests.Auth;

[TestFixture]
public class LoginRateLimiterTests
{
    private const string Email = "User@Example.com";
    private const string Ip = "10.0.0.1";

    private static LoginRateLimiter CreateLimiter(
        ILoginAttemptStore? primary = null,
        int maxAttempts = 3,
        int windowMinutes = 15)
    {
        var fallback = new InMemoryLoginAttemptStore();
        return new LoginRateLimiter(
            primary ?? fallback,
            fallback,
            new LoginRateLimitSettings { MaxFailedAttempts = maxAttempts, WindowMinutes = windowMinutes },
            NullLogger<LoginRateLimiter>.Instance);
    }

    [Test]
    public async Task Check_WithoutFailures_IsNotBlocked()
    {
        var limiter = CreateLimiter();

        var result = await limiter.CheckAsync(Email, Ip);

        Assert.That(result.IsBlocked, Is.False);
    }

    [Test]
    public async Task Check_BelowThreshold_IsNotBlocked()
    {
        var limiter = CreateLimiter(maxAttempts: 3);

        await limiter.RecordFailureAsync(Email, Ip);
        await limiter.RecordFailureAsync(Email, Ip);

        var result = await limiter.CheckAsync(Email, Ip);
        Assert.That(result.IsBlocked, Is.False);
    }

    [Test]
    public async Task Check_AtThreshold_BlocksWithRetryAfter()
    {
        var limiter = CreateLimiter(maxAttempts: 3);

        for (var i = 0; i < 3; i++)
            await limiter.RecordFailureAsync(Email, Ip);

        var result = await limiter.CheckAsync(Email, Ip);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsBlocked, Is.True);
            Assert.That(result.RetryAfter, Is.Not.Null);
            Assert.That(result.RetryAfter!.Value, Is.GreaterThan(TimeSpan.Zero));
        });
    }

    [Test]
    public async Task Reset_AfterSuccessfulLogin_Unblocks()
    {
        var limiter = CreateLimiter(maxAttempts: 3);

        for (var i = 0; i < 3; i++)
            await limiter.RecordFailureAsync(Email, Ip);
        await limiter.ResetAsync(Email, Ip);

        var result = await limiter.CheckAsync(Email, Ip);
        Assert.That(result.IsBlocked, Is.False);
    }

    [Test]
    public async Task Key_IsCaseInsensitiveOnEmail()
    {
        var limiter = CreateLimiter(maxAttempts: 2);

        await limiter.RecordFailureAsync("user@example.com", Ip);
        await limiter.RecordFailureAsync("USER@EXAMPLE.COM", Ip);

        var result = await limiter.CheckAsync("User@Example.com", Ip);
        Assert.That(result.IsBlocked, Is.True);
    }

    [Test]
    public async Task DifferentIp_HasSeparateCounter()
    {
        var limiter = CreateLimiter(maxAttempts: 2);

        await limiter.RecordFailureAsync(Email, "10.0.0.1");
        await limiter.RecordFailureAsync(Email, "10.0.0.1");

        var otherIp = await limiter.CheckAsync(Email, "10.0.0.2");
        Assert.That(otherIp.IsBlocked, Is.False, "блокировка не должна распространяться на другие IP");
    }

    [Test]
    public async Task PrimaryStoreFailure_FallsBackToMemory()
    {
        var limiter = CreateLimiter(primary: new ThrowingStore(), maxAttempts: 2);

        await limiter.RecordFailureAsync(Email, Ip);
        await limiter.RecordFailureAsync(Email, Ip);

        var result = await limiter.CheckAsync(Email, Ip);
        Assert.That(result.IsBlocked, Is.True, "fallback-хранилище должно продолжать считать попытки");
    }

    [Test]
    public async Task InMemoryStore_ExpiredWindow_RestartsCounting()
    {
        var store = new InMemoryLoginAttemptStore();

        await store.IncrementAsync("key", TimeSpan.FromMilliseconds(-1)); // окно уже истекло
        var state = await store.GetAsync("key");

        Assert.That(state, Is.Null);
    }

    private sealed class ThrowingStore : ILoginAttemptStore
    {
        public Task<long> IncrementAsync(string key, TimeSpan window, CancellationToken ct = default) =>
            throw new InvalidOperationException("Redis is down");

        public Task<(long Count, TimeSpan? Ttl)?> GetAsync(string key, CancellationToken ct = default) =>
            throw new InvalidOperationException("Redis is down");

        public Task ResetAsync(string key, CancellationToken ct = default) =>
            throw new InvalidOperationException("Redis is down");
    }
}
