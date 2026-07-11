using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;

namespace Modules.Users.Tests.Trainers;

[TestFixture]
public class VerificationServiceTests
{
    private TrainersDbContext _db = null!;
    private VerificationService _service = null!;

    private const string Trainer = "trainer-1";
    private const string Moderator = "moderator-1";
    private const string FileUrl = "https://example.com/cert.pdf";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TrainersDbContext>()
            .UseInMemoryDatabase($"verification-{Guid.NewGuid()}")
            .Options;
        _db = new TrainersDbContext(options);
        _service = new VerificationService(_db);
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    // ─── Submit ───────────────────────────────────────────────────────────────

    [Test]
    public async Task Submit_ValidRequest_CreatesVerification()
    {
        var result = await _service.SubmitAsync(Trainer, new SubmitVerificationRequest
        {
            Documents =
            [
                new AddDocumentRequest { Type = DocumentType.Certificate, FileName = "cert.pdf", FileUrl = FileUrl },
            ],
        });

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.TrainerUserId, Is.EqualTo(Trainer));
            Assert.That(result.Value.Status, Is.EqualTo("Pending"));
            Assert.That(result.Value.Documents, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task Submit_Twice_ReturnsConflict()
    {
        await SubmitVerificationAsync();

        var result = await _service.SubmitAsync(Trainer, new SubmitVerificationRequest());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("VerificationAlreadyExists"));
    }

    [Test]
    public async Task Submit_InvalidFileUrl_ReturnsError()
    {
        var result = await _service.SubmitAsync(Trainer, new SubmitVerificationRequest
        {
            Documents =
            [
                new AddDocumentRequest { Type = DocumentType.Identity, FileName = "id.pdf", FileUrl = "http://not-https.com/file.pdf" },
            ],
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("InvalidFileUrl"));
    }

    // ─── AddDocument ──────────────────────────────────────────────────────────

    [Test]
    public async Task AddDocument_ToPendingVerification_Succeeds()
    {
        await SubmitVerificationAsync();

        var result = await _service.AddDocumentAsync(Trainer, new AddDocumentRequest
        {
            Type = DocumentType.SportTitle,
            FileName = "kms.pdf",
            FileUrl = FileUrl,
        });

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Documents, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task AddDocument_WhenNoVerification_ReturnsNotFound()
    {
        var result = await _service.AddDocumentAsync(Trainer, new AddDocumentRequest
        {
            Type = DocumentType.Certificate,
            FileName = "cert.pdf",
            FileUrl = FileUrl,
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("VerificationNotFound"));
    }

    // ─── RemoveDocument ───────────────────────────────────────────────────────

    [Test]
    public async Task RemoveDocument_OwnDocument_Succeeds()
    {
        var submitted = await SubmitVerificationAsync();
        var docId = submitted.Documents[0].Id;

        var result = await _service.RemoveDocumentAsync(Trainer, docId);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Documents, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task RemoveDocument_AnotherTrainersDocument_ReturnsForbidden()
    {
        await SubmitVerificationAsync();
        // Создаём отдельную верификацию для другого тренера.
        var other = await _service.SubmitAsync("trainer-2", new SubmitVerificationRequest
        {
            Documents = [new AddDocumentRequest { Type = DocumentType.Identity, FileName = "id.pdf", FileUrl = FileUrl }],
        });
        var otherDocId = other.Value!.Documents[0].Id;

        var result = await _service.RemoveDocumentAsync(Trainer, otherDocId);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("DocumentBelongsToAnotherVerification"));
    }

    // ─── Review (модерация) ───────────────────────────────────────────────────

    [Test]
    public async Task Review_Approve_SetsApprovedAndBadgeOnProfile()
    {
        var submitted = await SubmitVerificationAsync();
        await SeedTrainerProfileAsync();

        var result = await _service.ReviewAsync(Moderator, submitted.Id, new ReviewVerificationRequest
        {
            Approved = true,
            Badge = VerificationBadge.Master,
        });

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Status, Is.EqualTo("Approved"));
        Assert.That(result.Value.Badge, Is.EqualTo("Master"));

        // Проверяем, что бейдж выставлен на карточке тренера.
        var profile = _db.TrainerProfiles.Single(p => p.UserId == Trainer);
        Assert.That(profile.HasVerifiedBadge, Is.True);
        Assert.That(profile.VerificationBadge, Is.EqualTo(VerificationBadge.Master));
    }

    [Test]
    public async Task Review_Reject_SetsRejectedWithComment()
    {
        var submitted = await SubmitVerificationAsync();

        var result = await _service.ReviewAsync(Moderator, submitted.Id, new ReviewVerificationRequest
        {
            Approved = false,
            Comment = "Документы нечитаемые",
        });

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Status, Is.EqualTo("Rejected"));
        Assert.That(result.Value.ModeratorComment, Is.EqualTo("Документы нечитаемые"));
    }

    [Test]
    public async Task Review_AlreadyApproved_ReturnsConflict()
    {
        var submitted = await SubmitVerificationAsync();
        await _service.ReviewAsync(Moderator, submitted.Id, new ReviewVerificationRequest { Approved = true });

        var result = await _service.ReviewAsync(Moderator, submitted.Id, new ReviewVerificationRequest { Approved = true });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Errors[0].Code, Does.Contain("VerificationNotPending"));
    }

    // ─── GetPending ───────────────────────────────────────────────────────────

    [Test]
    public async Task GetPending_ReturnsPendingOnly()
    {
        var v1 = await SubmitVerificationAsync(Trainer);
        var v2 = await SubmitVerificationAsync("trainer-2");
        // Одобряем v1.
        await _service.ReviewAsync(Moderator, v1.Id, new ReviewVerificationRequest { Approved = true });

        var pending = await _service.GetPendingAsync();

        Assert.That(pending, Has.Count.EqualTo(1));
        Assert.That(pending[0].TrainerUserId, Is.EqualTo("trainer-2"));
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private async Task<TrainerVerificationDto> SubmitVerificationAsync(string? trainerId = null)
    {
        trainerId ??= Trainer;
        var result = await _service.SubmitAsync(trainerId, new SubmitVerificationRequest
        {
            Documents = [new AddDocumentRequest { Type = DocumentType.Certificate, FileName = "cert.pdf", FileUrl = FileUrl }],
        });
        Assert.That(result.IsSuccess, Is.True, "test setup: submit should succeed");
        return result.Value!;
    }

    private async Task SeedTrainerProfileAsync()
    {
        _db.TrainerProfiles.Add(new TrainerProfile
        {
            Id = Guid.NewGuid(),
            UserId = Trainer,
            Specializations = TrainerSpecializations.Strength,
            Formats = TrainingFormats.Online,
            PricePerSession = 500,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
        });
        await _db.SaveChangesAsync();
    }
}
