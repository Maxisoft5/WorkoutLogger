using Moduels.Workouts.DTO.Requests;
using WorkoutLogg.Database;
using WorkoutLogg.Database.Entities;

namespace WorkoutLogg.Services
{
    public class WorkoutSyncService
    {
        private readonly WorkoutDatabase _db;
        private readonly IWorkoutsApi _api;

        public WorkoutSyncService(WorkoutDatabase db, IWorkoutsApi api)
        {
            _db = db;
            _api = api;
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
                await TrySyncAsync();
        }

        public async Task TrySyncAsync(CancellationToken ct = default)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return;

            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;

            var unsynced = await _db.GetUnsyncedAsync();
            foreach (var workout in unsynced)
            {
                if (ct.IsCancellationRequested) break;
                await SyncOneAsync(workout, token, ct);
            }
        }

        private async Task SyncOneAsync(WorkoutEntity workout, string token, CancellationToken ct)
        {
            var bearer = $"Bearer {token}";

            if (workout.RemoteId != Guid.Empty)
            {
                var req = new UpdateWorkoutRequest
                {
                    WorkoutType = workout.MuscleGroup,
                    StartDate = workout.StartDate,
                    EndDate = workout.EndDate,
                    Exercises = workout.Exercises.Select(ToExerciseRequest).ToList(),
                };
                var resp = await _api.UpdateWorkoutAsync(bearer, workout.RemoteId, req, ct);
                if (resp.IsSuccessStatusCode)
                {
                    workout.IsSynced = true;
                    await _db.SaveWorkoutAsync(workout);
                }
            }
            else
            {
                var req = new CreateWorkoutRequest
                {
                    LocalId = workout.Id,
                    WorkoutType = workout.MuscleGroup,
                    StartDate = workout.StartDate,
                    EndDate = workout.EndDate,
                    Exercises = workout.Exercises.Select(ToExerciseRequest).ToList(),
                };
                var resp = await _api.CreateWorkoutAsync(bearer, req, ct);
                if (resp.IsSuccessStatusCode && resp.Content is not null)
                {
                    workout.IsSynced = true;
                    workout.RemoteId = resp.Content.Id;
                    await _db.SaveWorkoutAsync(workout);
                }
            }
        }

        public async Task TryDeleteRemoteAsync(Guid remoteId)
        {
            if (remoteId == Guid.Empty) return;
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return;
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;
            await _api.DeleteWorkoutAsync($"Bearer {token}", remoteId);
        }

        private static CreateExerciseRequest ToExerciseRequest(WorkoutSetEntity ex) => new()
        {
            Name = ex.ExerciseName,
            Description = ex.Description,
            Complexity = ex.ExerciesComplexity,
            Sets = ex.Sets.Select(s => new CreateSetRequest
            {
                SetNumber = s.SetNumber,
                Reps = s.Reps,
                WeightKg = s.WeightKg,
                RestSeconds = s.RestSeconds,
                IsWarmup = s.IsWarmup,
            }).ToList(),
        };
    }
}
