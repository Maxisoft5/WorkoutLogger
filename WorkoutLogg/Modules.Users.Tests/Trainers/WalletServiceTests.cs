using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;

namespace Modules.Users.Tests.Trainers;

[TestFixture]
public class WalletServiceTests
{
    private TrainersDbContext _db = null!;
    private WalletService _service = null!;

    private const string UserId = "user-1";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TrainersDbContext>()
            .UseInMemoryDatabase($"wallet-{Guid.NewGuid()}")
            .Options;
        _db = new TrainersDbContext(options);
        _service = new WalletService(_db);
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    [Test]
    public async Task GetWallet_CreatesEmptyWalletLazily()
    {
        var wallet = await _service.GetWalletAsync(UserId);

        Assert.Multiple(() =>
        {
            Assert.That(wallet.Balance, Is.EqualTo(0));
            Assert.That(_db.Wallets.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task Credit_IncreasesBalance_AndWritesTransaction()
    {
        var result = await _service.CreditAsync(UserId, 200, WalletTransactionType.ChallengeReward, "Челлендж");

        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.Balance, Is.EqualTo(200));
            Assert.That(_db.WalletTransactions.Single().Amount, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task Debit_WithEnoughFunds_DecreasesBalance()
    {
        await _service.CreditAsync(UserId, 500, WalletTransactionType.ReferralBonus, null);

        var result = await _service.DebitAsync(UserId, 450, WalletTransactionType.TrainingPayment, "Тренировка");

        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.Balance, Is.EqualTo(50));
            Assert.That(_db.WalletTransactions.OrderBy(t => t.CreatedAtUtc).Last().Amount, Is.EqualTo(-450));
        });
    }

    [Test]
    public async Task Debit_InsufficientFunds_IsRejected()
    {
        await _service.CreditAsync(UserId, 100, WalletTransactionType.StreakBonus, null);

        var result = await _service.DebitAsync(UserId, 450, WalletTransactionType.TrainingPayment, null);

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.InsufficientFunds"));
            Assert.That(_db.Wallets.Single().Balance, Is.EqualTo(100));
        });
    }

    [TestCase(0)]
    [TestCase(-50)]
    public async Task Credit_NonPositiveAmount_IsRejected(int amount)
    {
        var result = await _service.CreditAsync(UserId, amount, WalletTransactionType.Adjustment, null);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.InvalidAmount"));
    }

    [Test]
    public async Task Credit_SameIdempotencyKey_IsRejectedOnce()
    {
        await _service.CreditAsync(UserId, 300, WalletTransactionType.ReferralBonus, null, "ref:friend-1");

        var duplicate = await _service.CreditAsync(UserId, 300, WalletTransactionType.ReferralBonus, null, "ref:friend-1");

        Assert.Multiple(() =>
        {
            Assert.That(duplicate.Errors![0].Code, Is.EqualTo("Trainers.DuplicateOperation"));
            Assert.That(_db.Wallets.Single().Balance, Is.EqualTo(300));
        });
    }

    [Test]
    public async Task GetHistory_ReturnsNewestFirstPaged()
    {
        for (var i = 1; i <= 5; i++)
        {
            await _service.CreditAsync(UserId, i * 10, WalletTransactionType.Adjustment, $"op-{i}");
            await Task.Delay(5); // разные CreatedAtUtc
        }

        var page = await _service.GetHistoryAsync(UserId, page: 1, pageSize: 3);

        Assert.Multiple(() =>
        {
            Assert.That(page.TotalCount, Is.EqualTo(5));
            Assert.That(page.Items, Has.Count.EqualTo(3));
            Assert.That(page.Items[0].Description, Is.EqualTo("op-5"));
        });
    }

    private static List<DateTime> StreakDates(DateTime today, int days) =>
        Enumerable.Range(0, days).Select(offset => today.AddDays(-offset)).ToList();

    [Test]
    public async Task ClaimStreak_SevenConsecutiveDays_AwardsBonus()
    {
        var today = DateTime.UtcNow.Date.AddHours(10); // относительно текущего дня: CreatedAtUtc в сервисе — реальный UtcNow

        var result = await _service.ClaimStreakBonusAsync(UserId, StreakDates(today, 7), today);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Balance, Is.EqualTo(RewardAmounts.Streak7Days));
        });
    }

    [Test]
    public async Task ClaimStreak_BrokenStreak_IsRejected()
    {
        var today = DateTime.UtcNow.Date.AddHours(10); // относительно текущего дня: CreatedAtUtc в сервисе — реальный UtcNow
        var dates = StreakDates(today, 7);
        dates.RemoveAt(3); // пропущен день

        var result = await _service.ClaimStreakBonusAsync(UserId, dates, today);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.StreakNotReached"));
    }

    [Test]
    public async Task ClaimStreak_SecondClaimWithinWindow_IsRejected()
    {
        var today = DateTime.UtcNow.Date.AddHours(10); // относительно текущего дня: CreatedAtUtc в сервисе — реальный UtcNow
        await _service.ClaimStreakBonusAsync(UserId, StreakDates(today, 7), today);

        var tomorrow = today.AddDays(1);
        var second = await _service.ClaimStreakBonusAsync(UserId, StreakDates(tomorrow, 7), tomorrow);

        Assert.That(second.Errors![0].Code, Is.EqualTo("Trainers.StreakBonusAlreadyClaimed"));
    }

    [Test]
    public async Task ClaimStreak_NewSeriesAfterWindow_AwardsAgain()
    {
        var today = DateTime.UtcNow.Date.AddHours(10); // относительно текущего дня: CreatedAtUtc в сервисе — реальный UtcNow
        await _service.ClaimStreakBonusAsync(UserId, StreakDates(today, 7), today);

        var nextSeriesDay = today.AddDays(RewardAmounts.StreakLengthDays);
        var result = await _service.ClaimStreakBonusAsync(UserId, StreakDates(nextSeriesDay, 7), nextSeriesDay);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Balance, Is.EqualTo(RewardAmounts.Streak7Days * 2));
        });
    }
}
