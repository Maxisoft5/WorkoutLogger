using Moduels.Workouts.DTO.Requests;
using Moduels.Workouts.DTO.Responses;

namespace Modules.Users.Infrastructure.Workouts;

public interface IWorkoutService
{
    Task<WorkoutResponse> CreateAsync(string userId, CreateWorkoutRequest request, CancellationToken ct = default);

    /// <summary>Returns the user's workouts ordered by start date (newest first), paged.</summary>
    Task<List<WorkoutResponse>> GetAsync(string userId, int page, int pageSize, CancellationToken ct = default);

    /// <summary>Replaces the workout's fields and exercises. Returns null when not found.</summary>
    Task<WorkoutResponse?> UpdateAsync(string userId, Guid workoutId, UpdateWorkoutRequest request, CancellationToken ct = default);

    /// <summary>Returns false when the workout does not exist or belongs to another user.</summary>
    Task<bool> DeleteAsync(string userId, Guid workoutId, CancellationToken ct = default);
}
