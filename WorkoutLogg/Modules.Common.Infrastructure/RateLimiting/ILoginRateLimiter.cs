namespace Modules.Common.Infrastructure.RateLimiting
{
    /// <summary>Результат проверки лимита попыток входа.</summary>
    public record LoginRateLimitResult(bool IsBlocked, TimeSpan? RetryAfter);

    /// <summary>
    /// Ограничитель неудачных попыток входа (защита от перебора паролей).
    /// Счётчики хранятся в Redis (fixed window по паре IP+email);
    /// при недоступности Redis лимитер переключается на in-memory-хранилище,
    /// чтобы auth-flow продолжал работать (та же философия, что и HybridCacheService).
    /// </summary>
    public interface ILoginRateLimiter
    {
        /// <summary>Проверить, не заблокированы ли попытки входа для пары IP+email.</summary>
        Task<LoginRateLimitResult> CheckAsync(string email, string? ipAddress, CancellationToken ct = default);

        /// <summary>Зафиксировать неудачную попытку входа.</summary>
        Task RecordFailureAsync(string email, string? ipAddress, CancellationToken ct = default);

        /// <summary>Сбросить счётчик после успешного входа.</summary>
        Task ResetAsync(string email, string? ipAddress, CancellationToken ct = default);
    }

    public class LoginRateLimitSettings
    {
        /// <summary>Максимум неудачных попыток в окне, после которого вход блокируется.</summary>
        public int MaxFailedAttempts { get; set; } = 5;

        /// <summary>Длина окна (и время блокировки), минуты.</summary>
        public int WindowMinutes { get; set; } = 15;
    }
}
