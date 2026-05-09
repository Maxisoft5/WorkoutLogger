using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Modules.Common.Infrastructure.Configurations;
using Modules.Common.Infrastructure.Messaging;
using Modules.Users.Domain.Authentication;
using Modules.Users.Domain.Users;
using Modules.Users.Infrastructure.Authorization;
using Modules.Users.Infrastructure.Database;
using System.Text;

namespace WorkoutLogger.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaMessaging(
            this IServiceCollection services,
            IConfiguration configuration)
                {
            var settings = configuration.GetSection("Kafka").Get<KafkaSettings>() ?? new KafkaSettings();
            services.AddSingleton(settings);
            services.AddSingleton<IEventPublisher, KafkaEventPublisher>();

            services.AddHealthChecks()
                .AddKafka(new ProducerConfig { BootstrapServers = settings.BootstrapServers },
                          topic: "health-check",
                          name: "kafka",
                          failureStatus: HealthStatus.Degraded);

            return services;
        }
        public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = string.Empty;
            })
              .AddRoles<Role>()
              .AddEntityFrameworkStores<UsersDbContext>()
              .AddSignInManager()
              .AddDefaultTokenProviders();
            services.RemoveAll<IUserValidator<User>>();
            services.AddScoped<IUserValidator<User>, NoDuplicateUserNameValidator<User>>();
            services.AddJwtAuthentication(configuration);
            services.AddClaimsAuthorization();
            services.AddScoped<IAuthService, AuthService>();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AuthConfiguration>()
                .Bind(configuration.GetSection(nameof(AuthConfiguration)));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["AuthConfiguration:Issuer"],
                ValidAudience = configuration["AuthConfiguration:Audience"],
#pragma warning disable S6781
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthConfiguration:Key"] ?? throw new InvalidOperationException("JWT key is not configured")))
#pragma warning restore S6781
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });

            return services;
        }

        private static IServiceCollection AddClaimsAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }
    }
}
