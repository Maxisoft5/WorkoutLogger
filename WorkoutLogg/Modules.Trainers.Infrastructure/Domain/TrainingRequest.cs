namespace Modules.Trainers.Infrastructure.Domain
{
    /// <summary>
    /// Заявка ученика на тренировки (экран 03 «Тренер: вкладка Ученики»).
    /// Либо адресована конкретному тренеру (входящие заявки), либо открытая —
    /// попадает в ленту «Ищут тренера сейчас» и может быть принята любым тренером.
    /// </summary>
    public class TrainingRequest
    {
        public Guid Id { get; set; }

        public string StudentUserId { get; set; } = null!;

        /// <summary>Null — открытая заявка в ленту «Ищут тренера сейчас».</summary>
        public string? TrainerUserId { get; set; }

        /// <summary>Цель ученика в терминах специализаций тренера (для фильтра «По моему профилю»).</summary>
        public TrainerSpecializations Goal { get; set; }

        public StudentLevel Level { get; set; }

        /// <summary>Желаемые форматы занятий (для фильтра «Онлайн» в ленте).</summary>
        public TrainingFormats Formats { get; set; }

        /// <summary>График в свободной форме, например «Пн/Ср/Пт, вечер».</summary>
        public string? Schedule { get; set; }

        /// <summary>Бюджет за тренировку в FitCoins (карточка ученика в дизайне).</summary>
        public int? Budget { get; set; }

        public string? Message { get; set; }

        public TrainingRequestStatus Status { get; set; }

        /// <summary>Причина отклонения (заполняет тренер, опционально).</summary>
        public string? DeclineReason { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? RespondedAtUtc { get; set; }
    }

    public enum TrainingRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2,
        Cancelled = 3
    }

    public enum StudentLevel
    {
        Beginner = 0,
        Intermediate = 1,
        Advanced = 2
    }
}
