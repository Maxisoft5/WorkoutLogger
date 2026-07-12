using Microsoft.Extensions.Logging;

namespace Modules.Common.Infrastructure.RateLimiting
{
    /// <summary>
    /// Лимитер неудачных попыток входа поверх двух хранилищ:
    /// основное — Redis, при его недоступности прозрачный fallback на in-memory.
    /// Ключ — пара IP+email, чтобы злоумышленник с одного адреса не мог
    /// заблокировать чужой аккаунт для остальных.
    /// </summary>
    public class LoginRateLimiter(
        ILoginAttemptStore primaryStore,
        InMemoryLoginAttemptStore fallbackStore,
        LoginRateLimitSettings settings,
        ILogger<LoginRateLimiter> logger) : ILoginRateLimiter
    {
        private TimeSpan Window => TimeSpan.FromMinutes(settings.WindowMinutes);

        public async Task<LoginRateLimitResult> CheckAsync(string email, string? ipAddress, CancellationToken ct = default)
        {
            var key = BuildKey(email, ipAddress);
            var state = await ExecuteAsync(
                s => s.GetAsync(key, ct),
                "GET");

            if (state is null)
                return new LoginRateLimitResult(false, null);

            var (count, ttl) = state.Value;
            return count >= settings.MaxFailedAttempts
                ? new LoginRateLimitResult(true, ttl ?? Window)
                : new LoginRateLimitResult(false, null);
        }

        public async Task RecordFailureAsync(string email, string? ipAddress, CancellationToken ct = default)
        {
            var key = BuildKey(email, ipAddress);
            await ExecuteAsync(s => s.IncrementAsync(key, Window, ct), "INCR");
        }

        public async Task ResetAsync(string email, string? ipAddress, CancellationToken ct = default)
        {
            var key = BuildKey(email, ipAddress);
            await ExecuteAsync<object?>(
                async s => { await s.ResetAsync(key, ct); return null; },
                "RESET");
        }

        private static string BuildKey(string email, string? ipAddress) =>
            $"{ipAddress ?? "unknown"}:{email.Trim().ToLowerInvariant()}";

        private async Task<T> ExecuteAsync<T>(Func<ILoginAttemptStore, Task<T>> action, string operation)
        {
            try
            {
                return await action(primaryStore);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Login rate limiter: primary store failed on {Operation}, falling back to memory", operation);
                return await action(fallbackStore);
            }
        }
    }
}
