using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

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
    }
}
