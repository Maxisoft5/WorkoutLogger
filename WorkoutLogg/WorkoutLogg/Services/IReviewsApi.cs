using Refit;

namespace WorkoutLogg.Services
{
    // Client-side mirror of Modules.Trainers review DTOs (см. ReviewDtos).

    public class ReviewDto
    {
        public Guid Id { get; set; }
        public string StudentUserId { get; set; } = "";
        public string TrainerUserId { get; set; } = "";
        public Guid PaymentId { get; set; }
        public int Rating { get; set; }
        public string? Text { get; set; }
        public string? TrainerReply { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? TrainerRepliedAtUtc { get; set; }
    }

    public class ReviewsPageDto
    {
        public List<ReviewDto> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public bool HasMore { get; set; }
    }

    /// <summary>REST-контракт отзывов о тренерах (M8) — чтение для карточки тренера.</summary>
    public interface IReviewsApi
    {
        /// <summary>Отзывы тренера, новые сверху.</summary>
        [Get("/api/reviews/trainer/{trainerId}")]
        Task<IApiResponse<ReviewsPageDto>> GetTrainerReviewsAsync(
            [Header("Authorization")] string token,
            string trainerId,
            [AliasAs("page")] int page = 1,
            [AliasAs("pageSize")] int pageSize = 20,
            CancellationToken ct = default);
    }
}
