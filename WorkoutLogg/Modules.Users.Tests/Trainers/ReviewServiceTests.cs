using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;

namespace Modules.Users.Tests.Trainers;

[TestFixture]
public class ReviewServiceTests
{
    private TrainersDbContext _db = null!;
    private ReviewService _service = null!;

    private const string Student = "student-1";
    private const string Trainer = "trainer-1";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TrainersDbContext>()
            .UseInMemoryDatabase($"reviews-{Guid.NewGuid()}")
            .Options;
        _db = new TrainersDbContext(options);
        _service = new ReviewService(_db);
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    // ─── PostReview ───────────────────────────────────────────────────────────

    [Test]
    public async Task PostReview_AfterCompletedPayment_Succeeds()
    {
        var paymentId = await SeedCompletedPaymentAsync();

        var result = await _service.PostAsync(Student, new PostReviewRequest
        {
            PaymentId = paymentId,
            Rating = 5,
            Text = "Отличный тренер!",
        });

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.Rating, Is.EqualTo(5));
            Assert.That(result.Value.Text, Is.EqualTo("Отличный тренер!"));
            Assert.That(result.Value.TrainerUserId, Is.EqualTo(Trainer));
            Assert.That(_db.Reviews.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task PostReview_InvalidRating_ReturnsError()
    {
        var paymentId = await SeedCompletedPaymentAsync();

        var result = await _service.PostAsync(Student, new PostReviewRequest
        {
            PaymentId = paymentId,
            Rating = 6,
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("InvalidRating"));
    }

    [Test]
    public async Task PostReview_PaymentNotCompleted_ReturnsError()
    {
        var paymentId = await SeedPaymentAsync(TrainingPaymentStatus.Held);

        var result = await _service.PostAsync(Student, new PostReviewRequest
        {
            PaymentId = paymentId,
            Rating = 4,
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("ReviewRequiresCompletedPayment"));
    }

    [Test]
    public async Task PostReview_WrongStudent_ReturnsError()
    {
        var paymentId = await SeedCompletedPaymentAsync();

        var result = await _service.PostAsync("other-student", new PostReviewRequest
        {
            PaymentId = paymentId,
            Rating = 4,
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("ReviewRequiresCompletedPayment"));
    }

    [Test]
    public async Task PostReview_DuplicateForSamePayment_ReturnsConflict()
    {
        var paymentId = await SeedCompletedPaymentAsync();
        await _service.PostAsync(Student, new PostReviewRequest { PaymentId = paymentId, Rating = 5 });

        var result = await _service.PostAsync(Student, new PostReviewRequest { PaymentId = paymentId, Rating = 3 });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("ReviewAlreadyExists"));
    }

    // ─── ReplyToReview ────────────────────────────────────────────────────────

    [Test]
    public async Task Reply_ByCorrectTrainer_Succeeds()
    {
        var reviewId = await SeedReviewAsync();

        var result = await _service.ReplyAsync(Trainer, reviewId, new ReplyToReviewRequest
        {
            Reply = "Спасибо за доверие!",
        });

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.TrainerReply, Is.EqualTo("Спасибо за доверие!"));
            Assert.That(result.Value.TrainerRepliedAtUtc, Is.Not.Null);
        });
    }

    [Test]
    public async Task Reply_ByWrongTrainer_ReturnsForbidden()
    {
        var reviewId = await SeedReviewAsync();

        var result = await _service.ReplyAsync("other-trainer", reviewId, new ReplyToReviewRequest { Reply = "..." });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("ReviewReplyForbidden"));
    }

    [Test]
    public async Task Reply_Twice_ReturnsConflict()
    {
        var reviewId = await SeedReviewAsync();
        await _service.ReplyAsync(Trainer, reviewId, new ReplyToReviewRequest { Reply = "Первый ответ" });

        var result = await _service.ReplyAsync(Trainer, reviewId, new ReplyToReviewRequest { Reply = "Второй ответ" });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("ReplyAlreadyExists"));
    }

    // ─── GetRating ────────────────────────────────────────────────────────────

    [Test]
    public async Task GetRating_NoReviews_ReturnsNullAverage()
    {
        var rating = await _service.GetRatingAsync(Trainer);

        Assert.That(rating.AverageRating, Is.Null);
        Assert.That(rating.ReviewCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GetRating_MultipleReviews_CalculatesCorrectAverage()
    {
        await SeedReviewWithRatingAsync(5);
        await SeedReviewWithRatingAsync(3);
        await SeedReviewWithRatingAsync(4);

        var rating = await _service.GetRatingAsync(Trainer);

        Assert.That(rating.AverageRating, Is.EqualTo(4.0).Within(0.01));
        Assert.That(rating.ReviewCount, Is.EqualTo(3));
    }

    [Test]
    public async Task GetRatingBatch_ReturnsRatingsForAllIds()
    {
        await SeedReviewWithRatingAsync(5);

        var dict = await _service.GetRatingBatchAsync([Trainer, "trainer-no-reviews"]);

        Assert.That(dict, Has.Count.EqualTo(2));
        Assert.That(dict[Trainer].AverageRating, Is.EqualTo(5.0).Within(0.01));
        Assert.That(dict["trainer-no-reviews"].AverageRating, Is.Null);
    }

    // ─── GetTrainerReviews ────────────────────────────────────────────────────

    [Test]
    public async Task GetTrainerReviews_Pagination_ReturnsCorrectPage()
    {
        for (var i = 0; i < 5; i++)
            await SeedReviewWithRatingAsync(i + 1);

        var page = await _service.GetTrainerReviewsAsync(Trainer, page: 1, pageSize: 3);

        Assert.That(page.Items, Has.Count.EqualTo(3));
        Assert.That(page.TotalCount, Is.EqualTo(5));
        Assert.That(page.HasMore, Is.True);
    }

    [Test]
    public async Task GetTrainerReviews_NewestFirst()
    {
        var p1 = await SeedCompletedPaymentAsync();
        var p2 = await SeedCompletedPaymentAsync();
        await _service.PostAsync(Student, new PostReviewRequest { PaymentId = p1, Rating = 3 });
        await _service.PostAsync(Student, new PostReviewRequest { PaymentId = p2, Rating = 5 });

        var page = await _service.GetTrainerReviewsAsync(Trainer, page: 1, pageSize: 10);

        // Последний добавленный — первый в списке.
        Assert.That(page.Items[0].Rating, Is.EqualTo(5));
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private async Task<Guid> SeedPaymentAsync(TrainingPaymentStatus status)
    {
        var payment = new TrainingPayment
        {
            Id = Guid.NewGuid(),
            StudentUserId = Student,
            TrainerUserId = Trainer,
            PriceFc = 450,
            CommissionFc = 45,
            PayoutFc = 405,
            Status = status,
            CreatedAtUtc = DateTime.UtcNow,
        };
        _db.TrainingPayments.Add(payment);
        await _db.SaveChangesAsync();
        return payment.Id;
    }

    private Task<Guid> SeedCompletedPaymentAsync() => SeedPaymentAsync(TrainingPaymentStatus.Completed);

    private async Task<Guid> SeedReviewAsync(int rating = 5)
    {
        var paymentId = await SeedCompletedPaymentAsync();
        var result = await _service.PostAsync(Student, new PostReviewRequest
        {
            PaymentId = paymentId,
            Rating = rating,
        });
        Assert.That(result.IsSuccess, Is.True, "test setup: review creation should succeed");
        return result.Value!.Id;
    }

    private Task<Guid> SeedReviewWithRatingAsync(int rating) => SeedReviewAsync(rating);
}
