namespace Modules.Trainers.Infrastructure.Domain
{
    /// <summary>
    /// Отзыв ученика на тренера (M8, пробел №4 из отчёта-анализа).
    /// Отзыв доступен только после оплаченной (Completed) тренировки — защита от накруток.
    /// Один отзыв на каждый завершённый платёж.
    /// </summary>
    public class Review
    {
        public Guid Id { get; set; }

        public string StudentUserId { get; set; } = null!;
        public string TrainerUserId { get; set; } = null!;

        /// <summary>
        /// Привязка к завершённой оплате — гарантирует, что ученик оплатил и подтвердил тренировку.
        /// Уникальное ограничение предотвращает повторный отзыв по одному и тому же платежу.
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>Оценка 1–5.</summary>
        public int Rating { get; set; }

        /// <summary>Текст отзыва (опционально, до 2000 символов).</summary>
        public string? Text { get; set; }

        /// <summary>Ответ тренера на отзыв (опционально).</summary>
        public string? TrainerReply { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? TrainerRepliedAtUtc { get; set; }
    }
}
