using Microsoft.EntityFrameworkCore;
using Modules.Common.Infrastructure.Extensions;
using Modules.Users.Infrastructure.Database;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Text.Json.Serialization;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, _, config) =>
{
    var openSearchUrl = ctx.Configuration["OpenSearch:Url"] ?? "http://opensearch:9200";

    config
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(openSearchUrl))
        {
            AutoRegisterTemplate = true,
            IndexFormat = "workoutlogger-logs-{0:yyyy.MM.dd}",
            NumberOfShards = 1,
            NumberOfReplicas = 0,
            EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog
        });
});

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters
        .Add(new JsonStringEnumConverter());
});
builder.Services.AddGrpc(); 

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var configration = builder.Configuration;
builder.Services.AddAuthModule(configration);
builder.Services.AddHybridCache(configration);
builder.Services.AddKafkaMessaging(configration);


var app = builder.Build();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    await db.Database.MigrateAsync();
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
