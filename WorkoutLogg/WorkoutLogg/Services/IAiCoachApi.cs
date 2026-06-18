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

    public interface IAiCoachApi
    {
        [Post("/api/ai/chat")]
        Task<IApiResponse<AiChatResponseDto>> ChatAsync(
            [Header("Authorization")] string token,
            [Body] AiChatRequestDto request,
            CancellationToken ct = default);
    }
}
