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
        var kafkaEnabled      = configuration.GetValue<bool>("Kafka:Enabled", true);
        var openSearchEnabled = configuration.GetValue<bool>("OpenSearch:Enabled", true);

        if (!kafkaEnabled)
        {
            logger.LogInformation("Kafka is disabled — EventsConsumer idle");
            await Task.Delay(Timeout.Infinite, stoppingToken);
            return;
        }

        var bootstrap     = configuration["Kafka:BootstrapServers"] ?? "localhost:9094";
        var topic         = configuration["Kafka:Topics:AuthEvents"] ?? "auth-events";
        var openSearchUrl = configuration["OpenSearch:Url"] ?? "http://localhost:9200";

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrap,
            GroupId          = "auth-events-to-opensearch",
            AutoOffsetReset  = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        OpenSearchClient? os = null;
        if (openSearchEnabled)
        {
            var osSettings = new ConnectionSettings(new Uri(openSearchUrl)).DefaultIndex("auth-events");
            os = new OpenSearchClient(osSettings);
        }

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);

        logger.LogInformation("Consumer started, listening to {Topic} (OpenSearch={Enabled})", topic, openSearchEnabled);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result?.Message?.Value is null) continue;

                    if (os is not null)
                    {
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
                    else
                    {
                        consumer.Commit(result);
                        logger.LogDebug("Consumed (OpenSearch disabled) offset {Offset}", result.Offset);
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
