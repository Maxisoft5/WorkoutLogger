using Microsoft.EntityFrameworkCore;
using Modules.Subscriptions.Infrastructure.Database;
using Modules.Subscriptions.Infrastructure.Domain;
using Modules.Subscriptions.Infrastructure.Services;

namespace Modules.Users.Tests.Payments;

[TestFixture]
public class SubscriptionRestoreTests
{
    private SubscriptionsDbContext _db = null!;
    private SubscriptionService _service = null!;

    private const string UserId = "user-1";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<SubscriptionsDbContext>()
            .UseInMemoryDatabase($"subs-{Guid.NewGuid()}")
            .Options;
        _db = new SubscriptionsDbContext(options);
        _service = new SubscriptionService(_db, [], new SubscriptionSettings());
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    private Subscription AddSubscription(
        SubscriptionStatus status, DateTime? expiresAt, DateTime? createdAt = null)
    {
        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = UserId,
            Plan = SubscriptionPlan.Monthly,
            Status = status,
            Provider = PaymentProviderType.Stripe,
            StartedAt = DateTime.UtcNow.AddDays(-10),
            ExpiresAt = expiresAt,
            CreatedAt = createdAt ?? DateTime.UtcNow.AddDays(-10),
        };
        _db.Subscriptions.Add(sub);
        _db.SaveChanges();
        return sub;
    }

    [Test]
    public async Task Restore_WithActiveSubscription_ReturnsIt()
    {
        var sub = AddSubscription(SubscriptionStatus.Active, DateTime.UtcNow.AddDays(20));

        var restored = await _service.RestoreAsync(UserId);

        Assert.That(restored?.Id, Is.EqualTo(sub.Id));
    }

    [Test]
    public async Task Restore_WithoutSubscriptions_ReturnsNull()
    {
        var restored = await _service.RestoreAsync(UserId);

        Assert.That(restored, Is.Null);
    }

    [Test]
    public async Task Restore_ExpiredActive_IsMarkedExpired_AndNullReturned()
    {
        var sub = AddSubscription(SubscriptionStatus.Active, DateTime.UtcNow.AddDays(-1));

        var restored = await _service.RestoreAsync(UserId);

        Assert.Multiple(() =>
        {
            Assert.That(restored, Is.Null);
            Assert.That(_db.Subscriptions.Single(s => s.Id == sub.Id).Status,
                Is.EqualTo(SubscriptionStatus.Expired));
        });
    }

    [Test]
    public async Task Restore_PrefersLatestNonExpired()
    {
        AddSubscription(SubscriptionStatus.Active, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddMonths(-2));
        var fresh = AddSubscription(SubscriptionStatus.Trial, DateTime.UtcNow.AddDays(25), DateTime.UtcNow.AddDays(-2));

        var restored = await _service.RestoreAsync(UserId);

        Assert.That(restored?.Id, Is.EqualTo(fresh.Id));
    }

    [Test]
    public async Task Restore_CancelledSubscription_IsIgnored()
    {
        AddSubscription(SubscriptionStatus.Cancelled, DateTime.UtcNow.AddDays(20));

        var restored = await _service.RestoreAsync(UserId);

        Assert.That(restored, Is.Null);
    }

    [Test]
    public async Task Restore_OtherUsersSubscription_IsIgnored()
    {
        _db.Subscriptions.Add(new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = "someone-else",
            Plan = SubscriptionPlan.Annual,
            Status = SubscriptionStatus.Active,
            Provider = PaymentProviderType.YooKassa,
            StartedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddYears(1),
            CreatedAt = DateTime.UtcNow,
        });
        _db.SaveChanges();

        var restored = await _service.RestoreAsync(UserId);

        Assert.That(restored, Is.Null);
    }
}
