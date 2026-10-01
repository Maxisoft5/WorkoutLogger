using Microsoft.EntityFrameworkCore;
using Modules.Common.Infrastructure.Extensions;
using Modules.Subscriptions.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Database;
using Modules.Users.Infrastructure.Database;
using Serilog;
using Serilog.Sinks.OpenSearch;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Grpc;
using WorkoutLogger.WebApi.Tenancy;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// appsettings.Local.json — локальные секреты, не попадает в git
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);
builder.Configuration.AddJsonFile("appsettings.Tenants.json", optional: true, reloadOnChange: false);
// Окружение и аргументы запуска имеют приоритет над локальным JSON.
builder.Configuration.AddEnvironmentVariables().AddCommandLine(args);
builder.AddLocalServiceConfiguration();

Serilog.Debugging.SelfLog.Enable(msg => Console.Error.WriteLine($"[Serilog] {msg}"));

builder.Host.UseSerilog((ctx, _, config) =>
{
    var openSearchEnabled = ctx.Configuration.GetValue<bool>("OpenSearch:Enabled", true);
    var openSearchUrl = ctx.Configuration["OpenSearch:Url"] ?? "http://opensearch:9200";

    config
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

    if (openSearchEnabled)
    {
        config.WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri(openSearchUrl))
        {
            AutoRegisterTemplate = true,
            IndexFormat = "workoutlogger-logs-{0:yyyy.MM.dd}",
            NumberOfShards = 1,
            NumberOfReplicas = 0,
            EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog
                             | EmitEventFailureHandling.RaiseCallback,
            FailureCallback = e => Console.Error.WriteLine($"[Serilog] Failed: {e.MessageTemplate}")
        });
    }
});

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters
        .Add(new JsonStringEnumConverter());
});
builder.Services.AddGrpc(); 

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<WorkoutLogger.WebApi.Services.ICurrentUser, WorkoutLogger.WebApi.Services.CurrentUser>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<WorkoutLogger.WebApi.Services.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"{httpContext.RequestServices.GetRequiredService<Modules.Common.Infrastructure.Tenancy.TenantContext>().Current.Id}:{httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"}",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});


var configuration = builder.Configuration;
builder.Services.AddAuthModule(configuration);
builder.Services.AddSubscriptionsModule(configuration);
builder.Services.AddTrainersModule(configuration);
builder.Services.AddAiCoachService(configuration);
builder.Services.AddHybridCache(configuration);
builder.Services.AddLoginRateLimiter(configuration);
builder.Services.AddKafkaMessaging(configuration);
builder.Services.AddGymTenancy(configuration);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    // Host remains the original HTTP Host, checked against TenantCatalog.
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    foreach (var address in configuration.GetSection("ReverseProxy:KnownProxies").Get<string[]>() ?? [])
        options.KnownProxies.Add(System.Net.IPAddress.Parse(address));
});


var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();

app.MapDefaultEndpoints();

if (configuration.GetValue<bool>("Tenancy:MigrateOnStartup", true) || configuration.GetValue<bool>("Tenancy:MigrationOnly"))
    await app.Services.MigrateGymsAsync(configuration);
if (configuration.GetValue<bool>("Tenancy:MigrationOnly")) return;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Use(TenantServices.ResolveTenant);
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<ExercisesGrpcService>();
app.MapGrpcService<WorkoutsGrpcService>();
app.MapFallbackToFile("index.html");

app.Run();
