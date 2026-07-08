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

var builder = WebApplication.CreateBuilder(args);

// appsettings.Local.json — локальные секреты, не попадает в git
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);

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

// Если UseLocalhost=true — перекрываем все адреса localhost'ом поверх appsettings.json.
// Пароль БД берётся из POSTGRES_PASSWORD (env / appsettings.Local.json), а не хардкодится.
if (builder.Configuration.GetValue<bool>("UseLocalhost"))
{
    var localDbPassword = builder.Configuration["POSTGRES_PASSWORD"] ?? "postgres";
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ConnectionStrings:DefaultConnection"] = $"Host=localhost;Port=5432;Database=workoutLogger;Username=postgres;Password={localDbPassword}",
        ["ConnectionStrings:Redis"]             = "localhost:6379",
        ["Kafka:BootstrapServers"]              = "localhost:9094",
        ["OpenSearch:Url"]                      = "http://localhost:9200",
    });
}

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
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
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
builder.Services.AddKafkaMessaging(configuration);


var app = builder.Build();

app.UseExceptionHandler();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var usersDb = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    await usersDb.Database.MigrateAsync();

    var subsDb = scope.ServiceProvider.GetRequiredService<SubscriptionsDbContext>();
    await subsDb.Database.MigrateAsync();

    var trainersDb = scope.ServiceProvider.GetRequiredService<TrainersDbContext>();
    await trainersDb.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<ExercisesGrpcService>();

app.Run();
