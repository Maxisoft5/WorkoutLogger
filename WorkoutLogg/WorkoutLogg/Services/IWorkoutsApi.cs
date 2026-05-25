using Moduels.Workouts.DTO.Requests;
using Moduels.Workouts.DTO.Responses;
using Refit;

namespace WorkoutLogg.Services
{
    public interface IWorkoutsApi
    {
        [Post("/Workouts")]
        Task<IApiResponse<WorkoutResponse>> CreateWorkoutAsync(
            [Header("Authorization")] string token,
            [Body] CreateWorkoutRequest request,
            CancellationToken ct = default);

        [Put("/Workouts/{id}")]
        Task<IApiResponse<WorkoutResponse>> UpdateWorkoutAsync(
            [Header("Authorization")] string token,
            Guid id,
            [Body] UpdateWorkoutRequest request,
            CancellationToken ct = default);

        [Delete("/Workouts/{id}")]
        Task<IApiResponse<string>> DeleteWorkoutAsync(
            [Header("Authorization")] string token,
            Guid id,
            CancellationToken ct = default);

        [Get("/Workouts")]
        Task<IApiResponse<List<WorkoutResponse>>> GetWorkoutsAsync(
            [Header("Authorization")] string token,
            CancellationToken ct = default);
    }
}
