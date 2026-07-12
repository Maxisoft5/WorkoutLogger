using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Common.Infrastructure.Caching;
using Modules.Common.Infrastructure.RateLimiting;

namespace Modules.Common.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
   
        public static IServiceCollection AddHybridCache(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "WorkoutLogger:";
            });
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, HybridCacheService>();
            return services;
        }

        /// <summary>
        /// Rate limiting на Login через Redis: счётчик неудачных попыток по паре IP+email
        /// (атомарный INCR + EXPIRE). При недоступном Redis — fallback на in-memory.
        /// </summary>
        public static IServiceCollection AddLoginRateLimiter(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = configuration.GetSection("LoginRateLimit").Get<LoginRateLimitSettings>()
                ?? new LoginRateLimitSettings();
            services.AddSingleton(settings);

            services.AddSingleton<InMemoryLoginAttemptStore>();

            // Без строки подключения к Redis лимитер работает целиком на in-memory-хранилище.
            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                services.AddSingleton<ILoginAttemptStore>(sp =>
                    sp.GetRequiredService<InMemoryLoginAttemptStore>());
            }
            else
            {
                services.AddSingleton<ILoginAttemptStore>(sp => new RedisLoginAttemptStore(
                    redisConnectionString,
                    sp.GetRequiredService<ILogger<RedisLoginAttemptStore>>()));
            }

            services.AddSingleton<ILoginRateLimiter, LoginRateLimiter>();
            return services;
        }
    }
}
