namespace Modules.Trainers.Infrastructure.Domain
{
    /// <summary>
    /// Диалог тренер↔ученик (M6, кнопка «Написать» на экране 03).
    /// Диалог создаётся, только если между пользователями есть заявка (Pending/Accepted),
    /// — незапрошенные сообщения незнакомцам исключены (анти-спам из отчёта-анализа).
    /// MVP работает на поллинге; realtime (SignalR) — отдельный этап.
    /// </summary>
    public class Conversation
    {
        public Guid Id { get; set; }

        public string StudentUserId { get; set; } = null!;
        public string TrainerUserId { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; }

        /// <summary>Время последнего сообщения — для сортировки списка диалогов.</summary>
        public DateTime? LastMessageAtUtc { get; set; }

        public ICollection<ChatMessage> Messages { get; set; } = null!;
    }

    public class ChatMessage
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public string SenderUserId { get; set; } = null!;

        public string Text { get; set; } = null!;

        public DateTime SentAtUtc { get; set; }

        /// <summary>Когда сообщение прочитано получателем (null — непрочитано).</summary>
        public DateTime? ReadAtUtc { get; set; }
    }
}
