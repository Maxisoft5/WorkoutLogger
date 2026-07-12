using StackExchange.Redis;

namespace Modules.Common.Infrastructure.RateLimiting
{
    /// <summary>
    /// Redis-хранилище счётчиков попыток входа: атомарный INCR + EXPIRE
    /// (fixed window). Подключение ленивое, AbortOnConnectFail=false —
    /// недоступный Redis не валит приложение на старте.
    /// Ошибки Redis пробрасываются наверх — их логирует LoginRateLimiter при fallback.
    /// </summary>
    public class RedisLoginAttemptStore(string connectionString)
        : ILoginAttemptStore, IDisposable
    {
        private readonly Lazy<Task<ConnectionMultiplexer>> _connection = new(() =>
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 2000;
            return ConnectionMultiplexer.ConnectAsync(options);
        });

        private const string KeyPrefix = "WorkoutLogger:login-attempts:";

        public async Task<long> IncrementAsync(string key, TimeSpan window, CancellationToken ct = default)
        {
            var db = (await _connection.Value).GetDatabase();
            var redisKey = KeyPrefix + key;

            var count = await db.StringIncrementAsync(redisKey);
            if (count == 1)
                await db.KeyExpireAsync(redisKey, window);

            return count;
        }

        public async Task<(long Count, TimeSpan? Ttl)?> GetAsync(string key, CancellationToken ct = default)
        {
            var db = (await _connection.Value).GetDatabase();
            var redisKey = KeyPrefix + key;

            var value = await db.StringGetAsync(redisKey);
            if (value.IsNullOrEmpty)
                return null;

            var ttl = await db.KeyTimeToLiveAsync(redisKey);
            return ((long)value, ttl);
        }

        public async Task ResetAsync(string key, CancellationToken ct = default)
        {
            var db = (await _connection.Value).GetDatabase();
            await db.KeyDeleteAsync(KeyPrefix + key);
        }

        public void Dispose()
        {
            if (_connection.IsValueCreated && _connection.Value.IsCompletedSuccessfully)
                _connection.Value.Result.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
