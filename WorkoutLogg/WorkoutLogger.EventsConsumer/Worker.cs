using Confluent.Kafka;
using OpenSearch.Client;
using OpenSearch.Net;
using System.Text.Json;

namespace WorkoutLogger.EventsConsumer;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bootstrap = configuration["Kafka:BootstrapServers"] ?? "localhost:9094";
        var topic = configuration["Kafka:Topics:AuthEvents"] ?? "auth-events";
        var openSearchUrl = configuration["OpenSearch:Url"] ?? "http://localhost:9200";

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrap,
            GroupId = "auth-events-to-opensearch",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false  // комитим вручную после успешной записи
        };

        var osSettings = new ConnectionSettings(new Uri(openSearchUrl))
            .DefaultIndex("auth-events");
        var os = new OpenSearchClient(osSettings);

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);

        logger.LogInformation("Consumer started, listening to {Topic}", topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result?.Message?.Value is null) continue;

                    var doc = JsonSerializer.Deserialize<JsonElement>(result.Message.Value, JsonOptions);

                    // Индекс по дате — стандартная практика для логов
                    var indexName = $"auth-events-{DateTime.UtcNow:yyyy.MM.dd}";
                    var response = await os.LowLevel.IndexAsync<StringResponse>(
                        indexName,
                        PostData.String(result.Message.Value),
                        ctx: stoppingToken);

                    if (response.Success)
                    {
                        consumer.Commit(result);
                        logger.LogDebug("Indexed event from offset {Offset}", result.Offset);
                    }
                    else
                    {
                        logger.LogError("OpenSearch index failed: {Error}", response.DebugInformation);
                    }
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException) { /* graceful shutdown */ }
        finally
        {
            consumer.Close();
        }
    }
}
