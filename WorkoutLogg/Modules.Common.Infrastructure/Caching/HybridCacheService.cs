using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Modules.Common.Infrastructure.Caching
{
    public class HybridCacheService(
     IDistributedCache redis,
     IMemoryCache memory,
     ILogger<HybridCacheService> logger) : ICacheService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Состояние circuit breaker
        private static int _failureCount;
        private static DateTime _circuitOpenedAt = DateTime.MinValue;
        private const int FailureThreshold = 3;          // сколько падений подряд размыкает цепь
        private static readonly TimeSpan CircuitOpenDuration = TimeSpan.FromSeconds(30);

        private static bool IsRedisAvailable
        {
            get
            {
                // Если цепь разомкнута и время прошло — пробуем снова
                if (_failureCount >= FailureThreshold)
                {
                    if (DateTime.UtcNow - _circuitOpenedAt > CircuitOpenDuration)
                    {
                        Interlocked.Exchange(ref _failureCount, 0);
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }

        private void RecordFailure()
        {
            var count = Interlocked.Increment(ref _failureCount);
            if (count == FailureThreshold)
            {
                _circuitOpenedAt = DateTime.UtcNow;
                logger.LogWarning("Redis circuit breaker OPENED for {Duration}s", CircuitOpenDuration.TotalSeconds);
            }
        }

        private void RecordSuccess()
        {
            if (_failureCount > 0)
                Interlocked.Exchange(ref _failureCount, 0);
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
        {
            // Сначала пробуем Redis
            if (IsRedisAvailable)
            {
                try
                {
                    var bytes = await redis.GetAsync(key, ct);
                    RecordSuccess();
                    if (bytes is not null && bytes.Length > 0)
                        return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
                }
                catch (Exception ex)
                {
                    RecordFailure();
                    logger.LogWarning(ex, "Redis GET failed for key {Key}, falling back to memory", key);
                    return memory.TryGetValue(key, out T? cachedMem) ? cachedMem : null;
                }
                finally
                {

                }
            }

            // Fallback в memory
            return memory.TryGetValue(key, out T? cached) ? cached : null;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default) where T : class
        {
            var ttl = expiry ?? TimeSpan.FromMinutes(5);

            // Всегда пишем в memory — это страховка и быстрый L1-кэш
            memory.Set(key, value, ttl);

            if (IsRedisAvailable)
            {
                try
                {
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = ttl
                    };
                    var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
                    await redis.SetAsync(key, bytes, options, ct);
                    RecordSuccess();
                }
                catch (Exception ex)
                {
                    RecordFailure();
                    logger.LogWarning(ex, "Redis SET failed for key {Key}, value cached only in memory", key);
                }
            }
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            memory.Remove(key);

            if (IsRedisAvailable)
            {
                try
                {
                    await redis.RemoveAsync(key, ct);
                    RecordSuccess();
                }
                catch (Exception ex)
                {
                    RecordFailure();
                    logger.LogWarning(ex, "Redis REMOVE failed for key {Key}", key);
                }
            }
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            TimeSpan? expiry = null,
            CancellationToken ct = default) where T : class
        {
            var cached = await GetAsync<T>(key, ct);
            if (cached is not null)
                return cached;

            var value = await factory(ct);
            if (value is not null)
                await SetAsync(key, value, expiry, ct);

            return value;
        }
    }
}
