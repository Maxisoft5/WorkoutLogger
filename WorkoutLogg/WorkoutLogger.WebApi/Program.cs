using Microsoft.EntityFrameworkCore;
using Modules.Common.Infrastructure.Extensions;
using Modules.Subscriptions.Infrastructure.Database;
using Modules.Users.Infrastructure.Database;
using Serilog;
using Serilog.Sinks.OpenSearch;
using System.Text.Json.Serialization;
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

// Если UseLocalhost=true — перекрываем все адреса localhost'ом поверх appsettings.json
if (builder.Configuration.GetValue<bool>("UseLocalhost"))
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5432;Database=workoutLogger;Username=postgres;Password=051099",
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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<WorkoutLogger.WebApi.Services.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var configration = builder.Configuration;
builder.Services.AddAuthModule(configration);
builder.Services.AddSubscriptionsModule(configration);
builder.Services.AddAiCoachService(configration);
builder.Services.AddHybridCache(configration);
builder.Services.AddKafkaMessaging(configration);


var app = builder.Build();

app.UseExceptionHandler();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var usersDb = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    await usersDb.Database.MigrateAsync();

    var subsDb = scope.ServiceProvider.GetRequiredService<SubscriptionsDbContext>();
    await subsDb.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<ExercisesGrpcService>();

app.Run();
