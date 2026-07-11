namespace Modules.Trainers.Infrastructure.Domain
{
    /// <summary>
    /// Верификация тренера (M9, пробел №5 из отчёта-анализа):
    /// загрузка документов, ручная модерация, бейдж «Проверен»/«КМС».
    /// </summary>
    public class TrainerVerification
    {
        public Guid Id { get; set; }
        public string TrainerUserId { get; set; } = null!;

        public VerificationStatus Status { get; set; }

        /// <summary>Комментарий модератора при отклонении заявки.</summary>
        public string? ModeratorComment { get; set; }

        /// <summary>Бейдж, назначенный при одобрении (Verified, Master, etc.).</summary>
        public VerificationBadge? Badge { get; set; }

        public DateTime SubmittedAtUtc { get; set; }
        public DateTime? ReviewedAtUtc { get; set; }
        public string? ReviewedByUserId { get; set; }
    }

    /// <summary>
    /// Документ, загруженный тренером в рамках заявки на верификацию.
    /// В MVP хранится URL файла (загрузка через отдельный сервис/S3).
    /// </summary>
    public class VerificationDocument
    {
        public Guid Id { get; set; }
        public Guid VerificationId { get; set; }
        public TrainerVerification Verification { get; set; } = null!;

        public DocumentType Type { get; set; }

        /// <summary>Имя файла (для отображения в UI).</summary>
        public string FileName { get; set; } = null!;

        /// <summary>
        /// URL загруженного файла. В MVP — временная ссылка; в продакшене — S3/Blob Storage.
        /// Максимум 2048 символов.
        /// </summary>
        public string FileUrl { get; set; } = null!;

        public DateTime UploadedAtUtc { get; set; }
    }

    public enum VerificationStatus
    {
        /// <summary>Тренер отправил заявку, ожидает проверки.</summary>
        Pending = 0,

        /// <summary>Заявка одобрена, бейдж присвоен.</summary>
        Approved = 1,

        /// <summary>Заявка отклонена (с комментарием).</summary>
        Rejected = 2,
    }

    public enum VerificationBadge
    {
        /// <summary>Документы проверены: «✓ Проверен».</summary>
        Verified = 0,

        /// <summary>КМС или мастер спорта подтверждён: «🏅 КМС».</summary>
        Master = 1,
    }

    public enum DocumentType
    {
        /// <summary>Сертификат тренера / диплом.</summary>
        Certificate = 0,

        /// <summary>Паспорт или другой удостоверяющий документ.</summary>
        Identity = 1,

        /// <summary>Документ о спортивном звании (КМС, МС и т.д.).</summary>
        SportTitle = 2,

        /// <summary>Другое.</summary>
        Other = 3,
    }
}
