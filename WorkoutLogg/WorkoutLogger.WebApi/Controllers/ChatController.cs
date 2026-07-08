using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Trainers.Infrastructure.Services;
using WorkoutLogger.WebApi.Extensions;
using WorkoutLogger.WebApi.Services;

namespace WorkoutLogger.WebApi.Controllers
{
    /// <summary>
    /// Чат тренер↔ученик (M6, кнопка «Написать» на экране 03).
    /// Диалог доступен только паре, связанной заявкой (анти-спам).
    /// MVP работает на поллинге; realtime (SignalR) — отдельный этап.
    /// </summary>
    [ApiController]
    [Route("api/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ICurrentUser _currentUser;

        public ChatController(IChatService chatService, ICurrentUser currentUser)
        {
            _chatService = chatService;
            _currentUser = currentUser;
        }

        /// <summary>Открыть (или получить существующий) диалог с собеседником.</summary>
        [HttpPost("conversations")]
        public async Task<IActionResult> OpenConversation(
            [FromBody] OpenConversationRequest request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _chatService.GetOrCreateConversationAsync(userId, request.OtherUserId, ct);
            return result.ToActionResult();
        }

        /// <summary>Мои диалоги: последние сверху, с превью и счётчиком непрочитанного.</summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations(CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            return Ok(await _chatService.GetConversationsAsync(userId, ct));
        }

        /// <summary>Сообщения диалога (страницы от новых к старым, внутри страницы — хронологически).</summary>
        [HttpGet("conversations/{id:guid}/messages")]
        public async Task<IActionResult> GetMessages(
            Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _chatService.GetMessagesAsync(userId, id, page, pageSize, ct);
            return result.ToActionResult();
        }

        /// <summary>Отправить сообщение.</summary>
        [HttpPost("conversations/{id:guid}/messages")]
        public async Task<IActionResult> SendMessage(
            Guid id, [FromBody] SendMessageRequest request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _chatService.SendMessageAsync(userId, id, request.Text, ct);
            return result.ToActionResult();
        }

        /// <summary>Пометить входящие сообщения диалога прочитанными.</summary>
        [HttpPost("conversations/{id:guid}/read")]
        public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (userId is null) return Unauthorized();

            var result = await _chatService.MarkReadAsync(userId, id, ct);
            return result.ToActionResult();
        }
    }

    public class OpenConversationRequest
    {
        public string OtherUserId { get; set; } = null!;
    }

    public class SendMessageRequest
    {
        public string Text { get; set; } = null!;
    }
}
