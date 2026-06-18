using WorkoutLogger.EventsConsumer;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Configuration.GetValue<bool>("UseLocalhost"))
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Kafka:BootstrapServers"] = "localhost:9094",
        ["OpenSearch:Url"]         = "http://localhost:9200",
    });
}

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
