using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Modules.Common.Infrastructure.Messaging
{
    public class KafkaEventPublisher : IEventPublisher, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaEventPublisher> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public KafkaEventPublisher(KafkaSettings settings, ILogger<KafkaEventPublisher> logger)
        {
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                ClientId = "workoutlogger-webapi",
                // Надёжная доставка
                Acks = Acks.All,
                EnableIdempotence = true,
                MessageSendMaxRetries = 3,
                // Чтобы не блокировать запросы пользователей надолго
                MessageTimeoutMs = 5000
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync<T>(string topic, T @event, CancellationToken ct = default) where T : class
        {
            try
            {
                var json = JsonSerializer.Serialize(@event, JsonOptions);
                var message = new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),  // на проде сюда лучше userId для партицирования
                    Value = json,
                    Timestamp = new Timestamp(DateTime.UtcNow)
                };

                var result = await _producer.ProduceAsync(topic, message, ct);
                _logger.LogDebug("Event published to {Topic} at offset {Offset}", topic, result.Offset);
            }
            catch (ProduceException<string, string> ex)
            {
                // Не роняем основной flow если Kafka недоступна
                _logger.LogError(ex, "Failed to publish event to {Topic}: {Reason}", topic, ex.Error.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error publishing to {Topic}", topic);
            }
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }

    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = "localhost:9094";
        public AuthTopics Topics { get; set; } = new();
    }

    public class AuthTopics
    {
        public string AuthEvents { get; set; } = "auth-events";
    }
}
