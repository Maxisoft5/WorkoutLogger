using WorkoutLogger.WebApi.Services;
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
using Modules.Common.Infrastructure.Email;
using Modules.Common.Infrastructure.Messaging;
using Modules.Subscriptions.Infrastructure.Database;
using Modules.Subscriptions.Infrastructure.Services;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Services;
using Modules.Users.Domain.Authentication;
using Modules.Users.Domain.Users;
using Modules.Users.Infrastructure.Authorization;
using Modules.Users.Infrastructure.Database;
using Modules.Users.Infrastructure.Outbox;
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

            if (!settings.Enabled)
            {
                services.AddSingleton<IEventPublisher, NullEventPublisher>();
                return services;
            }

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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<Modules.Users.Infrastructure.Workouts.IWorkoutService,
                Modules.Users.Infrastructure.Workouts.WorkoutService>();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            var mailtrapSettings = configuration.GetSection("Mailtrap").Get<MailtrapSettings>() ?? new MailtrapSettings();
            services.AddSingleton(mailtrapSettings);
            services.AddHttpClient<IEmailSender, MailtrapHttpEmailSender>();
            services.AddHostedService<OutboxProcessorService>();

            return services;
        }

        public static IServiceCollection AddAiCoachService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = configuration.GetSection("AiCoach").Get<AiSettings>() ?? new AiSettings();
            services.AddSingleton(settings);
            services.AddHttpClient<AiChatService>();
            return services;
        }

        public static IServiceCollection AddSubscriptionsModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<SubscriptionsDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            var settings = configuration.GetSection("SubscriptionSettings").Get<SubscriptionSettings>()
                ?? new SubscriptionSettings();
            services.AddSingleton(settings);

            services.AddHttpClient<YooKassaProvider>();
            services.AddHttpClient<StripeProvider>();
            services.AddTransient<IPaymentProvider, YooKassaProvider>();
            services.AddTransient<IPaymentProvider, StripeProvider>();
            services.AddScoped<SubscriptionService>();

            return services;
        }

        public static IServiceCollection AddTrainersModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<TrainersDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ITrainerProfileService, TrainerProfileService>();
            services.AddScoped<ITrainingRequestService, TrainingRequestService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<ITrainingPaymentService, TrainingPaymentService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IScheduleService, ScheduleService>();

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
