using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Modules.Common.Infrastructure.Caching;
using Modules.Common.Infrastructure.RateLimiting;
using Modules.Common.Infrastructure.Tenancy;
using Modules.Users.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Database;
using Modules.Subscriptions.Infrastructure.Database;
using Npgsql;
using WorkoutLogger.WebApi.Site;
using Modules.Subscriptions.Infrastructure.Services;

namespace WorkoutLogger.WebApi.Tenancy;

public static class TenantServices
{
    public static IServiceCollection AddGymTenancy(this IServiceCollection services, IConfiguration configuration)
    {
        var definitions = configuration.GetSection("Tenancy:Tenants").Get<TenantDefinition[]>() ??
            [new() { Id = "legacy", Hosts = ["localhost", "127.0.0.1", "10.0.2.2"], Name = "WorkoutLogg" }];
        services.AddSingleton(new TenantCatalog(definitions));
        services.AddScoped<TenantContext>();
        // Payment credentials and return URLs cannot be shared implicitly between gyms.
        services.AddScoped(sp =>
        {
            var tenant = sp.GetRequiredService<TenantContext>().Current;
            var section = tenant.Schema is null ? configuration.GetSection("SubscriptionSettings")
                : configuration.GetSection($"Tenancy:Subscriptions:{tenant.Id}");
            return section.Get<SubscriptionSettings>() ?? new SubscriptionSettings();
        });
        services.AddDbContext<SiteDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddSingleton<HybridCacheService>();
        services.AddScoped<ICacheService, TenantCacheService>();
        services.AddSingleton<LoginRateLimiter>();
        services.AddScoped<ILoginRateLimiter, TenantLoginRateLimiter>();
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnTokenValidated = context =>
            {
                if (!context.HttpContext.RequestServices.GetRequiredService<TenantContext>().Accepts(context.Principal!))
                    context.Fail("This session belongs to another gym.");
                return Task.CompletedTask;
            };
        });
        services.AddAuthorizationBuilder().AddPolicy("GymAdmin", policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context => context.Resource is HttpContext http &&
                http.RequestServices.GetRequiredService<TenantContext>().Current.OwnerUserIds
                    .Contains(context.User.FindFirst("userid")?.Value, StringComparer.Ordinal)));
        services.AddAuthorizationBuilder().AddPolicy("GymTrainer", policy => policy.RequireAuthenticatedUser()
            .RequireAssertion(async context =>
            {
                if (context.Resource is not HttpContext http) return false;
                if (http.RequestServices.GetRequiredService<TenantContext>().Current.Schema is null) return true;
                var id = context.User.FindFirst("userid")?.Value;
                return await http.RequestServices.GetRequiredService<TrainersDbContext>().TrainerProfiles
                    .AnyAsync(x => x.UserId == id && x.IsActive, http.RequestAborted);
            }));
        return services;
    }

    public static async Task ResolveTenant(HttpContext context, RequestDelegate next)
    {
        var tenant = context.RequestServices.GetRequiredService<TenantCatalog>().FindByHost(context.Request.Host.Host);
        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { message = "Для этого домена фитнес-зал не настроен." });
            return;
        }
        context.RequestServices.GetRequiredService<TenantContext>().Set(tenant);
        await next(context);
    }

    public static async Task MigrateGymsAsync(this IServiceProvider services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Database connection is not configured.");
        foreach (var tenant in services.GetRequiredService<TenantCatalog>().Tenants)
        {
            // Canonical context models are essential: EF compares them to existing snapshots.
            await using var users = new UsersDbContext(Options<UsersDbContext>(connectionString, tenant, "Users"));
            await using var trainers = new TrainersDbContext(Options<TrainersDbContext>(connectionString, tenant, "Trainers"));
            await using var subscriptions = new SubscriptionsDbContext(Options<SubscriptionsDbContext>(connectionString, tenant, "Subscriptions"));
            await users.Database.MigrateAsync();
            await trainers.Database.MigrateAsync();
            await subscriptions.Database.MigrateAsync();
            await using var site = new SiteDbContext(Options<SiteDbContext>(connectionString, tenant, "Site"));
            await site.Database.MigrateAsync();
        }
    }

    public static DbContextOptions<T> Options<T>(string connectionString, TenantDefinition tenant, string module) where T : DbContext
    {
        var builder = new DbContextOptionsBuilder<T>();
        if (tenant.Schema is null) return builder.UseNpgsql(connectionString).Options;
        if (!TenantCatalog.IsValidSchema(tenant.Schema)) throw new InvalidOperationException("Invalid schema.");
        var connection = new NpgsqlConnectionStringBuilder(connectionString) { SearchPath = tenant.Schema };
        builder.UseNpgsql(connection.ConnectionString, options => options.MigrationsHistoryTable($"__{module}Migrations", tenant.Schema));
        builder.ReplaceService<IMigrationsSqlGenerator, TenantMigrationsSqlGenerator>();
        return builder.Options;
    }
}
