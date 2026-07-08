using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;

namespace Modules.Users.Tests.Trainers;

[TestFixture]
public class TrainerProfileServiceTests
{
    private TrainersDbContext _db = null!;
    private TrainerProfileService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TrainersDbContext>()
            .UseInMemoryDatabase($"trainers-{Guid.NewGuid()}")
            .Options;
        _db = new TrainersDbContext(options);
        _service = new TrainerProfileService(_db);
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    private static UpsertTrainerProfileRequest ValidRequest() => new()
    {
        Specializations = TrainerSpecializations.Strength | TrainerSpecializations.WeightLoss,
        Experience = ExperienceRange.ThreeToSevenYears,
        Formats = TrainingFormats.Online | TrainingFormats.Gym,
        PricePerSession = 450,
        About = "КМС по пауэрлифтингу, персональные программы",
        IsActive = true
    };

    [Test]
    public async Task Upsert_NewProfile_CreatesProfile()
    {
        var result = await _service.UpsertAsync("user-1", ValidRequest());

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.UserId, Is.EqualTo("user-1"));
            Assert.That(result.Value.PricePerSession, Is.EqualTo(450));
            Assert.That(result.Value.CreatedAtUtc, Is.Not.EqualTo(default(DateTime)));
            Assert.That(result.Value.UpdatedAtUtc, Is.Null);
            Assert.That(_db.TrainerProfiles.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task Upsert_ExistingProfile_UpdatesInsteadOfDuplicating()
    {
        await _service.UpsertAsync("user-1", ValidRequest());

        var updated = ValidRequest();
        updated.PricePerSession = 600;
        updated.Formats = TrainingFormats.OnSite;
        var result = await _service.UpsertAsync("user-1", updated);

        Assert.That(result.IsSuccess, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(_db.TrainerProfiles.Count(), Is.EqualTo(1));
            Assert.That(result.Value!.PricePerSession, Is.EqualTo(600));
            Assert.That(result.Value.Formats, Is.EqualTo(TrainingFormats.OnSite));
            Assert.That(result.Value.UpdatedAtUtc, Is.Not.Null);
        });
    }

