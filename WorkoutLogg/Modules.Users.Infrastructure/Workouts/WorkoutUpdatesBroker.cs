using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Modules.Users.Infrastructure.Workouts;

/// <summary>Завершённый подход активной тренировки.</summary>
public record SetCompletedEvent(string ExerciseId, string ExerciseName, int Reps, double WeightKg);

/// <summary>Итог завершённой тренировки.</summary>
public record WorkoutFinishedEvent(int TotalSets, int DurationSeconds);

/// <summary>
/// Событие live-обновления тренировки. Заполнено ровно одно из полей
/// (SetCompleted либо Finished) — зеркало oneof из workouts.proto.
/// </summary>
public record WorkoutUpdateEvent(
    Guid WorkoutId,
    DateTime TimestampUtc,
    SetCompletedEvent? SetCompleted = null,
    WorkoutFinishedEvent? Finished = null,
    string TenantId = "legacy");

/// <summary>
/// In-process-брокер live-обновлений тренировок для gRPC WatchWorkout:
/// подписчики получают события конкретной тренировки через bounded-каналы
/// (медленный подписчик теряет старые события, но не блокирует публикацию).
/// </summary>
public class WorkoutUpdatesBroker
{
    private const int SubscriberBufferSize = 64;

    private readonly ConcurrentDictionary<(string TenantId, Guid WorkoutId), ConcurrentDictionary<Guid, Channel<WorkoutUpdateEvent>>> _subscribers = new();

    /// <summary>Подписка на события тренировки. Dispose снимает подписку.</summary>
    public (ChannelReader<WorkoutUpdateEvent> Reader, IDisposable Subscription) Subscribe(Guid workoutId, string tenantId = "legacy")
    {
        var channel = Channel.CreateBounded<WorkoutUpdateEvent>(new BoundedChannelOptions(SubscriberBufferSize)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
        });

        var subscriptionId = Guid.NewGuid();
        var workoutSubscribers = _subscribers.GetOrAdd((tenantId, workoutId),
            _ => new ConcurrentDictionary<Guid, Channel<WorkoutUpdateEvent>>());
        workoutSubscribers[subscriptionId] = channel;

        return (channel.Reader, new Subscription(this, workoutId, subscriptionId, tenantId));
    }

    /// <summary>Разослать событие всем подписчикам тренировки.</summary>
    public void Publish(WorkoutUpdateEvent update)
    {
        if (!_subscribers.TryGetValue((update.TenantId, update.WorkoutId), out var workoutSubscribers))
            return;

        foreach (var channel in workoutSubscribers.Values)
            channel.Writer.TryWrite(update);
    }

    /// <summary>Число активных подписчиков тренировки (для тестов и диагностики).</summary>
    public int SubscriberCount(Guid workoutId, string tenantId = "legacy") =>
        _subscribers.TryGetValue((tenantId, workoutId), out var subs) ? subs.Count : 0;

    private void Unsubscribe(Guid workoutId, Guid subscriptionId, string tenantId)
    {
        if (!_subscribers.TryGetValue((tenantId, workoutId), out var workoutSubscribers))
            return;

        if (workoutSubscribers.TryRemove(subscriptionId, out var channel))
            channel.Writer.TryComplete();

        if (workoutSubscribers.IsEmpty)
            _subscribers.TryRemove((tenantId, workoutId), out _);
    }

    private sealed class Subscription(WorkoutUpdatesBroker broker, Guid workoutId, Guid subscriptionId, string tenantId) : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
                broker.Unsubscribe(workoutId, subscriptionId, tenantId);
        }
    }
}
