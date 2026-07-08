using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Common.Domain.Outbox;
using Modules.Common.Infrastructure.Email;
using Modules.Users.Infrastructure.Database;
using Modules.Users.Infrastructure.Outbox;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Text.Json;

namespace Modules.Users.Tests.Outbox;

[TestFixture]
public class OutboxProcessorServiceTests
{
    private UsersDbContext _db = null!;
    private IEmailSender _emailSender = null!;
    private OutboxProcessorService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase($"outbox-{Guid.NewGuid()}")
            .Options;
        _db = new UsersDbContext(options);
        _emailSender = Substitute.For<IEmailSender>();
        _sut = new OutboxProcessorService(
            Substitute.For<IServiceScopeFactory>(),
            NullLogger<OutboxProcessorService>.Instance);
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
        _sut.Dispose();
    }

    private static OutboxMessage Message(
        string to = "user@example.com",
        int retryCount = 0,
        DateTime? processedAtUtc = null,
        DateTime? createdAtUtc = null,
        string? payload = null) => new()
        {
            Type = "email",
            Payload = payload ?? JsonSerializer.Serialize(new EmailPayload(to, "subject", "body")),
            RetryCount = retryCount,
            ProcessedAtUtc = processedAtUtc,
            CreatedAtUtc = createdAtUtc ?? DateTime.UtcNow,
        };

    [Test]
    public async Task PendingMessage_IsSent_AndMarkedProcessed()
    {
        var msg = Message();
        _db.OutboxMessages.Add(msg);
        await _db.SaveChangesAsync();

        var processed = await _sut.ProcessBatchAsync(_db, _emailSender, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(processed, Is.EqualTo(1));
            Assert.That(msg.ProcessedAtUtc, Is.Not.Null);
            Assert.That(msg.Error, Is.Null);
            Assert.That(msg.RetryCount, Is.Zero);
        });
        await _emailSender.Received(1).SendAsync("user@example.com", "subject", "body", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task FailedSend_IncrementsRetryCount_StoresError_AndKeepsMessagePending()
    {
        var msg = Message();
        _db.OutboxMessages.Add(msg);
        await _db.SaveChangesAsync();
        _emailSender.SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("smtp down"));

        var processed = await _sut.ProcessBatchAsync(_db, _emailSender, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(processed, Is.Zero);
            Assert.That(msg.ProcessedAtUtc, Is.Null);
            Assert.That(msg.RetryCount, Is.EqualTo(1));
            Assert.That(msg.Error, Is.EqualTo("smtp down"));
        });
    }

    [Test]
    public async Task MessageAtRetryLimit_IsParked_AndNeverSent()
    {
        _db.OutboxMessages.Add(Message(retryCount: 3));
        await _db.SaveChangesAsync();

        var processed = await _sut.ProcessBatchAsync(_db, _emailSender, CancellationToken.None);

        Assert.That(processed, Is.Zero);
        await _emailSender.DidNotReceiveWithAnyArgs().SendAsync(default!, default!, default!, default);
    }

    [Test]
    public async Task AlreadyProcessedMessage_IsSkipped()
    {
        _db.OutboxMessages.Add(Message(processedAtUtc: DateTime.UtcNow));
        await _db.SaveChangesAsync();

        var processed = await _sut.ProcessBatchAsync(_db, _emailSender, CancellationToken.None);

        Assert.That(processed, Is.Zero);
        await _emailSender.DidNotReceiveWithAnyArgs().SendAsync(default!, default!, default!, default);
    }

    [Test]
    public async Task PoisonMessage_StopsBeingRetried_AfterThirdFailure()
    {
        var msg = Message(payload: "{ not valid json");
        _db.OutboxMessages.Add(msg);
        await _db.SaveChangesAsync();

        // Three batches → three failed attempts.
        for (var i = 0; i < 3; i++)
            await _sut.ProcessBatchAsync(_db, _emailSender, CancellationToken.None);

        Assert.That(msg.RetryCount, Is.EqualTo(3));

        // The fourth batch must not pick the message up again.
        var processed = await _sut.ProcessBatchAsync(_db, _emailSender, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(processed, Is.Zero);
            Assert.That(msg.RetryCount, Is.EqualTo(3));
            Assert.That(msg.ProcessedAtUtc, Is.Null);
        });
    }

    [Test]
    public async Task Batch_TakesAtMostTenMessages_OldestFirst()
    {
        var now = DateTime.UtcNow;
        for (var i = 0; i < 12; i++)
            _db.OutboxMessages.Add(Message(to: $"user{i}@example.com", createdAtUtc: now.AddMinutes(i)));
        await _db.SaveChangesAsync();

        var processed = await _sut.ProcessBatchAsync(_db, _emailSender, CancellationToken.None);

        Assert.That(processed, Is.EqualTo(10));
        // The two newest messages wait for the next batch.
        await _emailSender.DidNotReceive().SendAsync("user10@example.com", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _emailSender.DidNotReceive().SendAsync("user11@example.com", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _emailSender.Received(1).SendAsync("user0@example.com", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task FailedMessage_IsRetried_OnNextBatch_AndSucceeds()
    {
        var msg = Message();
        _db.OutboxMessages.Add(msg);
        await _db.SaveChangesAsync();

        _emailSender.SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("transient"));
        await _sut.ProcessBatchAsync(_db, _emailSender, CancellationToken.None);
        Assert.That(msg.RetryCount, Is.EqualTo(1));

        // The sender recovers.
        _emailSender.SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        var processed = await _sut.ProcessBatchAsync(_db, _emailSender, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(processed, Is.EqualTo(1));
            Assert.That(msg.ProcessedAtUtc, Is.Not.Null);
            Assert.That(msg.Error, Is.Null, "Error must be cleared after a successful retry");
        });
    }
}
