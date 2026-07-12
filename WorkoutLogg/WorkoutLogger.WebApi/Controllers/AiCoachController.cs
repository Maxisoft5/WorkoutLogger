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
            var premiumCheck = await EnsurePremiumAsync();
            if (premiumCheck is not null) return premiumCheck;

            var systemPrompt = BuildSystemPrompt(request.Context, request.Language);

            var messages = request.Messages
                .Select(m => (m.Role, m.Content))
                .ToList();

            return await CompleteAsync(systemPrompt, messages, ct);
        }

        /// <summary>
        /// AI Record Forecast: прогноз новых личных рекордов на основе
        /// истории тренировок и текущих PR пользователя.
        /// </summary>
        [HttpPost("forecast")]
        public async Task<IActionResult> Forecast([FromBody] AiForecastApiRequest request, CancellationToken ct)
        {
            var premiumCheck = await EnsurePremiumAsync();
            if (premiumCheck is not null) return premiumCheck;

            var systemPrompt = BuildSystemPrompt(request.Context, request.Language);
            var lang = ResolveLanguage(request.Language);

            var userPrompt =
                $"Act as a strength forecasting engine. Based on my personal records and recent training data above, " +
                $"forecast when I can realistically hit new personal records in my main lifts" +
                (string.IsNullOrWhiteSpace(request.ExerciseName) ? "" : $", focusing on {request.ExerciseName}") +
                ". For each lift give: current PR, forecasted next PR (weight), and an estimated horizon " +
                $"expressed in weeks of consistent training. Add one short tip per lift. Reply in {lang}.";

            return await CompleteAsync(systemPrompt, [("user", userPrompt)], ct);
        }

        /// <summary>
        /// AI Plan Generator: генерация недельного плана тренировок под цель
        /// пользователя с учётом его реальной истории.
        /// </summary>
        [HttpPost("plan")]
        public async Task<IActionResult> GeneratePlan([FromBody] AiPlanApiRequest request, CancellationToken ct)
        {
            var premiumCheck = await EnsurePremiumAsync();
            if (premiumCheck is not null) return premiumCheck;

            var systemPrompt = BuildSystemPrompt(request.Context, request.Language);
            var lang = ResolveLanguage(request.Language);
            var daysPerWeek = Math.Clamp(request.DaysPerWeek ?? 3, 1, 7);

            var userPrompt =
                $"Generate a one-week training plan with {daysPerWeek} sessions" +
                (string.IsNullOrWhiteSpace(request.Goal) ? "" : $" for this goal: {request.Goal}") +
                ". Use my training data above to pick suitable exercises and working weights. " +
                "Format: one line per day (Day — focus), then exercises as 'Name — sets×reps @ weight'. " +
                $"Keep it compact and actionable. Reply in {lang}.";

            return await CompleteAsync(systemPrompt, [("user", userPrompt)], ct);
        }

        private async Task<IActionResult?> EnsurePremiumAsync()
        {
            var email = currentUser.Email;
            if (email is null) return Unauthorized();

            var user = await userService.GetUserByEmail(email);
            if (!user.IsSuccess || user.Value?.IsPremium != true)
                return StatusCode(403, new { error = "Premium subscription required" });

            return null;
        }

        private async Task<IActionResult> CompleteAsync(
            string systemPrompt, List<(string Role, string Content)> messages, CancellationToken ct)
        {
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

        private static string ResolveLanguage(string? language) =>
            language?.StartsWith("ru", StringComparison.OrdinalIgnoreCase) == true ? "Russian" : "English";

        private static string BuildSystemPrompt(AiWorkoutContextDto? ctx, string? language)
        {
            var lang = ResolveLanguage(language);

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

    public record AiForecastApiRequest(
        AiWorkoutContextDto? Context,
        string? Language,
        [property: MaxLength(200)] string? ExerciseName = null);

    public record AiPlanApiRequest(
        AiWorkoutContextDto? Context,
        string? Language,
        [property: MaxLength(500)] string? Goal = null,
        [property: Range(1, 7)] int? DaysPerWeek = null);

    public record AiChatApiResponse(string Content, bool Success, string? Error = null);
}
