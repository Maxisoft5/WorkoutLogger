using System.ComponentModel.DataAnnotations;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    // ─── Requests ────────────────────────────────────────────────────────────

    public class PostReviewRequest
    {
        /// <summary>ID завершённого платежа (TrainingPayment.Status = Completed).</summary>
        [Required]
        public Guid PaymentId { get; set; }

        /// <summary>Оценка 1–5.</summary>
        [Range(1, 5)]
        public int Rating { get; set; }

        /// <summary>Текст отзыва (опционально).</summary>
        [MaxLength(2000)]
        public string? Text { get; set; }
    }

    public class ReplyToReviewRequest
    {
        [Required]
        [MaxLength(2000)]
        public string Reply { get; set; } = null!;
    }

    // ─── DTOs ────────────────────────────────────────────────────────────────

    public class ReviewDto
    {
        public Guid Id { get; set; }
        public string StudentUserId { get; set; } = null!;
        public string TrainerUserId { get; set; } = null!;
        public Guid PaymentId { get; set; }
        public int Rating { get; set; }
        public string? Text { get; set; }
        public string? TrainerReply { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? TrainerRepliedAtUtc { get; set; }
    }

    public class TrainerRatingDto
    {
        public string TrainerUserId { get; set; } = null!;
        /// <summary>Средний рейтинг за последние 12 месяцев. Null если отзывов нет.</summary>
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class ReviewsPageDto
    {
        public List<ReviewDto> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public bool HasMore { get; set; }
    }

    internal static class ReviewMapper
    {
        public static ReviewDto ToDto(Review r) => new()
        {
            Id = r.Id,
            StudentUserId = r.StudentUserId,
            TrainerUserId = r.TrainerUserId,
            PaymentId = r.PaymentId,
            Rating = r.Rating,
            Text = r.Text,
            TrainerReply = r.TrainerReply,
            CreatedAtUtc = r.CreatedAtUtc,
            TrainerRepliedAtUtc = r.TrainerRepliedAtUtc,
        };
    }
}
