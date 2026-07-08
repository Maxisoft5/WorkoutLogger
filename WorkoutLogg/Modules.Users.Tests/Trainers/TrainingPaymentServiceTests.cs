using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;

namespace Modules.Users.Tests.Trainers;

[TestFixture]
public class TrainingPaymentServiceTests
{
    private TrainersDbContext _db = null!;
    private WalletService _wallet = null!;
    private TrainingPaymentService _service = null!;

    private const string Student = "student-1";
    private const string Trainer = "trainer-1";
    private const int Price = 450;

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<TrainersDbContext>()
            .UseInMemoryDatabase($"payments-{Guid.NewGuid()}")
            .Options;
        _db = new TrainersDbContext(options);
        _wallet = new WalletService(_db);
        _service = new TrainingPaymentService(_db, _wallet);

        _db.TrainerProfiles.Add(new TrainerProfile
        {
            Id = Guid.NewGuid(),
            UserId = Trainer,
            Specializations = TrainerSpecializations.Strength,
            Formats = TrainingFormats.Online,
            PricePerSession = Price,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        });
        _db.TrainingRequests.Add(new TrainingRequest
        {
            Id = Guid.NewGuid(),
            StudentUserId = Student,
            TrainerUserId = Trainer,
            Status = TrainingRequestStatus.Accepted,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    private async Task TopUpStudentAsync(int amount = 1000) =>
        await _wallet.CreditAsync(Student, amount, WalletTransactionType.Adjustment, "test top-up");

    [Test]
    public void Commission_IsTenPercentRounded()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PlatformFees.CommissionFor(450), Is.EqualTo(45));
            Assert.That(PlatformFees.CommissionFor(455), Is.EqualTo(46)); // 45.5 → 46
            Assert.That(PlatformFees.CommissionFor(100), Is.EqualTo(10));
        });
    }

    [Test]
    public async Task Pay_DebitsStudentAndHoldsPayment()
    {
        await TopUpStudentAsync();

        var result = await _service.PayAsync(Student, Trainer);

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(async () =>
        {
            Assert.That(result.Value!.Status, Is.EqualTo(TrainingPaymentStatus.Held));
            Assert.That(result.Value.PriceFc, Is.EqualTo(Price));
            Assert.That(result.Value.CommissionFc, Is.EqualTo(45));
            Assert.That(result.Value.PayoutFc, Is.EqualTo(405));
            Assert.That((await _wallet.GetWalletAsync(Student)).Balance, Is.EqualTo(1000 - Price));
            // тренеру пока ничего не выплачено — эскроу
            Assert.That((await _wallet.GetWalletAsync(Trainer)).Balance, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Pay_WithoutAcceptedRequest_IsRejected()
    {
        await TopUpStudentAsync();

        var result = await _service.PayAsync("stranger", Trainer);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NoAcceptedRequest"));
    }

    [Test]
    public async Task Pay_InsufficientFunds_IsRejectedAndNothingPersisted()
    {
        await TopUpStudentAsync(100);

        var result = await _service.PayAsync(Student, Trainer);

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.InsufficientFunds"));
            Assert.That(_db.TrainingPayments.Count(), Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Pay_ToSelf_IsRejected()
    {
        var result = await _service.PayAsync(Trainer, Trainer);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.CannotRequestSelf"));
    }

    [Test]
    public async Task Complete_ByStudent_PaysTrainerMinusCommission()
    {
        await TopUpStudentAsync();
        var payment = await _service.PayAsync(Student, Trainer);

        var completed = await _service.CompleteAsync(Student, payment.Value!.Id);

        Assert.Multiple(async () =>
        {
            Assert.That(completed.Value!.Status, Is.EqualTo(TrainingPaymentStatus.Completed));
            Assert.That((await _wallet.GetWalletAsync(Trainer)).Balance, Is.EqualTo(405));
        });
    }

    [Test]
    public async Task Complete_ByTrainer_IsForbidden()
    {
        await TopUpStudentAsync();
        var payment = await _service.PayAsync(Student, Trainer);

        var result = await _service.CompleteAsync(Trainer, payment.Value!.Id);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NotPaymentStudent"));
    }

    [Test]
    public async Task Complete_Twice_IsRejected()
    {
        await TopUpStudentAsync();
        var payment = await _service.PayAsync(Student, Trainer);
        await _service.CompleteAsync(Student, payment.Value!.Id);

        var again = await _service.CompleteAsync(Student, payment.Value!.Id);

        Assert.Multiple(async () =>
        {
            Assert.That(again.Errors![0].Code, Is.EqualTo("Trainers.PaymentNotHeld"));
            Assert.That((await _wallet.GetWalletAsync(Trainer)).Balance, Is.EqualTo(405)); // без двойной выплаты
        });
    }

    [Test]
    public async Task Refund_ByTrainer_ReturnsFundsToStudent()
    {
        await TopUpStudentAsync();
        var payment = await _service.PayAsync(Student, Trainer);

        var refunded = await _service.RefundAsync(Trainer, payment.Value!.Id);

        Assert.Multiple(async () =>
        {
            Assert.That(refunded.Value!.Status, Is.EqualTo(TrainingPaymentStatus.Refunded));
            Assert.That((await _wallet.GetWalletAsync(Student)).Balance, Is.EqualTo(1000));
            Assert.That((await _wallet.GetWalletAsync(Trainer)).Balance, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Refund_ByStudent_IsForbidden()
    {
        await TopUpStudentAsync();
        var payment = await _service.PayAsync(Student, Trainer);

        var result = await _service.RefundAsync(Student, payment.Value!.Id);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NotPaymentTrainer"));
    }

    [Test]
    public async Task Refund_AfterComplete_IsRejected()
    {
        await TopUpStudentAsync();
        var payment = await _service.PayAsync(Student, Trainer);
        await _service.CompleteAsync(Student, payment.Value!.Id);

        var refund = await _service.RefundAsync(Trainer, payment.Value!.Id);

        Assert.That(refund.Errors![0].Code, Is.EqualTo("Trainers.PaymentNotHeld"));
    }

    [Test]
    public async Task Histories_ReturnStudentAndTrainerViews()
    {
        await TopUpStudentAsync();
        var payment = await _service.PayAsync(Student, Trainer);
        await _service.CompleteAsync(Student, payment.Value!.Id);

        var my = await _service.GetMyPaymentsAsync(Student);
        var received = await _service.GetReceivedPaymentsAsync(Trainer);

        Assert.Multiple(() =>
        {
            Assert.That(my, Has.Count.EqualTo(1));
            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0].PayoutFc, Is.EqualTo(405));
        });
    }
}
