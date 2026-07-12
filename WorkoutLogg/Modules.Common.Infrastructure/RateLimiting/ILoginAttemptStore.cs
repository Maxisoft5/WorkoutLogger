using System.Collections.Concurrent;

namespace Modules.Common.Infrastructure.RateLimiting
{
    /// <summary>
    /// Хранилище счётчиков неудачных попыток входа.
    /// Отделено от лимитера, чтобы логику блокировки можно было
    /// unit-тестировать без Redis (на InMemoryLoginAttemptStore).
    /// </summary>
    public interface ILoginAttemptStore
    {
        /// <summary>
        /// Инкремент счётчика. TTL выставляется при первом инкременте (fixed window).
        /// Возвращает новое значение счётчика.
        /// </summary>
        Task<long> IncrementAsync(string key, TimeSpan window, CancellationToken ct = default);

        /// <summary>Текущее значение счётчика и остаток TTL (null — счётчика нет).</summary>
        Task<(long Count, TimeSpan? Ttl)?> GetAsync(string key, CancellationToken ct = default);

        /// <summary>Удалить счётчик.</summary>
        Task ResetAsync(string key, CancellationToken ct = default);
    }

    /// <summary>In-memory-хранилище: используется в тестах и как fallback при недоступности Redis.</summary>
    public class InMemoryLoginAttemptStore : ILoginAttemptStore
    {
        private sealed class Entry
        {
            public long Count;
            public DateTime ExpiresAtUtc;
        }

        private readonly ConcurrentDictionary<string, Entry> _entries = new();

        public Task<long> IncrementAsync(string key, TimeSpan window, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var entry = _entries.AddOrUpdate(
                key,
                _ => new Entry { Count = 1, ExpiresAtUtc = now + window },
                (_, existing) =>
                {
                    lock (existing)
                    {
                        if (existing.ExpiresAtUtc <= now)
                        {
                            existing.Count = 1;
                            existing.ExpiresAtUtc = now + window;
                        }
                        else
                        {
                            existing.Count++;
                        }
                        return existing;
                    }
                });

            lock (entry)
            {
                return Task.FromResult(entry.Count);
            }
        }

        public Task<(long Count, TimeSpan? Ttl)?> GetAsync(string key, CancellationToken ct = default)
        {
            if (!_entries.TryGetValue(key, out var entry))
                return Task.FromResult<(long, TimeSpan?)?>(null);

            lock (entry)
            {
                var ttl = entry.ExpiresAtUtc - DateTime.UtcNow;
                if (ttl <= TimeSpan.Zero)
                {
                    _entries.TryRemove(key, out _);
                    return Task.FromResult<(long, TimeSpan?)?>(null);
                }
                return Task.FromResult<(long, TimeSpan?)?>((entry.Count, ttl));
            }
        }

        public Task ResetAsync(string key, CancellationToken ct = default)
        {
            _entries.TryRemove(key, out _);
            return Task.CompletedTask;
        }
    }
}
