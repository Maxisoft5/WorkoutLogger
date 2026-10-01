using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modules.Common.Infrastructure.Email;
using Modules.Users.Infrastructure.Database;
using System.Text.Json;
using Modules.Common.Infrastructure.Tenancy;

namespace Modules.Users.Infrastructure.Outbox;

public class OutboxProcessorService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessorService> logger, TenantCatalog? tenants = null) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox processor error");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        foreach (var tenant in tenants?.Tenants ?? [new TenantDefinition { Id = "legacy" }])
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                scope.ServiceProvider.GetService<TenantContext>()?.Set(tenant);
                var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                await ProcessBatchAsync(db, emailSender, ct);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                logger.LogError(ex, "Outbox processing failed for tenant {TenantId}", tenant.Id);
            }
        }
    }

    // Internal so the batch logic can be unit-tested directly
    // (see Modules.Users.Tests) without spinning up the ExecuteAsync loop.
    internal async Task<int> ProcessBatchAsync(UsersDbContext db, IEmailSender emailSender, CancellationToken ct)
    {
        var messages = await db.OutboxMessages
            .Where(m => m.ProcessedAtUtc == null && m.RetryCount < 3)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(10)
            .ToListAsync(ct);

        var processed = 0;
        foreach (var msg in messages)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<EmailPayload>(msg.Payload)!;
                await emailSender.SendAsync(payload.To, payload.Subject, payload.Body, ct);
                msg.ProcessedAtUtc = DateTime.UtcNow;
                msg.Error = null;
                processed++;
                logger.LogInformation("Outbox message {Id} processed, sent to {To}", msg.Id, payload.To);
            }
            catch (Exception ex)
            {
                msg.Error = ex.Message;
                msg.RetryCount++;
                logger.LogError(ex, "Failed to process outbox message {Id}, retry {Retry}", msg.Id, msg.RetryCount);
            }
        }

        if (messages.Count > 0)
            await db.SaveChangesAsync(ct);

        return processed;
    }
}
