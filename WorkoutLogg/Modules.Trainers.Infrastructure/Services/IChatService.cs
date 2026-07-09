using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public string StudentUserId { get; set; } = null!;
        public string TrainerUserId { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? LastMessageAtUtc { get; set; }

        /// <summary>Последнее сообщение для превью в списке диалогов.</summary>
        public string? LastMessageText { get; set; }

        /// <summary>Непрочитанные сообщения для текущего пользователя.</summary>
        public int UnreadCount { get; set; }
    }

    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public string SenderUserId { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime SentAtUtc { get; set; }
        public DateTime? ReadAtUtc { get; set; }
    }

    public class ChatMessagesPageDto
    {
        /// <summary>Сообщения в хронологическом порядке (старые сверху) внутри страницы.</summary>
        public List<ChatMessageDto> Items { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public static class ChatMapper
    {
        public static ChatMessageDto MapMessage(this ChatMessage message) => new()
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderUserId = message.SenderUserId,
            Text = message.Text,
            SentAtUtc = message.SentAtUtc,
            ReadAtUtc = message.ReadAtUtc
        };
    }

    public interface IChatService
    {
        /// <summary>
        /// Открывает (или возвращает существующий) диалог с собеседником.
        /// Требуется заявка (Pending/Accepted) между учеником и тренером — в любую сторону.
        /// </summary>
        Task<Result<ConversationDto>> GetOrCreateConversationAsync(string userId, string otherUserId, CancellationToken ct = default);

        /// <summary>Диалоги текущего пользователя: последние сверху, с превью и счётчиком непрочитанного.</summary>
        Task<List<ConversationDto>> GetConversationsAsync(string userId, CancellationToken ct = default);

        /// <summary>Сообщения диалога (только участникам), страницы от новых к старым.</summary>
        Task<Result<ChatMessagesPageDto>> GetMessagesAsync(string userId, Guid conversationId, int page, int pageSize, CancellationToken ct = default);

        /// <summary>Отправка сообщения (только участникам).</summary>
        Task<Result<ChatMessageDto>> SendMessageAsync(string userId, Guid conversationId, string text, CancellationToken ct = default);

        /// <summary>Пометить входящие сообщения диалога прочитанными; возвращает число помеченных.</summary>
        Task<Result<int>> MarkReadAsync(string userId, Guid conversationId, CancellationToken ct = default);
    }
}
