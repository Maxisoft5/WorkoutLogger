using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Modules.Users.Infrastructure.Workouts;
using WorkoutLogger.Grpc.Contracts;

namespace WorkoutLogger.WebApi.Grpc
{
    /// <summary>
    /// gRPC WatchWorkout: server-streaming live-обновлений активной тренировки.
    /// Клиент подписывается по workout_id и получает события SetCompleted /
    /// WorkoutFinished, которые публикует WorkoutService при каждом sync.
    /// </summary>
    [Authorize]
    public class WorkoutsGrpcService(WorkoutUpdatesBroker broker) : WorkoutsService.WorkoutsServiceBase
    {
        public override async Task WatchWorkout(
            WatchWorkoutRequest request,
            IServerStreamWriter<WorkoutUpdate> responseStream,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.WorkoutId, out var workoutId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "workout_id must be a GUID"));

            var (reader, subscription) = broker.Subscribe(workoutId);
            using (subscription)
            {
                try
                {
                    await foreach (var update in reader.ReadAllAsync(context.CancellationToken))
                    {
                        await responseStream.WriteAsync(MapUpdate(update), context.CancellationToken);

                        // Тренировка завершена — корректно закрываем стрим.
                        if (update.Finished is not null)
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    // Клиент отключился — штатное завершение стрима.
                }
            }
        }

        private static WorkoutUpdate MapUpdate(WorkoutUpdateEvent update)
        {
            var result = new WorkoutUpdate
            {
                WorkoutId = update.WorkoutId.ToString(),
                Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(update.TimestampUtc, DateTimeKind.Utc)),
            };

            if (update.SetCompleted is { } set)
            {
                result.SetCompleted = new SetCompleted
                {
                    ExerciseId = set.ExerciseId,
                    Reps = set.Reps,
                    WeightKg = set.WeightKg,
                };
            }
            else if (update.Finished is { } finished)
            {
                result.WorkoutFinished = new WorkoutFinished
                {
                    TotalSets = finished.TotalSets,
                    DurationSeconds = finished.DurationSeconds,
                };
            }

            return result;
        }
    }
}
