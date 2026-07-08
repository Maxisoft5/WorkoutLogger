using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;

namespace Modules.Users.Tests.Trainers;

[TestFixture]
public class TrainingRequestServiceTests
{
    private TrainersDbContext _db = null!;
    private TrainingRequestService _service = null!;

    private const string Student = "student-1";
    private const string Trainer = "trainer-1";

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<TrainersDbContext>()
            .UseInMemoryDatabase($"requests-{Guid.NewGuid()}")
            .Options;
        _db = new TrainersDbContext(options);
        _service = new TrainingRequestService(_db);

        _db.TrainerProfiles.Add(new TrainerProfile
        {
            Id = Guid.NewGuid(),
            UserId = Trainer,
            Specializations = TrainerSpecializations.Strength,
            Formats = TrainingFormats.Online,
            PricePerSession = 450,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    private static CreateTrainingRequestDto DirectRequest(string? trainerId = Trainer) => new()
    {
        TrainerUserId = trainerId,
        Goal = TrainerSpecializations.Strength,
        Level = StudentLevel.Beginner,
        Formats = TrainingFormats.Online,
        Schedule = "Пн/Ср/Пт, вечер",
        Budget = 500,
        Message = "Хочу набрать форму к лету"
    };

    [Test]
    public async Task Create_DirectRequest_IsPending()
    {
        var result = await _service.CreateAsync(Student, DirectRequest());

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.Status, Is.EqualTo(TrainingRequestStatus.Pending));
            Assert.That(result.Value.TrainerUserId, Is.EqualTo(Trainer));
        });
    }

    [Test]
    public async Task Create_DuplicatePendingToSameTrainer_IsRejected()
    {
        await _service.CreateAsync(Student, DirectRequest());

        var second = await _service.CreateAsync(Student, DirectRequest());

        Assert.Multiple(() =>
        {
            Assert.That(second.IsError, Is.True);
            Assert.That(second.Errors![0].Code, Is.EqualTo("Trainers.RequestAlreadyPending"));
        });
    }

    [Test]
    public async Task Create_ToSelf_IsRejected()
    {
        var result = await _service.CreateAsync(Trainer, DirectRequest());

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.CannotRequestSelf"));
    }

    [Test]
    public async Task Create_ToUnknownOrInactiveTrainer_IsRejected()
    {
        var result = await _service.CreateAsync(Student, DirectRequest("ghost-trainer"));

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.TrainerNotFoundOrInactive"));
    }

    [Test]
    public async Task Create_SecondOpenRequest_IsRejected()
    {
        await _service.CreateAsync(Student, DirectRequest(trainerId: null));

        var second = await _service.CreateAsync(Student, DirectRequest(trainerId: null));

        Assert.That(second.Errors![0].Code, Is.EqualTo("Trainers.OpenRequestAlreadyPending"));
    }

    [Test]
    public async Task Accept_DirectRequest_MarksAcceptedAndSetsRespondedAt()
    {
        var created = await _service.CreateAsync(Student, DirectRequest());

        var accepted = await _service.AcceptAsync(Trainer, created.Value!.Id);

        Assert.That(accepted.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(accepted.Value!.Status, Is.EqualTo(TrainingRequestStatus.Accepted));
            Assert.That(accepted.Value.RespondedAtUtc, Is.Not.Null);
        });
    }

    [Test]
    public async Task Accept_ByAnotherTrainer_IsForbidden()
    {
        var created = await _service.CreateAsync(Student, DirectRequest());

        var result = await _service.AcceptAsync("other-trainer", created.Value!.Id);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NotRequestTrainer"));
    }

    [Test]
    public async Task Accept_OpenRequest_AssignsTrainer()
    {
        var created = await _service.CreateAsync(Student, DirectRequest(trainerId: null));

        var accepted = await _service.AcceptAsync(Trainer, created.Value!.Id);

        Assert.Multiple(() =>
        {
            Assert.That(accepted.IsSuccess, Is.True);
            Assert.That(accepted.Value!.TrainerUserId, Is.EqualTo(Trainer));
        });
    }

    [Test]
    public async Task Accept_AlreadyAccepted_IsRejected()
    {
        var created = await _service.CreateAsync(Student, DirectRequest());
        await _service.AcceptAsync(Trainer, created.Value!.Id);

        var again = await _service.AcceptAsync(Trainer, created.Value!.Id);

        Assert.That(again.Errors![0].Code, Is.EqualTo("Trainers.RequestNotPending"));
    }

    [Test]
    public async Task Decline_WithReason_StoresReason()
    {
        var created = await _service.CreateAsync(Student, DirectRequest());

        var declined = await _service.DeclineAsync(Trainer, created.Value!.Id, "Нет свободных слотов");

        Assert.Multiple(() =>
        {
            Assert.That(declined.Value!.Status, Is.EqualTo(TrainingRequestStatus.Declined));
            Assert.That(declined.Value.DeclineReason, Is.EqualTo("Нет свободных слотов"));
        });
    }

    [Test]
    public async Task Cancel_ByOwner_Works_ByOthers_Forbidden()
    {
        var created = await _service.CreateAsync(Student, DirectRequest());

        var foreign = await _service.CancelAsync("someone-else", created.Value!.Id);
        var own = await _service.CancelAsync(Student, created.Value!.Id);

        Assert.Multiple(() =>
        {
            Assert.That(foreign.Errors![0].Code, Is.EqualTo("Trainers.NotRequestOwner"));
            Assert.That(own.Value!.Status, Is.EqualTo(TrainingRequestStatus.Cancelled));
        });
    }

    [Test]
    public async Task OpenFeed_Filters_ByProfileOnlineAndBeginners()
    {
        // подходит по всем фильтрам
        await _service.CreateAsync("s1", new CreateTrainingRequestDto
        {
            Goal = TrainerSpecializations.Strength,
            Level = StudentLevel.Beginner,
            Formats = TrainingFormats.Online
        });
        // не по профилю (йога), офлайн, продвинутый
        await _service.CreateAsync("s2", new CreateTrainingRequestDto
        {
            Goal = TrainerSpecializations.Yoga,
            Level = StudentLevel.Advanced,
            Formats = TrainingFormats.Gym
        });

        var all = await _service.GetOpenFeedAsync(Trainer, new OpenRequestsFeedFilter());
        var filtered = await _service.GetOpenFeedAsync(Trainer, new OpenRequestsFeedFilter
        {
            ByMyProfile = true,
            OnlineOnly = true,
            BeginnersOnly = true
        });

        Assert.Multiple(() =>
        {
            Assert.That(all.Value!.TotalCount, Is.EqualTo(2));
            Assert.That(filtered.Value!.TotalCount, Is.EqualTo(1));
            Assert.That(filtered.Value.Items[0].StudentUserId, Is.EqualTo("s1"));
        });
    }

    [Test]
    public async Task OpenFeed_ByMyProfile_WithoutProfile_ReturnsNotFound()
    {
        var result = await _service.GetOpenFeedAsync("no-profile-trainer",
            new OpenRequestsFeedFilter { ByMyProfile = true });

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.ProfileNotFound"));
    }

    [Test]
    public async Task Stats_CountsPendingAndDistinctStudents()
    {
        var r1 = await _service.CreateAsync("s1", DirectRequest());
        await _service.AcceptAsync(Trainer, r1.Value!.Id);
        await _service.CreateAsync("s2", DirectRequest());

        var stats = await _service.GetStatsAsync(Trainer);

        Assert.Multiple(() =>
        {
            Assert.That(stats.PendingRequestsCount, Is.EqualTo(1));
            Assert.That(stats.StudentsCount, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task IncomingAndStudents_ReturnExpectedBuckets()
    {
        var r1 = await _service.CreateAsync("s1", DirectRequest());
        await _service.AcceptAsync(Trainer, r1.Value!.Id);
        await _service.CreateAsync("s2", DirectRequest());

        var incoming = await _service.GetIncomingAsync(Trainer);
        var students = await _service.GetMyStudentsAsync(Trainer);

        Assert.Multiple(() =>
        {
            Assert.That(incoming, Has.Count.EqualTo(1));
            Assert.That(incoming[0].StudentUserId, Is.EqualTo("s2"));
            Assert.That(students, Has.Count.EqualTo(1));
            Assert.That(students[0].StudentUserId, Is.EqualTo("s1"));
        });
    }
}
