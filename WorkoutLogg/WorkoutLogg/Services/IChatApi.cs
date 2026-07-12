using Refit;

namespace WorkoutLogg.Services
{
    // Client-side mirror of Modules.Trainers chat DTOs (см. IChatService).

    public class ConversationDto
    {
        public Guid Id { get; set; }
        public string StudentUserId { get; set; } = "";
        public string TrainerUserId { get; set; } = "";
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? LastMessageAtUtc { get; set; }
        public string? LastMessageText { get; set; }
        public int UnreadCount { get; set; }
    }

    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public string SenderUserId { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime SentAtUtc { get; set; }
        public DateTime? ReadAtUtc { get; set; }
    }

    public class ChatMessagesPageDto
    {
        public List<ChatMessageDto> Items { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public record OpenConversationRequestDto(string OtherUserId);

    public record SendMessageRequestDto(string Text);

    /// <summary>
    /// REST-контракт чата тренер↔ученик (M6). MVP работает на поллинге —
    /// страница диалога перечитывает сообщения по таймеру.
    /// </summary>
    public interface IChatApi
    {
        /// <summary>Открыть (или получить существующий) диалог с собеседником.</summary>
        [Post("/api/chat/conversations")]
        Task<IApiResponse<ConversationDto>> OpenConversationAsync(
            [Header("Authorization")] string token,
            [Body] OpenConversationRequestDto body,
            CancellationToken ct = default);

        /// <summary>Мои диалоги: последние сверху, с превью и счётчиком непрочитанного.</summary>
        [Get("/api/chat/conversations")]
        Task<IApiResponse<List<ConversationDto>>> GetConversationsAsync(
            [Header("Authorization")] string token,
            CancellationToken ct = default);

        /// <summary>Сообщения диалога (внутри страницы — хронологический порядок).</summary>
        [Get("/api/chat/conversations/{id}/messages")]
        Task<IApiResponse<ChatMessagesPageDto>> GetMessagesAsync(
            [Header("Authorization")] string token,
            Guid id,
            [AliasAs("page")] int page = 1,
            [AliasAs("pageSize")] int pageSize = 50,
            CancellationToken ct = default);

        /// <summary>Отправить сообщение.</summary>
        [Post("/api/chat/conversations/{id}/messages")]
        Task<IApiResponse<ChatMessageDto>> SendMessageAsync(
            [Header("Authorization")] string token,
            Guid id,
            [Body] SendMessageRequestDto body,
            CancellationToken ct = default);

        /// <summary>Пометить входящие сообщения диалога прочитанными.</summary>
        [Post("/api/chat/conversations/{id}/read")]
        Task<IApiResponse<int>> MarkReadAsync(
            [Header("Authorization")] string token,
            Guid id,
            CancellationToken ct = default);
    }
}
