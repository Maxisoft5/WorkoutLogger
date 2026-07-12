using Modules.Users.Infrastructure.Workouts;

namespace Modules.Users.Tests.Workouts;

[TestFixture]
public class WorkoutUpdatesBrokerTests
{
    [Test]
    public async Task Publish_DeliversEventToSubscriber()
    {
        var broker = new WorkoutUpdatesBroker();
        var workoutId = Guid.NewGuid();
        var (reader, subscription) = broker.Subscribe(workoutId);

        using (subscription)
        {
            broker.Publish(new WorkoutUpdateEvent(
                workoutId, DateTime.UtcNow,
                SetCompleted: new SetCompletedEvent("ex-1", "Bench Press", 5, 100)));

            var received = await reader.ReadAsync(new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);
            Assert.Multiple(() =>
            {
                Assert.That(received.WorkoutId, Is.EqualTo(workoutId));
                Assert.That(received.SetCompleted!.Reps, Is.EqualTo(5));
                Assert.That(received.SetCompleted.WeightKg, Is.EqualTo(100));
            });
        }
    }

    [Test]
    public void Publish_ForAnotherWorkout_IsNotDelivered()
    {
        var broker = new WorkoutUpdatesBroker();
        var (reader, subscription) = broker.Subscribe(Guid.NewGuid());

        using (subscription)
        {
            broker.Publish(new WorkoutUpdateEvent(
                Guid.NewGuid(), DateTime.UtcNow,
                Finished: new WorkoutFinishedEvent(10, 3600)));

            Assert.That(reader.TryRead(out _), Is.False);
        }
    }

    [Test]
    public async Task Publish_MultipleSubscribers_AllReceive()
    {
        var broker = new WorkoutUpdatesBroker();
        var workoutId = Guid.NewGuid();
        var (reader1, sub1) = broker.Subscribe(workoutId);
        var (reader2, sub2) = broker.Subscribe(workoutId);

        using (sub1)
        using (sub2)
        {
            broker.Publish(new WorkoutUpdateEvent(
                workoutId, DateTime.UtcNow,
                Finished: new WorkoutFinishedEvent(12, 2700)));

            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
            var first = await reader1.ReadAsync(ct);
            var second = await reader2.ReadAsync(ct);

            Assert.Multiple(() =>
            {
                Assert.That(first.Finished!.TotalSets, Is.EqualTo(12));
                Assert.That(second.Finished!.DurationSeconds, Is.EqualTo(2700));
            });
        }
    }

    [Test]
    public void Dispose_RemovesSubscription_AndCompletesChannel()
    {
        var broker = new WorkoutUpdatesBroker();
        var workoutId = Guid.NewGuid();
        var (reader, subscription) = broker.Subscribe(workoutId);

        Assert.That(broker.SubscriberCount(workoutId), Is.EqualTo(1));

        subscription.Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(broker.SubscriberCount(workoutId), Is.EqualTo(0));
            Assert.That(reader.Completion.IsCompleted, Is.True);
        });
    }

    [Test]
    public void Dispose_Twice_IsSafe()
    {
        var broker = new WorkoutUpdatesBroker();
        var (_, subscription) = broker.Subscribe(Guid.NewGuid());

        subscription.Dispose();
        Assert.DoesNotThrow(subscription.Dispose);
    }
}
