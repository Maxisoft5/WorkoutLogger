using Modules.Common.Infrastructure.Extensions;
using System.Text.Json.Serialization;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Grpc;

var builder = WebApplication.CreateBuilder(args);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<ExercisesGrpcService>();

app.Run();
