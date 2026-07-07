using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Domain.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AiCoachController(AiChatService claude, IUserService userService, ICurrentUser currentUser) : ControllerBase
    {
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] AiChatApiRequest request, CancellationToken ct)
        {
            var email = currentUser.Email;
            if (email is null) return Unauthorized();

            var user = await userService.GetUserByEmail(email);
            if (!user.IsSuccess || user.Value?.IsPremium != true)
                return StatusCode(403, new { error = "Premium subscription required" });

            var systemPrompt = BuildSystemPrompt(request.Context, request.Language);

            var messages = request.Messages
                .Select(m => (m.Role, m.Content))
                .ToList();

            try
            {
                var reply = await claude.ChatAsync(systemPrompt, messages, ct);
                return Ok(new AiChatApiResponse(reply, true));
            }
            catch (Exception)
            {
                // The underlying error is already logged in AiChatService.
                // Never surface the raw exception message to the client.
                return StatusCode(500, new AiChatApiResponse("", false, "AI service is temporarily unavailable"));
            }
        }

        private static string BuildSystemPrompt(AiWorkoutContextDto? ctx, string? language)
        {
            var lang = language?.StartsWith("ru", StringComparison.OrdinalIgnoreCase) == true ? "Russian" : "English";

            var sb = new StringBuilder();
            sb.AppendLine($"You are an AI fitness coach inside the WorkoutLog mobile app. Be concise, specific, and motivating. Reply in {lang}.");
            sb.AppendLine();

            if (ctx is not null)
            {
                sb.AppendLine("USER WORKOUT DATA:");
                sb.AppendLine($"- Total logged sessions: {ctx.TotalSessions}");
                sb.AppendLine($"- Current streak: {ctx.CurrentStreak} days");

                if (!string.IsNullOrWhiteSpace(ctx.PersonalRecords))
                    sb.AppendLine($"- Personal records: {ctx.PersonalRecords}");

                if (!string.IsNullOrWhiteSpace(ctx.RecentSummary))
                {
                    sb.AppendLine("- Recent sessions:");
                    sb.AppendLine(ctx.RecentSummary);
                }

                sb.AppendLine();
            }

            sb.AppendLine("Guidelines: base your answers on the actual data above. Keep responses to 2-4 sentences unless a plan is requested. Use specific numbers from the data when relevant.");
            return sb.ToString();
        }
    }

    public record AiChatMessageDto(
        [property: Required, MaxLength(20)] string Role,
        [property: Required, MaxLength(4000)] string Content);

    public record AiWorkoutContextDto(
        int TotalSessions,
        int CurrentStreak,
        string? PersonalRecords,
        string? RecentSummary);

    public record AiChatApiRequest(
        [property: Required, MinLength(1), MaxLength(50)] List<AiChatMessageDto> Messages,
        AiWorkoutContextDto? Context,
        string? Language);

    public record AiChatApiResponse(string Content, bool Success, string? Error = null);
}
