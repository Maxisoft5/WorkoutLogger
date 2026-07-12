using Microsoft.EntityFrameworkCore;
using Modules.Users.Infrastructure.Database;
using Modules.Users.Infrastructure.Workouts;
using Modules.Workouts.DTO.Enums;
using Modules.Workouts.DTO.Requests;

namespace Modules.Users.Tests.Workouts;

[TestFixture]
public class WorkoutServiceLiveUpdatesTests
{
    private UsersDbContext _db = null!;
    private WorkoutUpdatesBroker _broker = null!;
    private WorkoutService _service = null!;

    private const string UserId = "user-1";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase($"workouts-{Guid.NewGuid()}")
            .Options;
        _db = new UsersDbContext(options);
        _broker = new WorkoutUpdatesBroker();
        _service = new WorkoutService(_db, _broker);
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    private static CreateExerciseRequest Exercise(string name, params (int Reps, double Weight)[] sets) => new()
    {
        Name = name,
        Sets = sets.Select((s, i) => new CreateSetRequest
        {
            SetNumber = i + 1,
            Reps = s.Reps,
            WeightKg = s.Weight,
        }).ToList(),
    };

    private async Task<Guid> CreateWorkoutAsync(DateTime start, DateTime end)
    {
        var created = await _service.CreateAsync(UserId, new CreateWorkoutRequest
        {
            WorkoutType = WorkoutType.Strength,
            StartDate = start,
            EndDate = end,
            Exercises = [],
        });
        return created.Id;
    }

    [Test]
    public async Task Update_WithSets_PublishesLastSetCompleted()
    {
        var start = DateTime.UtcNow.AddHours(-1);
        var workoutId = await CreateWorkoutAsync(start, start);
        var (reader, subscription) = _broker.Subscribe(workoutId);

        using (subscription)
        {
            await _service.UpdateAsync(UserId, workoutId, new UpdateWorkoutRequest
            {
                WorkoutType = WorkoutType.Strength,
                StartDate = start,
                EndDate = start,
                Exercises = [Exercise("Bench Press", (8, 80), (5, 100))],
            });

            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
            var update = await reader.ReadAsync(ct);

            Assert.Multiple(() =>
            {
                Assert.That(update.SetCompleted, Is.Not.Null);
                Assert.That(update.SetCompleted!.ExerciseName, Is.EqualTo("Bench Press"));
                Assert.That(update.SetCompleted.Reps, Is.EqualTo(5), "должен публиковаться последний подход");
                Assert.That(update.SetCompleted.WeightKg, Is.EqualTo(100));
            });
        }
    }

    [Test]
    public async Task Update_MovingEndDate_PublishesWorkoutFinished()
    {
        var start = DateTime.UtcNow.AddHours(-1);
        var workoutId = await CreateWorkoutAsync(start, start);
        var (reader, subscription) = _broker.Subscribe(workoutId);

        using (subscription)
        {
            await _service.UpdateAsync(UserId, workoutId, new UpdateWorkoutRequest
            {
                WorkoutType = WorkoutType.Strength,
                StartDate = start,
                EndDate = start.AddMinutes(45),
                Exercises = [Exercise("Squat", (5, 120))],
            });

            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
            var first = await reader.ReadAsync(ct);   // SetCompleted
            var second = await reader.ReadAsync(ct);  // WorkoutFinished

            Assert.Multiple(() =>
            {
                Assert.That(first.SetCompleted, Is.Not.Null);
                Assert.That(second.Finished, Is.Not.Null);
                Assert.That(second.Finished!.TotalSets, Is.EqualTo(1));
                Assert.That(second.Finished.DurationSeconds, Is.EqualTo(45 * 60));
            });
        }
    }

    [Test]
    public async Task Update_SameEndDate_DoesNotPublishFinished()
    {
        var start = DateTime.UtcNow.AddHours(-2);
        var end = start.AddHours(1);
        var workoutId = await CreateWorkoutAsync(start, end);
        var (reader, subscription) = _broker.Subscribe(workoutId);

        using (subscription)
        {
            await _service.UpdateAsync(UserId, workoutId, new UpdateWorkoutRequest
            {
                WorkoutType = WorkoutType.Strength,
                StartDate = start,
                EndDate = end,
                Exercises = [Exercise("Deadlift", (3, 150))],
            });

            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
            var first = await reader.ReadAsync(ct);

            Assert.Multiple(() =>
            {
                Assert.That(first.SetCompleted, Is.Not.Null);
                Assert.That(reader.TryRead(out _), Is.False, "WorkoutFinished не должен публиковаться без переноса EndDate");
            });
        }
    }

    [Test]
    public async Task Update_WithoutBroker_DoesNotThrow()
    {
        var serviceWithoutBroker = new WorkoutService(_db);
        var start = DateTime.UtcNow.AddHours(-1);
        var workoutId = await CreateWorkoutAsync(start, start);

        Assert.DoesNotThrowAsync(() => serviceWithoutBroker.UpdateAsync(UserId, workoutId, new UpdateWorkoutRequest
        {
            WorkoutType = WorkoutType.Cardio,
            StartDate = start,
            EndDate = start.AddMinutes(30),
            Exercises = [],
        }));
    }
}