    [Test]
    public async Task Upsert_NoSpecializations_ReturnsValidationError()
    {
        var request = ValidRequest();
        request.Specializations = TrainerSpecializations.None;

        var result = await _service.UpsertAsync("user-1", request);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NoSpecializations"));
        });
    }

    [Test]
    public async Task Upsert_NoFormats_ReturnsValidationError()
    {
        var request = ValidRequest();
        request.Formats = TrainingFormats.None;

        var result = await _service.UpsertAsync("user-1", request);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NoFormats"));
        });
    }

    [TestCase(0)]
    [TestCase(99)]
    [TestCase(50_001)]
    public async Task Upsert_PriceOutOfRange_ReturnsValidationError(int price)
    {
        var request = ValidRequest();
        request.PricePerSession = price;

        var result = await _service.UpsertAsync("user-1", request);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.InvalidPrice"));
        });
    }

    [Test]
    public async Task GetMy_MissingProfile_ReturnsNotFound()
    {
        var result = await _service.GetMyAsync("unknown-user");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.ProfileNotFound"));
        });
    }

    [Test]
    public async Task GetMy_ExistingProfile_ReturnsProfile()
    {
        await _service.UpsertAsync("user-1", ValidRequest());

        var result = await _service.GetMyAsync("user-1");

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.UserId, Is.EqualTo("user-1"));
    }

    [Test]
    public async Task GetActive_ReturnsOnlyActiveProfilesPaged()
    {
        for (var i = 1; i <= 5; i++)
        {
            var request = ValidRequest();
            request.PricePerSession = 200 + i * 100;
            request.IsActive = i != 5; // пятый профиль скрыт
            await _service.UpsertAsync($"user-{i}", request);
        }

        var page1 = await _service.GetActiveAsync(page: 1, pageSize: 3);
        var page2 = await _service.GetActiveAsync(page: 2, pageSize: 3);

        Assert.Multiple(() =>
        {
            Assert.That(page1.TotalCount, Is.EqualTo(4));
            Assert.That(page1.Items, Has.Count.EqualTo(3));
            Assert.That(page2.Items, Has.Count.EqualTo(1));
            Assert.That(page1.Items.Select(p => p.PricePerSession), Is.Ordered.Ascending);
            Assert.That(page1.Items.All(p => p.IsActive), Is.True);
        });
    }

    [Test]
    public async Task Search_FiltersBySpecializationFormatAndPrice()
    {
        var strengthOnline = ValidRequest(); // Strength|WeightLoss, Online|Gym, 450
        await _service.UpsertAsync("user-1", strengthOnline);

        var yogaOnSite = ValidRequest();
        yogaOnSite.Specializations = TrainerSpecializations.Yoga;
        yogaOnSite.Formats = TrainingFormats.OnSite;
        yogaOnSite.PricePerSession = 800;
        await _service.UpsertAsync("user-2", yogaOnSite);

        var result = await _service.SearchAsync(
            new TrainerSearchRequest
            {
                Specializations = TrainerSpecializations.Strength,
                Formats = TrainingFormats.Online,
                PriceMin = 200,
                PriceMax = 500
            },
            new StudentPreferences());

        Assert.Multiple(() =>
        {
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.Items, Has.Count.EqualTo(1));
            Assert.That(result.Items[0].Profile.UserId, Is.EqualTo("user-1"));
        });
    }

    [Test]
    public async Task Search_SortsByMatchScoreDescending()
    {
        var strength = ValidRequest();
        strength.Specializations = TrainerSpecializations.Strength;
        await _service.UpsertAsync("strength-trainer", strength);

        var yoga = ValidRequest();
        yoga.Specializations = TrainerSpecializations.Yoga;
        await _service.UpsertAsync("yoga-trainer", yoga);

        var preferences = new StudentPreferences { DesiredSpecializations = TrainerSpecializations.Yoga };
        var result = await _service.SearchAsync(new TrainerSearchRequest(), preferences);

        Assert.Multiple(() =>
        {
            Assert.That(result.Items, Has.Count.EqualTo(2));
            Assert.That(result.Items[0].Profile.UserId, Is.EqualTo("yoga-trainer"));
            Assert.That(result.Items[0].MatchScore, Is.GreaterThan(result.Items[1].MatchScore));
        });
    }

    [Test]
    public async Task Search_SortByPriceAsc_IgnoresMatchOrdering()
    {
        var cheap = ValidRequest();
        cheap.PricePerSession = 200;
        await _service.UpsertAsync("cheap", cheap);

        var expensive = ValidRequest();
        expensive.PricePerSession = 700;
        await _service.UpsertAsync("expensive", expensive);

        var result = await _service.SearchAsync(
            new TrainerSearchRequest { SortBy = TrainerSortBy.PriceAsc },
            new StudentPreferences());

        Assert.That(result.Items.Select(i => i.Profile.PricePerSession), Is.Ordered.Ascending);
    }

    [Test]
    public async Task Search_ExcludesInactiveTrainers()
    {
        var inactive = ValidRequest();
        inactive.IsActive = false;
        await _service.UpsertAsync("inactive", inactive);

        var result = await _service.SearchAsync(new TrainerSearchRequest(), new StudentPreferences());

        Assert.That(result.TotalCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GetActive_InvalidPaging_IsNormalized()
    {
        await _service.UpsertAsync("user-1", ValidRequest());

        var page = await _service.GetActiveAsync(page: 0, pageSize: -5);

        Assert.Multiple(() =>
        {
            Assert.That(page.Page, Is.EqualTo(1));
            Assert.That(page.PageSize, Is.EqualTo(1));
            Assert.That(page.Items, Has.Count.EqualTo(1));
        });
    }
}
