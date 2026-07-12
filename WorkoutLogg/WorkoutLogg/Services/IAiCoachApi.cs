using Refit;

namespace WorkoutLogg.Services
{
    public record AiChatMessageDto(string Role, string Content);

    public record AiWorkoutContextDto(
        int TotalSessions,
        int CurrentStreak,
        string? PersonalRecords,
        string? RecentSummary);

    public record AiChatRequestDto(
        List<AiChatMessageDto> Messages,
        AiWorkoutContextDto? Context,
        string? Language);

    public record AiChatResponseDto(string Content, bool Success, string? Error = null);

    public record AiForecastRequestDto(
        AiWorkoutContextDto? Context,
        string? Language,
        string? ExerciseName = null);

    public record AiPlanRequestDto(
        AiWorkoutContextDto? Context,
        string? Language,
        string? Goal = null,
        int? DaysPerWeek = null);

    public interface IAiCoachApi
    {
        [Post("/api/ai/chat")]
        Task<IApiResponse<AiChatResponseDto>> ChatAsync(
            [Header("Authorization")] string token,
            [Body] AiChatRequestDto request,
            CancellationToken ct = default);

        /// <summary>AI Record Forecast: прогноз следующих личных рекордов.</summary>
        [Post("/api/ai/forecast")]
        Task<IApiResponse<AiChatResponseDto>> ForecastAsync(
            [Header("Authorization")] string token,
            [Body] AiForecastRequestDto request,
            CancellationToken ct = default);

        /// <summary>AI Plan Generator: недельный план под цель пользователя.</summary>
        [Post("/api/ai/plan")]
        Task<IApiResponse<AiChatResponseDto>> GeneratePlanAsync(
            [Header("Authorization")] string token,
            [Body] AiPlanRequestDto request,
            CancellationToken ct = default);
    }
}
