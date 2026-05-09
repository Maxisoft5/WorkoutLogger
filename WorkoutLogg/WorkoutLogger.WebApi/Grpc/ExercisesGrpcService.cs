using Grpc.Core;
using WorkoutLogger.Grpc.Contracts;

namespace WorkoutLogger.WebApi.Grpc
{
    public class ExercisesGrpcService : ExercisesService.ExercisesServiceBase
    {
        // Заглушка: в реальности — репозиторий из БД
        private static readonly List<ExerciseDto> _seed =
        [
            new() { Id = "1", Name = "Bench Press",  MuscleGroup = "Chest",     Description = "Classic horizontal press", Difficulty = 3 },
            new() { Id = "2", Name = "Squat",        MuscleGroup = "Legs",      Description = "King of leg exercises",    Difficulty = 4 },
            new() { Id = "3", Name = "Deadlift",     MuscleGroup = "Back",      Description = "Full posterior chain",     Difficulty = 5 },
            new() { Id = "4", Name = "Pull-up",      MuscleGroup = "Back",      Description = "Bodyweight back builder",  Difficulty = 3 },
            new() { Id = "5", Name = "Overhead Press", MuscleGroup = "Shoulders", Description = "Standing barbell press", Difficulty = 4 },
        ];

        public override Task<ExerciseDto> GetExercise(GetExerciseRequest request, ServerCallContext context)
        {
            var exercise = _seed.FirstOrDefault(e => e.Id == request.Id);
            if (exercise is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Exercise {request.Id} not found"));

            return Task.FromResult(exercise);
        }

        public override async Task StreamExercises(
            StreamExercisesRequest request,
            IServerStreamWriter<ExerciseDto> responseStream,
            ServerCallContext context)
        {
            var query = string.IsNullOrWhiteSpace(request.MuscleGroup)
                ? _seed
                : _seed.Where(e => e.MuscleGroup.Equals(request.MuscleGroup, StringComparison.OrdinalIgnoreCase));

            foreach (var exercise in query)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                await responseStream.WriteAsync(exercise, context.CancellationToken);
                await Task.Delay(200, context.CancellationToken); // имитация чтения из БД
            }
        }
    }
}
